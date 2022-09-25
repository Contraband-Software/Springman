using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillSitting_Effects : MonoBehaviour
{
    private string premium_name;

    [Header("Important References")]
    [HideInInspector]
    public PremSkinDetailsDemo premDetails;
    [HideInInspector]
    public PlayerController playerCon;

    [Header("Details")]
    GameObject sittingEnemyObject;

    [Header("Charged Details")]
    public Animator chargedAnimator;

    [Header("Gyrocube Details")]
    public GameObject beamObject;

    [Header("Stomper Details")]
    public GameObject electricArc;
    public GameObject firePoint;
    public Animator stomperAnimator;

    [Header("DC Details")]
    public Animator dc_anim;
    public DC_CannonController dc_cannonCon;

    [Header("PC Deatials")]
    public Animator pc_anim;

    // Start is called before the first frame update
    void Start()
    {
        premDetails = gameObject.GetComponent<PremSkinDetailsDemo>();
        premium_name = premDetails.cosData.activePremiumSkinName;
    }

    public void PlayEffect(GameObject sittingEnemyObject = null)
    {
        this.sittingEnemyObject = sittingEnemyObject;

        //function assignment based off name
        switch (premium_name)
        {
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

            case "PC":
                PC();
                break;

            default:
                break;
        }

        this.sittingEnemyObject = null;
    }

    private void Charged()
    {
        chargedAnimator.Play("enemy_killed");
    }

    private void Gyrocube()
    {
        Vector3 enemyPos = sittingEnemyObject.transform.position;
        GameObject beamObjectClone = Instantiate(beamObject, beamObject.transform.position, Quaternion.identity);
        beamObjectClone.transform.SetParent(null);
        GyroBeam gyroBeamScript = beamObjectClone.GetComponent<GyroBeam>();
        gyroBeamScript.premDetails = premDetails;
        gyroBeamScript.FireAtEnemy(transform.position, enemyPos, sittingEnemyObject);
    }

    private void Stomper()
    {
        stomperAnimator.Play("kill_stomper");

        Vector3 enemyPos = sittingEnemyObject.transform.position;
        GameObject spawnedArc = Instantiate(electricArc, null, true);
        spawnedArc.transform.position = transform.position;
        ArcFire arcScript = spawnedArc.GetComponent<ArcFire>();
        arcScript.premDetails = premDetails;
        arcScript.StartArc(firePoint.transform, enemyPos);

    }

    private void DualCannon()
    {
        dc_anim.Play("dualcannon_glow_fire");
        print("FIRE");
        dc_cannonCon.AimAtPosition(sittingEnemyObject.transform.position, premDetails, sittingEnemyObject);
    }
    private void PC()
    {
        if (!pc_anim.GetBool("moneyAnimPlaying"))
        {
            pc_anim.Play("pc_flash_kill");
        }
    }

}
