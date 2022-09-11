using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanoramaBackground : MonoBehaviour
{
    public GameObject objectPool;

    //Pattern Variables
    public GameObject pattern1;
    public GameObject pattern2;
    public GameObject pattern3;

    PatSpecs pat1Specs;
    PatSpecs pat2Specs;
    PatSpecs pat3Specs;

    PatSpecs[] patterns;

    public enum Orientation { Standard, FlippedInX, FlippedInY, Rot180};


    //Other Variables
    Camera cam;
    Vector3 topRight;
    Vector3 bottomLeft;

    bool firstSpawn;
    float currentY;
    Bounds currentBounds;

    Vector4 currentOrientationSpecs;
    Orientation currentOrientation;
    PatSpecs currentPatternSpecs;
    GameObject spawnedPattern;
    PatSpecs spawnedPatternSpecs;

    bool finishedInit = false;

    public System.Random rnd = new System.Random();

    void Awake()
    {
        firstSpawn = false;
        finishedInit = false;

        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane)); //coords of bottom left corner of screen

        pat1Specs = pattern1.GetComponent<PatSpecs>();
        pat2Specs = pattern2.GetComponent<PatSpecs>();
        pat3Specs = pattern3.GetComponent<PatSpecs>();

        patterns = new PatSpecs[] { pat1Specs, pat2Specs, pat3Specs };

    }
    private void Start()
    {
        FirstPattern();
    }

    void Update()
    {
        topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane)); //Coords of top right corner of screen

        if (finishedInit) { SpawnNextPattern(); }
        else { InitialisePattern(spawnedPattern); }
    }

    void FirstPattern()
    {
        finishedInit = false;

        string firstPattern = patterns[rnd.Next(0, 3)].gameObject.name;
        GameObject spawnedFirstPattern = null;
        for(int i = 0; i < objectPool.transform.childCount; i++)
        {
            GameObject child = objectPool.transform.GetChild(i).gameObject;
            if(!child.activeSelf && child.name == firstPattern)
            {
                spawnedFirstPattern = child;
                break;
            }
        }
        spawnedFirstPattern.SetActive(true);
        spawnedFirstPattern.transform.position = Vector3.zero;
        spawnedFirstPattern.transform.rotation = Quaternion.identity;

        PatSpecs firstPatternSpecs = spawnedFirstPattern.GetComponent<PatSpecs>();
        SpriteRenderer firstPatternSprite = spawnedFirstPattern.GetComponent<SpriteRenderer>();

        currentBounds = spawnedFirstPattern.GetComponent<BoxCollider2D>().bounds;

        spawnedFirstPattern.transform.position = new Vector3(spawnedFirstPattern.transform.position.x,
            bottomLeft.y + currentBounds.extents.y, spawnedFirstPattern.transform.position.z);



        currentOrientation = (Orientation)rnd.Next(0, 4);
        switch (currentOrientation)
        {
            case Orientation.Standard:
                currentOrientationSpecs = firstPatternSpecs.standard;
                break;

            case Orientation.FlippedInX:
                currentOrientationSpecs = firstPatternSpecs.flippedInX;
                firstPatternSprite.flipX = true;
                break;

            case Orientation.FlippedInY:
                currentOrientationSpecs = firstPatternSpecs.flippedInY;
                firstPatternSprite.flipY = true;
                break;

            case Orientation.Rot180:
                currentOrientationSpecs = firstPatternSpecs.Rot180;
                spawnedFirstPattern.transform.Rotate(0f, 0f, 180f);
                break;

        }
        currentY = spawnedFirstPattern.transform.position.y;
        currentPatternSpecs = firstPatternSpecs;

        firstSpawn = true;
        finishedInit = true;
    }

    void SpawnNextPattern()
    {
        if (currentY + currentBounds.extents.y < topRight.y + 2f && firstSpawn == true && finishedInit == true)
        {
            finishedInit = false;

            float spawnY = currentY + (currentBounds.extents.y * 2) - 0.01f;

            bool patternFound = false;
            string pattern = patterns[rnd.Next(0, 3)].gameObject.name;
            for (int i = 0; i < objectPool.transform.childCount; i++)
            {
                GameObject child = objectPool.transform.GetChild(i).gameObject;
                if (!child.activeSelf && child.name == pattern)
                {
                    spawnedPattern = child;
                    patternFound = true;
                    break;
                }
            }
            if (!patternFound)
            {
                GameObject clonedPattern = Instantiate(objectPool.transform.Find(pattern).gameObject, Vector3.zero, Quaternion.identity, objectPool.transform);
                clonedPattern.name = pattern;
                spawnedPattern = clonedPattern;
            }

            spawnedPattern.SetActive(true);
            spawnedPattern.transform.position = Vector3.zero;
            spawnedPattern.transform.rotation = Quaternion.identity;
            SpriteRenderer sP_renderer = spawnedPattern.GetComponent<SpriteRenderer>();
            sP_renderer.flipX = false;
            sP_renderer.flipY = false;

            //get the specs of the spawned pattern
            spawnedPatternSpecs = spawnedPattern.GetComponent<PatSpecs>();



            spawnedPattern.transform.position = new Vector3(spawnedPattern.transform.position.x, spawnY, spawnedPattern.transform.position.z);

            currentY = spawnedPattern.transform.position.y;
            currentBounds = sP_renderer.bounds;
        }
           
    }


    Vector2 potentialNextBottomSpecs = Vector2.zero;
    int ranOrientationInt = -1;
    void InitialisePattern(GameObject spawnedPattern)
    {
        if(finishedInit == false)
        {
            //top specs of the pattern spawned before this one being done now
            Vector2 topCurrentSpecs = new Vector2(currentPatternSpecs.variants[(int)currentOrientation].x,
                currentPatternSpecs.variants[(int)currentOrientation].y);

            if (topCurrentSpecs != potentialNextBottomSpecs)
            {

                ranOrientationInt = rnd.Next(0, 4);

                potentialNextBottomSpecs = new Vector2(spawnedPatternSpecs.variants[ranOrientationInt].z,
                    spawnedPatternSpecs.variants[ranOrientationInt].w);

            }
            else
            {

                SpriteRenderer spawnedPatternSprite = spawnedPattern.GetComponent<SpriteRenderer>();

                //Adjusting the pattern to the correct orientation

                currentOrientationSpecs = spawnedPatternSpecs.variants[ranOrientationInt];
                currentOrientation = (Orientation)ranOrientationInt;

                switch (currentOrientation)
                {
                    case Orientation.Standard:
                        currentOrientationSpecs = spawnedPatternSpecs.standard;
                        break;

                    case Orientation.FlippedInX:
                        currentOrientationSpecs = spawnedPatternSpecs.flippedInX;
                        spawnedPatternSprite.flipX = true;
                        break;

                    case Orientation.FlippedInY:
                        currentOrientationSpecs = spawnedPatternSpecs.flippedInY;
                        spawnedPatternSprite.flipY = true;
                        break;

                    case Orientation.Rot180:
                        currentOrientationSpecs = spawnedPatternSpecs.Rot180;
                        spawnedPattern.transform.Rotate(0f, 0f, 180f);
                        break;

                }
                currentPatternSpecs = spawnedPatternSpecs;
                currentY = spawnedPattern.transform.position.y;
                currentBounds = spawnedPattern.GetComponent<SpriteRenderer>().bounds;

                ranOrientationInt = -1;
                potentialNextBottomSpecs = Vector2.zero;
                finishedInit = true;
            }
        }
    }
}
