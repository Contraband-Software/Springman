using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScaleToScreen : MonoBehaviour
{
    Camera cam;

    Vector3 topRight;
    Vector3 bottomLeft;

    Bounds bounds;

    Scene scene;

    GameObject water;
    BoxCollider2D waterCollider;

    private void Awake()
    {
        scene = SceneManager.GetActiveScene();

        if(scene.name == "Game")
        {
            water = GameObject.Find("Water");
            waterCollider = water.GetComponent<BoxCollider2D>();
        }

        bounds = GetComponent<BoxCollider2D>().bounds;
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane)); //Coords of top right corner of screen


        transform.localScale = new Vector3(topRight.x * 2.01f, topRight.x * 2.01f, transform.localScale.z);
        transform.position = Vector3.zero;
    }
    private void Update()
    {
        bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane)); //coords of bottom left corner of screen

        if (scene.name == "Main Menu")
        {
            if (transform.position.y + bounds.extents.y < bottomLeft.y)
            {
                gameObject.SetActive(false);
            }
        }
        if(scene.name == "Game")
        {

            if(transform.position.y + bounds.extents.y < water.transform.position.y - waterCollider.bounds.extents.y - 0.2f)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
