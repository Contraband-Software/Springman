using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Analytics;

public class CreatePlatforms : MonoBehaviour
{
	//GAMEDATA REFERENCE
	GameData gameData;

	//Camera Variables
	public Camera cam;
	public Vector3 topRight;
	public float zeroToEdge;

	//Object to Instantiate
	public GameObject platform;

	//Camera script to get yPositions from
	public CameraFollow camScript;

	//Platform variables
	public float halfPlatSize = 0.15f;
	float highestPlat = 0f;
	public float firstPlatPosY = -1.5f;
	Bounds bounds;

	//initial platform width >>>>> Make script to alter the number once score is implemented

	//Randomization variables
	public System.Random rnd = new System.Random();
	public float spawnEdge;

	//Distancing Variables
	public float platDistance = 3f;

	void Start()
	{
		gameData = GameObject.Find("GameController").GetComponent<GameData>();

		bounds = platform.transform.Find("PlatformMID").GetComponent<BoxCollider2D>().bounds;
		cam = GameObject.Find("Main Camera").GetComponent<Camera>();
		zeroToEdge = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane)).x;
		halfPlatSize = bounds.extents.y;

		CreateFirstPlat();
	}

	void Update()
	{
		CreatePlatform();
		//print(topRight);
	}

	private void CreatePlatform()
	{
		topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane)); //Coords of top right corner of screen

		CalculateHighestPlat();

		if (camScript.yPositions.Count() > 0)
		{
			if (highestPlat + platDistance - (halfPlatSize) < topRight.y) //checks if the "next" platform will come into view
			{
				if (camScript.yPositions.Contains(highestPlat + platDistance) == false) //this makes sure this plat position doesnt already exist
				{
					ChangePlatLength();
					if (gameData.tutorialComplete)
					{
						if (rnd.Next(0, 2) == 1)
						{
							//spawns left side
							spawnEdge = -zeroToEdge - (gameData.platLength / 2) - 0.05f;

						}
						else
						{
							//spawns right side
							spawnEdge = zeroToEdge + (gameData.platLength / 2) + 0.05f;
						}
					}
					else
					{
						if(camScript.yPositions.Count() == 1)
						{
							spawnEdge = spawnEdge = zeroToEdge + (gameData.platLength / 2) + 0.05f;
						}
						if(camScript.yPositions.Count() == 2)
						{
							spawnEdge = -zeroToEdge - (gameData.platLength / 2) - 0.05f;
						}
					}
					//platform = GameObject.FindWithTag("Platform");
					GameObject Platform = Instantiate(platform, new Vector3(spawnEdge, highestPlat + platDistance, 0f), new Quaternion(0f, 0f, 0f, 0f));
					Platform.GetComponent<SlideMove>().thisPlatLength = gameData.platLength;
				}
			}
		}
	}

	public void ChangePlatLength()
	{
		gameData.CalculateMinPlatLength();

		int maxPlat = (int)Math.Truncate(gameData.maxPlatLength * 1000);
		int minPlat = (int)Math.Truncate(gameData.minPlatLength * 1000);

		gameData.platLength = rnd.Next(minPlat, maxPlat) / 1000f;
	}

	public void CalculateHighestPlat()
	{
		if(camScript.yPositions.Count() > 0)
		{
			if (camScript.yPositions.Max() > highestPlat)
			{
				highestPlat = camScript.yPositions.Max();
			}
		}
	}
	void CreateFirstPlat()
	{
		ChangePlatLength();

		GameObject Platform = Instantiate(platform, new Vector3(-zeroToEdge - (gameData.platLength / 2) -0.05f, firstPlatPosY, 0f), new Quaternion(0f, 0f, 0f, 0f));
		Platform.GetComponent<SlideMove>().thisPlatLength = gameData.platLength;
		highestPlat = firstPlatPosY;
	}
}
