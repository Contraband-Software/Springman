using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public Transform target;
    public PlayerController playerController;
    public GameObject water;
    public BoxCollider2D waterCollider;
    public Camera cam;
    public float topOfWater;
    public float smoothSpeedUp = 0.1f;
    public float smoothSpeedDown = 0.125f;
    public Vector3 offset;
    public Vector3 bottomLeft;

    public bool followPlayer;

    //target variables
    public Vector3 desiredPosition;
    public Vector3 camFallPosition;

    public bool targetFound = false;
    public List<float> yPositions = new List<float>();
    public float lowestOnScreen;
    
    //camera movement Variables
    public float jumpByAmount = 3f;
    public float cameraNegJump = 5f;
    public enum CamDirection { Up, Down, Neutral}
    public CamDirection camDirection;

    private void Start()
    {
        followPlayer = true;
        cam = gameObject.GetComponent<Camera>();
        water = GameObject.Find("Water");
        waterCollider = water.GetComponent<BoxCollider2D>();

        playerController.revive_Reassign += ReassignPCon;
    }

    private void ReassignPCon(PlayerController pCon)
    {
        playerController = pCon;
        target = playerController.gameObject.transform;
    }

    void FixedUpdate()
    {
        if (targetFound == true)
        {
            if(playerController.state == PlayerController.State.Dead && playerController.deathBy != PlayerController.DeathBy.Water )
            {
                desiredPosition = transform.position;
            }
            //Camera falls back down if player goes underneath lowest platform
            if (camDirection == CamDirection.Down)
            {
                if(!(playerController.state == PlayerController.State.Dead) && playerController.deathBy == PlayerController.DeathBy.Water)
                {
                    desiredPosition = camFallPosition;
                }
                Vector3 smoothPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeedDown);
                transform.position = smoothPosition;
            }
            else if(camDirection == CamDirection.Up)
            {
                Vector3 smoothPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeedUp);
                transform.position = smoothPosition;
            }
        }
    }
    void Update()
    {
        topOfWater = water.GetComponent<WaterRise>().topOfCollider;
        bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane)); //coords of bottom left corner of screen

        FoundDesiredPos();
        FindLowestPlatform();
        CameraFall();
    }

    private void FindLowestPlatform()
    {
        if (yPositions.Count() > 0)
        {
            lowestOnScreen = yPositions.Min();
        }
    }

    private void FoundDesiredPos()
    {
        if(playerController.state == PlayerController.State.Dead)
        {
            if(playerController.deathBy == PlayerController.DeathBy.Water)
            {
                if (camFallPosition.y == 0.65f)
                {
                    desiredPosition = new Vector3(transform.position.x, 0.65f, transform.position.z);
                }
                else
                {
                    if (transform.position.y > topOfWater + 2f) //this is impossoble - maybe change in future
                    {
                        desiredPosition = new Vector3(transform.position.x, topOfWater + 2f, transform.position.z);
                    }
                }
            }
            else
            {
                if(playerController.deathBy == PlayerController.DeathBy.Mine)
                {
                    desiredPosition = transform.position;
                }
            }
            camDirection = CamDirection.Down;
        }

        if (camDirection == CamDirection.Down && camFallPosition.y != 0.65f && water.transform.position.y + waterCollider.bounds.extents.y > bottomLeft.y)
        {
            desiredPosition = new Vector3(transform.position.x, topOfWater + 2f, transform.position.z);
        }

        //If grounded, will calculate next position camera will move to + other things
        if (playerController.isGrounded == true)
        {
            //if it hits ground platform, camera returns to start location
            if(playerController.rayCastHit.collider.name == "Ground Platform" && playerController.state != PlayerController.State.Dead)
            {
                camDirection = CamDirection.Up;
                desiredPosition = new Vector3(transform.position.x, 0.65f, transform.position.z);
            }
            //if it hits anything else(platforms), it sets the desiredPosition based off of the location of that platfrom
            else if(playerController.rayCastHit.collider.tag == "Platform" && playerController.state != PlayerController.State.Dead)
            {
                camDirection = CamDirection.Up;
                desiredPosition = new Vector3(transform.position.x, playerController.rayCastHit.transform.position.y + jumpByAmount,
               transform.position.z);
            }
            targetFound = true;
        }

        /*
        //if player dies, focus on player
        if(playerController.state == PlayerController.State.Dead)
        {
            camDirection = CamDirection.Up;
            desiredPosition = new Vector3(transform.position.x, playerController.transform.position.y, transform.position.z);
        }
        */
    }
    void CameraFall()
    {
        //Locating the next potential position the camera will fall to
        if(targetFound == true)
        {
            if (transform.position.y - cameraNegJump <= 0.65f && playerController.state != PlayerController.State.Dead)
            {
                camFallPosition = new Vector3(transform.position.x, 0.65f, transform.position.z);
            }
            else
            {
                if(playerController.state != PlayerController.State.Dead)
                {
                    camFallPosition = new Vector3(transform.position.x, transform.position.y - cameraNegJump, transform.position.z);
                }
                //camFallPosition = new Vector3(transform.position.x, transform.position.y - cameraNegJump, transform.position.z);
                //print("not 0.65 target");
                /*
                if (water.transform.position.y + waterCollider.bounds.extents.y < bottomLeft.y)
                {
                    print("water lower than bottom of screen");
                    camFallPosition = new Vector3(transform.position.x, transform.position.y - cameraNegJump, transform.position.z);
                }
                */
            }
        }
        //Detecting whether to set the camera into fall mode  //change all localscale values to use bounds
        if(topOfWater > lowestOnScreen)
        {
            if (playerController.transform.position.y - playerController.transform.localScale.y / 2f < topOfWater && camDirection != CamDirection.Down && playerController.state != PlayerController.State.Alive)
            {
                camDirection = CamDirection.Down;
            }
        }
        else if(playerController.transform.position.y - playerController.transform.localScale.y / 2f < lowestOnScreen && camDirection != CamDirection.Down && playerController.state == PlayerController.State.Alive)
        {
            camDirection = CamDirection.Down;
        }

        //If the camera has reached the fall position, the direction is reset
        if(camDirection == CamDirection.Down && transform.position.y <= camFallPosition.y)
        {
            camDirection = CamDirection.Up;
        }
    }
}
