using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PanoramaTile : MonoBehaviour
{
    public SpriteRenderer overlay;
    public SpriteRenderer color;
    public SpriteRenderer underlay;

    Camera cam;

    Vector3 topRight;
    Vector3 bottomLeft;

    public Bounds bounds { private set; get; }

    Scene scene;

    GameObject water;
    BoxCollider2D waterCollider;

    [HideInInspector]
    public PanoramaBackground.PatternSpecs patternSpecs;

    public class OffscreenEvent : UnityEvent<GameObject> { }
    public OffscreenEvent offscreenEvent { get; private set; } = new OffscreenEvent();
    //[HideInInspector]
    public bool panActive = false;

    public void Init()
    {
        overlay.sprite = patternSpecs.overlay;
        color.sprite = patternSpecs.colour;
        underlay.sprite = patternSpecs.underlay;

        // move offscreen initially
        transform.localPosition = new Vector3(-20f, 0, 0);
    }

    private void Awake()
    {
        offscreenEvent.AddListener((GameObject g) =>
        {
            panActive = false;
        });

        scene = SceneManager.GetActiveScene();

        if(scene.name == "Game")
        {
            water = GameObject.Find("Water");
            waterCollider = water.GetComponent<BoxCollider2D>();
        }
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane)); //Coords of top right corner of screen


        transform.localScale = new Vector3(topRight.x * 2.01f, topRight.x * 2.01f, transform.localScale.z);
        transform.position = Vector3.zero;
        bounds = GetComponent<BoxCollider2D>().bounds;
    }
    private void Update()
    {
        if (panActive)
        {
            bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane)); //coords of bottom left corner of screen

            if (scene.name == "Main Menu")
            {
                if (transform.position.y + bounds.extents.y < bottomLeft.y)
                {
                    //gameObject.tag = "BackgroundInvisible";
                    offscreenEvent.Invoke(gameObject);

                }
            }
            if (scene.name == "Game")
            {

                if (transform.position.y + bounds.extents.y < water.transform.position.y - waterCollider.bounds.extents.y - 0.2f)
                {
                    //gameObject.tag = "BackgroundInvisible";
                    offscreenEvent.Invoke(gameObject);
                }
            }
        }
    }
}
