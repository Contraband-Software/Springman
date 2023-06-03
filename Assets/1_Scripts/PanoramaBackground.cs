using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Architecture.Managers;

public class PanoramaBackground : MonoBehaviour
{
    [Header("Pool")]
    public GameObject objectPool;
    public int instancesPerPattern = 3;

    [Serializable]
    public struct PatternSpecs
    {
        [Header("Specs")]
        public Vector4 standard;
        public Vector4 flippedInX;
        public Vector4 flippedInY;
        public Vector4 Rot180;

        public Vector4[] variants;

        [Header("References")]
        public Sprite overlay;
        public Sprite colour;
        public Sprite underlay;
        public GameObject gameObject;

        public void Init()
        {
            variants = new Vector4[] { standard, flippedInX, flippedInY, Rot180 };
        }
    }
    [SerializeField] List<PatternSpecs> panoramaPatterns = new();

    [Header("Colour Changeables")]
    public List<SpriteRenderer> backgroundColors = new List<SpriteRenderer>();

    public enum Orientation { Standard, FlippedInX, FlippedInY, Rot180};


    //Other Variables
    Camera cam;
    Vector3 topRight;
    Vector3 bottomLeft;

    bool firstSpawn;
    float currentY;
    Bounds currentBounds; 

    Orientation currentOrientation;
    PatternSpecs currentPatternSpecs;
    GameObject spawnedPattern;
    PatternSpecs spawnedPatternSpecs;

    bool finishedInit = false;

    System.Random rnd = new System.Random();

    List<GameObject> activeTiles = new List<GameObject>();
    List<GameObject> inactiveTiles = new List<GameObject>();


    Vector2 potentialNextBottomSpecs = Vector2.zero;
    int ranOrientationInt = -1;
    private Vector4 currentOrientationSpecs;

