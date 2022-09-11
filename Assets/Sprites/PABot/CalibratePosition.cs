using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibratePosition : MonoBehaviour
{
    public Camera cam;
    public Vector3 topRight;

    [Header("Object Type")]
    public bool isCircle;
    public bool isAreaInd;
    public bool isHand;

    void Start()
    {
        topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane)); //Coords of top right corner of screen

        if (isCircle)
        {
            if (transform.position.x < 0f)
            {
                transform.position = new Vector3(-topRight.x + 0.25f, transform.position.y, transform.position.z);
            }
            else
            {
                transform.position = new Vector3(topRight.x - 0.25f, transform.position.y, transform.position.z);
            }
        }
        if (isAreaInd)
        {
            if (transform.position.x < 0f)
            {
                transform.position = new Vector3(-topRight.x + 0.855427f, transform.position.y, transform.position.z);
            }
            else
            {
                transform.position = new Vector3(topRight.x - 0.855427f, transform.position.y, transform.position.z);
            }
        }
        if (isHand)
        {
            transform.position = new Vector3(topRight.x - 0.717587f, transform.position.y, transform.position.z);
        }
    }

}
