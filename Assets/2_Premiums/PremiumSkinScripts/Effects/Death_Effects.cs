using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death_Effects : MonoBehaviour
{

    private string premium_name;

    [Header("Important References")]
    [HideInInspector]
    public PremSkinDetailsDemo premDetails;
    [HideInInspector]
    public PlayerController playerCon;
    [HideInInspector]
    public EffectController effectCon;
    [HideInInspector]
    public GameData gameData;

    [Header("Details")]
    public string deathPassiveName;
    public bool hasDeathPassiveSound = false;
    public bool hasDeathPassiveAnim = false;

    [Header("Death Animator")]
    public Animator death_animator;

    [Header("SoundSources")]
    public AudioSource passive_death_audioSource;

    [Header("Charged Details")]
    public SpriteRenderer firstGlowPip;
    public Sprite redPipSprite;

    [Header("Alien Details")]
    public SpriteRenderer glowBG;
    public SpriteRenderer glowFG;
    public SpriteRenderer glint;
    public Sprite shatteredGlass;
    public GameObject pSys_GlassShatter;
    public GameObject pSys_WaterSpill;
    public GameObject dude;
    public float colourVariation;

    [Header("LightBulb Details")]
    public Animator lightbulbDeathAnimator;
    public ParticleSystem pSys_BulbShatter;

    [Header("Molten Core Details")]
    public SpriteRenderer moltenCoreGlow_1;
    public SpriteRenderer moltenCoreGlow_2;
    private Color MC_startColor_1;
    private Color MC_startColor_2;
    public ParticleSystem pSys_moltenCore_steam;

    [Header("Neon Details")]
    public List<TransparencyAnimator> trAnimList = new List<TransparencyAnimator>();

    [Header("PC Details")]
    public GameObject dead_face;
    public List<GameObject> turn_off_objects = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        premDetails = gameObject.GetComponent<PremSkinDetailsDemo>();
        premium_name = premDetails.cosData.activePremiumSkinName;

        //LOAD AUDIO EFFECTS
        if (hasDeathPassiveSound)
        {
            Sound loaded_PassiveDeathSound = playerCon.FindSound(deathPassiveName, effectCon.death_sounds);
            passive_death_audioSource.clip = loaded_PassiveDeathSound.clip;
            passive_death_audioSource.volume = loaded_PassiveDeathSound.volume;
        }
    }

    public void PlayEffect()
    {
        if(deathPassiveName != "" && hasDeathPassiveAnim)
        {
            death_animator.Play(deathPassiveName);
        }
        if (gameData.soundsOn && hasDeathPassiveSound)
        {
            passive_death_audioSource.Play();
        }



        //function assignment based off name
        switch (premium_name)
        {
            case "Artifact":
                Artifact();
                break;

            case "Charged":
                Charged();
                break;

            case "Alien":
                Alien();
                break;

            case "LightBulb":
                LightBulb();
                break;

            case "Molten Core":
                MoltenCore();
                break;

            case "Neon":
                Neon();
                break;

            case "PC":
                PC();
                break;

            default:
                break;
        }
        //Artifact();
        // 
    }

    private void InitializeColour(ParticleSystem pSys)
    {
        var main = pSys.main;

        Color baseColor = premDetails.targetColor;
        float r = baseColor.r * 255f;
        float g = baseColor.g * 255f;
        float b = baseColor.b * 255f;
        float a = baseColor.a * 255f;

        List<float> colorVals = new List<float>() { r, g, b };
        List<float> newColor = new List<float>() { r, g, b, a };

        float highest = 0f;

        foreach (float val in colorVals)
        {
            if (val > highest)
            {
                highest = val;
            }
        }
        colorVals.Remove(highest);

        float _2ndHighest = 0f;
        foreach (float val in colorVals)
        {
            if (val > _2ndHighest)
            {
                _2ndHighest = val;
            }
        }
        float distanceToTravel = Mathf.RoundToInt((255f - _2ndHighest) * colourVariation);
        colorVals.Remove(_2ndHighest);
        float stepRatio = (255f - colorVals[0]) / (255f - _2ndHighest);

        float lowest = colorVals[0];

        newColor[newColor.IndexOf(_2ndHighest)] = _2ndHighest + distanceToTravel;
        newColor[newColor.IndexOf(lowest)] = lowest + (distanceToTravel * stepRatio);

        Color otherColor = new Color(newColor[0] / 255f, newColor[1] / 255f, newColor[2] / 255f, newColor[3] / 255f);

        main.startColor = new ParticleSystem.MinMaxGradient(baseColor, otherColor);
    }

    private void Artifact()
    {
        death_animator.Play("wing_flutter");
    }

    private void Charged()
    {
        firstGlowPip.sprite = redPipSprite;
        death_animator.Play("no_battery");
    }

    private void Alien()
    {
        if(playerCon.deathBy != PlayerController.DeathBy.Water)
        {
            Sound loaded_PassiveDeathSound = playerCon.FindSound(deathPassiveName, effectCon.death_sounds);
            passive_death_audioSource.clip = loaded_PassiveDeathSound.clip;
            passive_death_audioSource.volume = loaded_PassiveDeathSound.volume;
            passive_death_audioSource.Play();

            glowBG.color = new Color(1f, 1f, 1f, 1f);
            glowBG.sprite = shatteredGlass;
            glowFG.enabled = false;
            glint.enabled = false;

            GameObject dudeClone = Instantiate(dude, transform.position, Quaternion.identity, null);
            Destroy(dude);
            dude = dudeClone;
            dude.GetComponent<Animator>().Play("rotate");

            Vector3 localSize = pSys_GlassShatter.transform.localScale;
            ParticleSystem pSys = pSys_GlassShatter.GetComponent<ParticleSystem>();
            pSys_GlassShatter.transform.SetParent(null);
            pSys_GlassShatter.transform.localScale = localSize;
            pSys.Play();

            Vector3 localSize2 = pSys_WaterSpill.transform.localScale;
            ParticleSystem pSys2 = pSys_WaterSpill.GetComponent<ParticleSystem>();
            InitializeColour(pSys2);
            pSys_WaterSpill.transform.SetParent(null);
            pSys_WaterSpill.transform.localScale = localSize2;
            pSys2.Play();
        }
    }

    private void LightBulb()
    {
        if(playerCon.deathBy != PlayerController.DeathBy.Water)
        {
            lightbulbDeathAnimator.Play("death_lightbulb_shatter");

            Sound loaded_PassiveDeathSound = playerCon.FindSound(deathPassiveName, effectCon.death_sounds);
            passive_death_audioSource.clip = loaded_PassiveDeathSound.clip;
            passive_death_audioSource.volume = loaded_PassiveDeathSound.volume;
            passive_death_audioSource.Play();

            Vector3 localSize = pSys_BulbShatter.transform.localScale;
            ParticleSystem pSys = pSys_BulbShatter.GetComponent<ParticleSystem>();
            pSys_BulbShatter.transform.SetParent(null);
            pSys_BulbShatter.transform.localScale = localSize;
            pSys.Play();
        }
    }

    private void MoltenCore()
    {
        MC_startColor_1 = moltenCoreGlow_1.color;
        MC_startColor_2 = moltenCoreGlow_2.color;
        LeanTween.value(gameObject, MoltenCoreCallback, 1f, 0f, 0.7f);

        if (playerCon.deathBy == PlayerController.DeathBy.Water)
        {
            Sound loaded_PassiveDeathSound = playerCon.FindSound(deathPassiveName, effectCon.death_sounds);
            passive_death_audioSource.clip = loaded_PassiveDeathSound.clip;
            passive_death_audioSource.volume = loaded_PassiveDeathSound.volume;
            passive_death_audioSource.Play();

            pSys_moltenCore_steam.Play();
        }
    }
    private void MoltenCoreCallback(float v)
    {
        Color glow_1_Col = MC_startColor_1;
        glow_1_Col.r *= v;
        glow_1_Col.g *= v;
        glow_1_Col.b *= v;
        moltenCoreGlow_1.color = glow_1_Col;

        Color glow_2_Col = MC_startColor_2;
        glow_2_Col.r *= v;
        glow_2_Col.g *= v;
        glow_2_Col.b *= v;
        moltenCoreGlow_2.color = glow_2_Col;
    }

    private void Neon()
    {
        foreach(TransparencyAnimator trAnim in trAnimList)
        {
            trAnim.setInitialColour();
            trAnim.fadeToTargetBlack();
        }
    }

    private void PC()
    {
        death_animator.Play("pc_death_passive_2");
    }
}
