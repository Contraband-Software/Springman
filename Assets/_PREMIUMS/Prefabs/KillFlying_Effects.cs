using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillFlying_Effects : MonoBehaviour
{
    private string premium_name;

    [Header("Important References")]
    [HideInInspector]
    public PremSkinDetailsDemo premDetails;
    [HideInInspector]
    public PlayerController playerCon;
    private TapToKill tapToKillReference;

    [Header("Details")]
    public bool manuallyTriggerFE_DeathAnim = false;
    private GameObject flyingEnemyObject;

    [Header("Artifact Details")]
    public Animator artifactAnimator;
    public GameObject missilePrefab;

    [Header("Charged Details")]
    public Animator chargedAnimator;

    [Header("Gyrocube Details")]
    public GameObject beamObject;

    [Header("Stomper Details")]
    public GameObject electricArc;
    public GameObject firePoint;
    public Animator stomperAnimator;

    [Header("DC_Details")]
    public Animator dc_anim;
    public DC_CannonController dc_cannonCon;

    public void Start()
    {
        premDetails = gameObject.GetComponent<PremSkinDetailsDemo>();
        premium_name = premDetails.cosData.activePremiumSkinName;
        tapToKillReference = playerCon.GetComponent<TapToKill>();
    }


    public void PlayEffect(GameObject flyingEnemyObject = null)
    {
        this.flyingEnemyObject = flyingEnemyObject;

        //function assignment based off name
        switch (premium_name)
        {
            case "Artifact":
                Artifact();
                break;

            case "Charged":
                Charged();
                break;

            case "Gyrocube":
                Gyrocube();
                break;

            case "Stomper":
                Stomper();
                break;

            case "Dual Cannon":
                DualCannon();
                break;

            default:
                break;
        }

        this.flyingEnemyObject = null;
    }

    private void Artifact()
    {
        //print("APPLYING CUSTOM KILL ANIMATION");
        Beam enemyBeamScript = flyingEnemyObject.transform.Find("Cannon/Barrel/ShootPoint").GetComponent<Beam>();
        enemyBeamScript.attack = false;

        //Instantiate Missile Prefab
        GameObject spawnedMissile = Instantiate(missilePrefab, gameObject.transform.root.position, Quaternion.identity, null);
        ArtifactMissile missileScript = spawnedMissile.GetComponent<ArtifactMissile>();
        missileScript.premDetails = premDetails;
        missileScript.target = flyingEnemyObject;
        missileScript.pCon = playerCon;
        missileScript.tapToKillReference = tapToKillReference;
        missileScript.gameData = playerCon.gamedata;
        missileScript.playerConAnimator = artifactAnimator;
    }

    private void Charged()
    {
        chargedAnimator.Play("enemy_killed");
    }

    private void Gyrocube()
    {
        Vector3 enemyPos = flyingEnemyObject.transform.position;
        GameObject beamObjectClone = Instantiate(beamObject, beamObject.transform.position, Quaternion.identity);
        beamObjectClone.transform.SetParent(null);
        GyroBeam gyroBeamScript = beamObjectClone.GetComponent<GyroBeam>();
        gyroBeamScript.premDetails = premDetails;
        gyroBeamScript.FireAtEnemy(transform.position, enemyPos, flyingEnemyObject);


        tapToKillReference.KillFlyingEnemy(flyingEnemyObject);
    }

    private void Stomper()
    {
        stomperAnimator.Play("kill_stomper");

        Vector3 enemyPos = flyingEnemyObject.transform.position;
        GameObject spawnedArc = Instantiate(electricArc, null, true);
        spawnedArc.transform.position = transform.position;
        ArcFire arcScript = spawnedArc.GetComponent<ArcFire>();
        arcScript.premDetails = premDetails;
        arcScript.StartArc(firePoint.transform, enemyPos);

        tapToKillReference.KillFlyingEnemy(flyingEnemyObject);
    }

    private void DualCannon()
    {
        dc_anim.Play("dualcannon_glow_fire");
        dc_cannonCon.AimAtPosition(flyingEnemyObject.transform.position, premDetails, flyingEnemyObject);

        tapToKillReference.KillFlyingEnemy(flyingEnemyObject);
    }
}
