using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleDown : MonoBehaviour
{

    public GameObject leftCap;
    public GameObject rightCap;

    Bounds leftBounds;
    Bounds rightBounds;

    Vector3 topRight;

    bool platformBecameVisible;

    float shrinkRate = 0.4f;

    Vector3 spawnPoint;
    float distTravelled;

    void Start()
    {
        spawnPoint = transform.position;

        topRight = transform.GetComponentInParent<SlideMove>().topRight;

        leftBounds = leftCap.GetComponent<BoxCollider2D>().bounds;
        rightBounds = rightCap.GetComponent<BoxCollider2D>().bounds;
    }

    void Update()
    {
        distanceFromOrigin();
        OnScreen();

        Mathf.Clamp(shrinkRate, 0f, 20f);
        if (platformBecameVisible)
        {
            transform.localScale = new Vector3(Mathf.Max(transform.localScale.x - (shrinkRate * Time.unscaledDeltaTime), 0),
                Mathf.Max(transform.localScale.y - (shrinkRate * Time.unscaledDeltaTime), 0), transform.localScale.z);
            shrinkRate += (distTravelled * 2f) / 10f;
        }

        if (!platformBecameVisible)
        {
            if(transform.position.x < 0)
            {
                transform.localPosition = new Vector3(rightCap.transform.localPosition.x + 0.43f, 0f, 0f);
                transform.rotation = new Quaternion(0f, 0f, 0f, transform.rotation.w);
            }
            if(transform.position.x > 0)
            {
                transform.localPosition = new Vector3(leftCap.transform.localPosition.x - 0.43f, 0f, 0f);
                transform.rotation = new Quaternion(0f, 0f, 180f, transform.rotation.w);
            }

            transform.localScale = new Vector3(Mathf.Min(transform.localScale.x + (shrinkRate * Time.unscaledDeltaTime), 0.2f),
                Mathf.Min(transform.localScale.y + (shrinkRate * Time.unscaledDeltaTime), 0.2f), transform.localScale.z);
        }
    }
    private void OnScreen()
    {
        if(transform.position.x < 0)
        {
            if(rightCap.transform.position.x + rightBounds.extents.x - 0.2f > topRight.x - (topRight.x * 2))
            {
                platformBecameVisible = true;
            }
            else
            {
                shrinkRate = 0.4f;
                platformBecameVisible = false;
            }
        }
        if(transform.position.x > 0)
        {

            if (leftCap.transform.position.x - leftBounds.extents.x + 0.2f < topRight.x)
            {
                platformBecameVisible = true;
            }
            else
            {
                shrinkRate = 0.4f;
                platformBecameVisible = false;
            }
        }
    }

    void distanceFromOrigin()
    {
        distTravelled = Math.Max(transform.position.x, spawnPoint.x) - Math.Min(transform.position.x, spawnPoint.x);
    }
}
