using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC_CannonController : MonoBehaviour
{
    public GameObject DC_Beam;
    public GameObject shootPoint1;
    public GameObject shootPoint2;


    public void AimAtPosition(Vector2 targetPos, PremSkinDetailsDemo premDeets, GameObject target)
    {
        Vector2 aimDir = targetPos - new Vector2(transform.position.x, transform.position.y);
        float angle = (Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg) + 180f;
        Vector3 rot = transform.eulerAngles;
        rot.z = angle;
        transform.eulerAngles = rot;

        GameObject beamObjectClone = Instantiate(DC_Beam, DC_Beam.transform.position, Quaternion.identity);
        beamObjectClone.transform.SetParent(null);
        DC_Beam DC_BeamScript = beamObjectClone.GetComponent<DC_Beam>();
        DC_BeamScript.premDetails = premDeets;

        //DC beam fire
        DC_BeamScript.FireAtEnemy(shootPoint1.transform.position, shootPoint2.transform.position, targetPos, target);
    }
}
