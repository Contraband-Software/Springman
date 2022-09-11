using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapToKill : MonoBehaviour
{
    public SpawnFlyingEnemy spawnFEscript;
    public GameData gameData;
    public PlayerController pController;

    public EffectController effectCon;

    [SerializeField] public LayerMask layerMask;


    private GameObject tempFEKilled;

    void Start()
    {
        spawnFEscript = gameObject.GetComponent<SpawnFlyingEnemy>();
        gameData = GameObject.Find("GameController").GetComponent<GameData>();
        pController = spawnFEscript.gameObject.GetComponent<PlayerController>();
    }
    void Update()
    {
        if (!gameData.Paused && pController.state == PlayerController.State.Alive)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.forward, Mathf.Infinity, layerMask);

                if (hit.collider != null)
                {
                    if (hit.collider.tag == "SittingEnemy")
                    {
                        SlideMove slideMove = hit.collider.transform.root.gameObject.GetComponent<SlideMove>();
                        slideMove.sittingEnemySpawned = false;

                        if (effectCon.premiumSkinActive)
                        {
                            effectCon.KillSittingEnemyEffect(hit.collider.transform.gameObject);
                        }
                        SittingEnemyAnimation(hit.collider.transform.gameObject);
                        //Destroy(hit.collider.gameObject);
                    }
                    if (hit.collider.tag == "FlyingEnemy" && tempFEKilled == null)
                    {
                        tempFEKilled = hit.collider.transform.root.gameObject;
                        if (effectCon.AllowAutoKillFlyingEnemy())
                        {
                            KillFlyingEnemy();

                            if (effectCon.premiumSkinActive)
                            {
                                effectCon.KillFlyingEnemyEffect();
                            }    
                        }
                        else
                        {
                            effectCon.KillFlyingEnemyEffect(tempFEKilled);
                        }
                    }
                }
            }
        }
    }
    GameObject bottomfall;
    ParticleSystem explosion;
    GameObject topfall;

    ParticleSystem bottomfall2;
    ParticleSystem explosion2;
    ParticleSystem topfall2;

    GameObject barrelfallEncap;
    GameObject barrelFall;

    GameObject sEm_FE;
    GameObject sEm2_FE;
    GameObject hover_emitter;
    float hoverAudioVolume;

    GameObject sEm_SE;
    GameObject sEm2_SE;

    public void KillFlyingEnemy(GameObject externalTarget = null)
    {
        if(externalTarget != null)
        {
            tempFEKilled = externalTarget;
        }

        FlyingEnemyAnimation(tempFEKilled);
        spawnFEscript.flyingEnemySpawned = false;
        gameData.flyingEnemiesKilled++;
        spawnFEscript.ControlSpawnRate();
        tempFEKilled = null;
        gameData.enemiesActive.Remove(externalTarget);
    }

    void SittingEnemyAnimation(GameObject se)
    {
        gameData.enemiesActive.Remove(se);

        bottomfall = se.transform.GetChild(0).gameObject;
        explosion = se.transform.GetChild(1).GetComponent<ParticleSystem>();
        topfall = se.transform.GetChild(2).gameObject;

        bottomfall.transform.SetParent(null);
        explosion.transform.SetParent(null);
        topfall.transform.SetParent(null);


        sEm_SE = se.transform.Find("Sound_Emitter").gameObject;
        sEm2_SE = se.transform.Find("Sound_Emitter (1)").gameObject;

        if (gameData.soundsOn)
        {
            sEm2_SE.GetComponent<AudioSource>().Play();
        }
        

        sEm_SE.transform.SetParent(null);
        sEm2_SE.transform.SetParent(null);

        sEm_SE.GetComponent<SelfDestruct>().DestructionCountdown();
        sEm2_SE.GetComponent<SelfDestruct>().DestructionCountdown();

        explosion.Play();

        LeanTween.delayedCall(0.05f, fallAnimations).setIgnoreTimeScale(true);
        Destroy(se);
    }

    void FlyingEnemyAnimation(GameObject fe)
    {

        //sound
        sEm_FE = fe.transform.Find("Sound_Emitter").gameObject;
        sEm2_FE = fe.transform.Find("Sound_Emitter (1)").gameObject;
        hover_emitter = fe.transform.Find("Sound_Emitter (2)").gameObject;
        hoverAudioVolume = hover_emitter.GetComponent<AudioSource>().volume;

        sEm_FE.transform.SetParent(null);
        sEm2_FE.transform.SetParent(null);
        hover_emitter.transform.SetParent(null);

        if (gameData.soundsOn)
        {
            sEm_FE.GetComponent<AudioSource>().Play();
        }
        

        sEm_FE.GetComponent<SelfDestruct>().DestructionCountdown();
        sEm2_FE.GetComponent<SelfDestruct>().DestructionCountdown();
        hover_emitter.GetComponent<SelfDestruct>().DestructionCountdown();
        hover_emitter.GetComponent<SelfDestruct>().fadeVolumeOut();
        //


        bottomfall2 = fe.transform.GetChild(2).GetComponent<ParticleSystem>();
        explosion2 = fe.transform.GetChild(3).GetComponent<ParticleSystem>();
        topfall2 = fe.transform.GetChild(4).GetComponent<ParticleSystem>();
        barrelfallEncap = fe.transform.GetChild(5).gameObject;

        bottomfall2.transform.SetParent(null);
        explosion2.transform.SetParent(null);
        topfall2.transform.SetParent(null);

        barrelfallEncap.transform.SetParent(null);
        barrelfallEncap.SetActive(true);

        barrelFall = barrelfallEncap.transform.GetChild(0).gameObject;

        float zCannonRotation = fe.transform.GetChild(0).transform.rotation.z;

        Quaternion setRot = Quaternion.identity;
        setRot.z = zCannonRotation;
        barrelfallEncap.transform.rotation = setRot;

        LeanTween.color(barrelFall, Color.clear, 1f).setIgnoreTimeScale(true).setOnComplete(DeleteBarell);
        if(zCannonRotation < -90f)
        {
            LeanTween.rotateZ(barrelFall, zCannonRotation - 200f, 1f).setIgnoreTimeScale(true);
        }
        else
        {
            LeanTween.rotateZ(barrelFall, zCannonRotation + 200f, 1f).setIgnoreTimeScale(true);
        }
        

        explosion2.Play();

        LeanTween.delayedCall(0.05f, fallAnimations2).setIgnoreTimeScale(true);
        Destroy(fe);
    }

    void DeleteBarell()
    {
        Destroy(barrelfallEncap);
    }

    void fallAnimations()
    {
        bottomfall.GetComponent<ParticleSystem>().Play();
        topfall.GetComponent<ParticleSystem>().Play();
    }

    void fallAnimations2()
    {
        bottomfall2.Play();
        topfall2.Play();
    }
}
