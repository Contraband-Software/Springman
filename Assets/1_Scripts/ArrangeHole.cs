using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrangeHole : MonoBehaviour
{
    //GAME DATA REFERENCE
    public GameObject leftFlat;
    public GameObject rightFlat;
    public GameObject leftCap;
    public GameObject rightCap;

    Bounds flatBounds;
    Bounds leftBounds;
    Bounds rightBounds;

    public float holeWidth = 1.5f;

    private Camera cam;
    private Vector3 topRight;
    private float minX;
    private float maxX;

    private void Awake()
    {
        flatBounds = leftFlat.GetComponent<BoxCollider2D>().bounds;
        leftBounds = leftCap.GetComponent<BoxCollider2D>().bounds;
        rightBounds = rightCap.GetComponent<BoxCollider2D>().bounds;

        cam = Camera.main;
    }

    public void ArrangePlatform()
    {
        flatBounds = leftFlat.GetComponent<BoxCollider2D>().bounds;
        leftBounds = leftCap.GetComponent<BoxCollider2D>().bounds;
        rightBounds = rightCap.GetComponent<BoxCollider2D>().bounds;

        Resize();
        PlaceFlats();
        PlaceCaps();
        PositionHoleRandomly();
    }

    private void PlaceCaps()
    {
        float offset = holeWidth / 2 + leftBounds.extents.x + 0.01f;
        leftCap.transform.localPosition = new Vector3(offset, leftCap.transform.localPosition.y, leftCap.transform.localPosition.z);
        rightCap.transform.localPosition = new Vector3(offset * -1, rightCap.transform.localPosition.y, rightCap.transform.localPosition.z);
    }

    void Resize()
    {
        topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane)); //Coords of top right corner of screen
        float currentTrueWidth = flatBounds.extents.x * 2f;
        float newTrueWidth = (topRight.x * 2f) - (rightBounds.extents.x * 2f) - holeWidth;
        float scaleMultiplier = newTrueWidth / currentTrueWidth;

        leftFlat.transform.localScale = new Vector3(leftFlat.transform.localScale.x * scaleMultiplier, leftFlat.transform.localScale.y, leftFlat.transform.localScale.z);
        rightFlat.transform.localScale = new Vector3(rightFlat.transform.localScale.x * scaleMultiplier, rightFlat.transform.localScale.y, rightFlat.transform.localScale.z);
        flatBounds = leftFlat.GetComponent<BoxCollider2D>().bounds;
    }

    void PlaceFlats()
    {
        float offset = holeWidth / 2 + leftBounds.extents.x * 2 + flatBounds.extents.x;
        leftFlat.transform.localPosition = new Vector3(offset * -1, leftFlat.transform.localPosition.y, leftFlat.transform.localPosition.z);
        rightFlat.transform.localPosition = new Vector3(offset, rightFlat.transform.localPosition.y, rightFlat.transform.localPosition.z);
    }

    void PositionHoleRandomly()
    {
        maxX = topRight.x - holeWidth / 2;
        minX = maxX * -1;

        float range = maxX - minX;
        float sample = Random.value;
        float scaled = (sample * range) + minX;
        transform.position = new Vector3(scaled, transform.position.y, transform.position.z);
    }

    #region GETTERS AND SETTERS
    public float GetMinX()
    {
        return minX;
    }

    public float GetMaxX()
    {
        return maxX;
    }
    #endregion
}
