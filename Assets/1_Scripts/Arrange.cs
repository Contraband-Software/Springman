using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrange : MonoBehaviour
{
    //GAME DATA REFERENCE

    public GameObject mid;
    public GameObject left;
    public GameObject right;
    public GameObject ind;

    Bounds midBounds;
    Bounds leftBounds;
    Bounds rightBounds;

    private void Awake()
    {
        midBounds = mid.GetComponent<BoxCollider2D>().bounds;
        leftBounds = left.GetComponent<BoxCollider2D>().bounds;
        rightBounds = right.GetComponent<BoxCollider2D>().bounds;
    }

    public void ArrangePlatform()
    {
        Resize();
        PlaceCaps();
        PositionIndicator();
    }

    private void PlaceCaps()
    {
        midBounds = mid.GetComponent<BoxCollider2D>().bounds;
        leftBounds = left.GetComponent<BoxCollider2D>().bounds;
        rightBounds = right.GetComponent<BoxCollider2D>().bounds;

        left.transform.position = new Vector3(mid.transform.position.x - midBounds.extents.x - leftBounds.extents.x + 0.01f,
                    left.transform.position.y, left.transform.position.z); //snap left cap

        right.transform.position = new Vector3(mid.transform.position.x + midBounds.extents.x + rightBounds.extents.x - 0.01f,
            right.transform.position.y, right.transform.position.z); //snap right cap
    }

    void Resize()
    {
        //float excludeCaps = gameData.platLength - (leftBounds.extents.x * 4) - 0.02f;
        //float scaleMultiplier = excludeCaps / (midBounds.extents.x * 2);

        //mid.transform.localScale = new Vector3(mid.transform.localScale.x * scaleMultiplier, mid.transform.localScale.y, mid.transform.localScale.z);
    }
    
    void PositionIndicator()
    {
        if(transform.position.x < 0) //if platform spawn left side
        {
            ind.transform.position = new Vector3(right.transform.position.x + 0.43f, ind.transform.position.y, ind.transform.position.z);
        }
        else
        {
            ind.transform.position = new Vector3(left.transform.position.x - 0.43f, ind.transform.position.y, ind.transform.position.z);
            ind.transform.rotation = new Quaternion(0f, 0f, 180f, transform.rotation.w);
        }
    }
}