    void Awake()
    {
        firstSpawn = false;
        finishedInit = false;

        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane)); //coords of bottom left corner of screen

        //run the init func on all the pats -
        //create the object pool -
        //put the patspec ref on each pantile -
        //sub event -
        //cache inactive go -
        int id = 0;
        foreach (PatternSpecs p in panoramaPatterns)
        {
            p.Init();

            for (int i = 0; i < instancesPerPattern; i++)
            {
                GameObject tileInstance = Instantiate(p.gameObject, objectPool.transform);
                inactiveTiles.Add(tileInstance);
                PanoramaTile pt = tileInstance.GetComponent<PanoramaTile>();
                pt.offscreenEvent.AddListener(MakeTileInactive);
                pt.patternSpecs = p;

                pt.Init();

                tileInstance.name = "Tile." + id.ToString();

                id++;
            }
        }
    }

    private void MakeTileActive(GameObject tile)
    {
        spawnedPattern = tile;

        tile.GetComponent<PanoramaTile>().panActive = true;

        Debug.Log("Active" + tile.name);

        activeTiles.Add(tile);
        inactiveTiles.Remove(tile);
    }

    private void MakeTileInactive(GameObject tile)
    {

        Debug.Log("inActive" + tile.name);

        activeTiles.Remove(tile);
        inactiveTiles.Add(tile);
    }

    private void Start()
    {
        CollectBackgroundColorRefs();
        FirstPattern();
    }

    public static PanoramaBackground GetReference()
    {
        return GameObject.FindGameObjectWithTag("BackgroundController").GetComponent<PanoramaBackground>();
    }

    void Update()
    {
        topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane)); //Coords of top right corner of screen

        if (finishedInit) { SpawnNextPattern(); }
        else { InitialisePattern(spawnedPattern); }
    }
    /// <summary>
    /// Accesses all children of the object pool to add a reference to the water color
    /// to the list of colours
    /// </summary>
    void CollectBackgroundColorRefs()
    {
        for (int child = 0; child < objectPool.transform.childCount; child++)
        {
            backgroundColors.Add(objectPool.transform.GetChild(child).GetChild(1).GetComponent<SpriteRenderer>());
        }
    }

    /// <summary>
    /// Updates each background of the object pool to match the theme color
    /// </summary>
    public void UpdateBackgroundColours()
    {
        for(int c = 0; c < backgroundColors.Count; c++)
        {
            backgroundColors[c].color = UserGameData.Instance.themeColour;
        }
    }

    void FirstPattern()
    {
        finishedInit = false;

        GameObject spawnedFirstPattern = inactiveTiles[rnd.Next(0, inactiveTiles.Count)].gameObject;
        
        MakeTileActive(spawnedFirstPattern);
        spawnedFirstPattern.transform.position = Vector3.zero;
        spawnedFirstPattern.transform.rotation = Quaternion.identity;

        PanoramaTile firstPatternTile = spawnedFirstPattern.GetComponent<PanoramaTile>();
        PatternSpecs firstPatternSpecs = firstPatternTile.patternSpecs;

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
                Flip(firstPatternTile, true, false);
                break;

            case Orientation.FlippedInY:
                currentOrientationSpecs = firstPatternSpecs.flippedInY;
                Flip(firstPatternTile, false, true);
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

        //apply theme colour from UserGameData
        firstPatternTile.color.color = UserGameData.Instance.themeColour;
    }

    void SpawnNextPattern()
    {
        if (currentY + currentBounds.extents.y < topRight.y + 2f && firstSpawn == true && finishedInit == true)
        {
            print("spawning pattern: currentY=" + currentY.ToString() + " bounds.y=" + currentBounds.extents.y.ToString());

            finishedInit = false;

            float spawnY = currentY + (currentBounds.extents.y * 2); //- 0.01f;

            #region CHOOSE_PATTERN

            //spawning code = just get some random ting off of inactive and bootstrap it VVVV

            //selects random pattern
            GameObject spawnedFirstPattern = inactiveTiles[rnd.Next(0, inactiveTiles.Count)].gameObject;

            MakeTileActive(spawnedFirstPattern);

            #endregion

            #region PLACE_PATTERN
            spawnedPattern.transform.position = Vector3.zero;
            spawnedPattern.transform.rotation = Quaternion.identity;

            //get the specs of the spawned pattern
            PanoramaTile spawnedPatternTile = spawnedPattern.GetComponent<PanoramaTile>();
            spawnedPatternSpecs = spawnedPatternTile.patternSpecs;
            Flip(spawnedPatternTile, false, false);


            spawnedPattern.transform.position = new Vector3(spawnedPattern.transform.position.x, spawnY, spawnedPattern.transform.position.z);

            currentY = spawnedPattern.transform.position.y;
            print("new y bounds will be: " + spawnedPatternTile.bounds.extents.y.ToString());
            currentBounds = spawnedPatternTile.bounds;

            //apply theme colour from UserGameData
            spawnedPatternTile.color.color = UserGameData.Instance.themeColour;
            #endregion
        }

    }

    void InitialisePattern(GameObject spawnedPattern)
    {
        if(finishedInit == false)
        {
            //top specs of the pattern spawned before this one being done now
            Vector2 topCurrentSpecs = new Vector2(
                currentPatternSpecs.variants[(int)currentOrientation].x,
                currentPatternSpecs.variants[(int)currentOrientation].y
            );

            if (topCurrentSpecs != potentialNextBottomSpecs)
            {

                ranOrientationInt = rnd.Next(0, 4);

                potentialNextBottomSpecs = new Vector2(spawnedPatternSpecs.variants[ranOrientationInt].z,
                    spawnedPatternSpecs.variants[ranOrientationInt].w);

            }
            else
            {
                PanoramaTile spawnPatTile = spawnedPattern.GetComponent<PanoramaTile>();
                PatternSpecs spawnPatSpecs = spawnPatTile.patternSpecs;

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
                        Flip(spawnPatTile, true, false);
                        break;

                    case Orientation.FlippedInY:
                        currentOrientationSpecs = spawnedPatternSpecs.flippedInY;
                        Flip(spawnPatTile, false, true);
                        break;

                    case Orientation.Rot180:
                        currentOrientationSpecs = spawnedPatternSpecs.Rot180;
                        spawnedPattern.transform.Rotate(0f, 0f, 180f);
                        break;

                }
                currentPatternSpecs = spawnedPatternSpecs;
                currentY = spawnedPattern.transform.position.y;

                ranOrientationInt = -1;
                potentialNextBottomSpecs = Vector2.zero;
                finishedInit = true;
            }
        }
    }

    void Flip(PanoramaTile patSpecs, bool flipX, bool flipY)
    {
        patSpecs.overlay.flipX = flipX;
        patSpecs.color.flipX = flipX;
        patSpecs.underlay.flipX = flipX;

        patSpecs.overlay.flipY = flipY;
        patSpecs.color.flipY = flipY;
        patSpecs.underlay.flipY = flipY;
    }
}
