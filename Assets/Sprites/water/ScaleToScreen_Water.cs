using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleToScreen_Water : MonoBehaviour
{
    Camera cam;
    Vector3 topRight;
    public float OscillationSpeed;

    private bool waterOscillating = false;

    void Start()
    {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane)); //Coords of top right corner of screen

        transform.localScale = new Vector3(topRight.x * 4.01f, topRight.x * 4.01f, 0f);
        transform.position = new Vector3(0f - topRight.x, transform.position.y, 0f);
    }

    void Update()
    {
        if (!waterOscillating)
        {
            LeanTween.moveX(gameObject, topRight.x, OscillationSpeed).setOnComplete(ResetPosition);
            waterOscillating = true;
        }
    }

    public void ResetPosition()
    {
        transform.position = new Vector3(0f - topRight.x, transform.position.y, 0f);
        LeanTween.moveX(gameObject, topRight.x, OscillationSpeed).setOnComplete(ResetPosition);
    }

}
