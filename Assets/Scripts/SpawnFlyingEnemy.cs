using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFlyingEnemy : MonoBehaviour {

	public CameraFollow camScript;
	public GameData gameData;
	public Camera cam;
	public PlayerController pController;
	float zeroToEdge;

	//Camera Variables
	public Vector3 topRight;


	[Header("Flying Enemy Varibales")]//FlyingEnemy Variables

	public int chanceToSpawnFE = 100;
	public GameObject flyingEnemyPrefab;
	public bool flyingEnemySpawned;
	public float startOffset = 1f;
	public float lastSpawnTime;
	public float prevPeak = 0;

	//Randomization variables
	public System.Random rnd = new System.Random();
	public float timeUntilNextSpawn = 5f;
	public float nextSpawn;

	// Use this for initialization
	void Start () 
	{
		gameData = GameObject.Find("GameController").GetComponent<GameData>();
		pController = gameObject.GetComponent<PlayerController>();
		cam = GameObject.Find("Main Camera").GetComponent<Camera>();
		camScript = cam.GetComponent<CameraFollow>();
		flyingEnemySpawned = false;
		zeroToEdge = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane)).x;

		prevPeak = 0;
		lastSpawnTime = Time.time;
		timeUntilNextSpawn = gameData.highestSpawnTime;
		nextSpawn = Time.time + timeUntilNextSpawn; //Initialises the first spawn time
	}
	
	// Update is called once per frame
	void Update () 
	{
		SpawnCountdown();
		PotentialFlyingEnemySpawn();

		topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane)); //Coords of top right corner of screen
	}

	public void PotentialFlyingEnemySpawn()
	{
		//Make them spawn only if they are going to be above the top most platfrom
		if (camScript.yPositions.Count > 0 && pController.state != PlayerController.State.Dead && gameData.tutorialComplete == true)
		{
			if (CanSpawnFlyingEnemy() && flyingEnemySpawned == false)
			{
				GameObject flyingEnemy = Instantiate(flyingEnemyPrefab, new Vector3(0f, topRight.y + 5f, 0f), new Quaternion(0f, 0f, 0f, 0f));

				Vector3 feBounds = flyingEnemy.gameObject.transform.root.GetComponent<BoxCollider2D>().bounds.extents;

				float spawnPosX = rnd.Next(Convert.ToInt32(((-(Math.Round(zeroToEdge, 2)) + (Math.Round(feBounds.x, 2))) * 100)),
					Convert.ToInt32(((Math.Round(zeroToEdge, 2) - (Math.Round(feBounds.x, 2))) * 100))) / 100f;

				float spawnPosY = (topRight.y + feBounds.y + startOffset);
				Vector3 spawnPos = new Vector3(spawnPosX, spawnPosY, 0f);

				flyingEnemy.transform.position = spawnPos;

				lastSpawnTime = Time.time;
				flyingEnemySpawned = true;

				gameData.enemiesActive.Add(flyingEnemy);
			}


		}
	}

	public bool CanSpawnFlyingEnemy()
	{
		if (SpawnCountdown())
		{
			int randNum = rnd.Next(1, 5000);
			if (randNum <= chanceToSpawnFE)
			{
				//print("can spawn FE");
				return true;
			}
			else
			{
				//print("cant spawn FE");
				return false;
			}
		}
		else
		{
			return false;
		}
	}
	public void ControlSpawnRate()
	{
		float hst = gameData.highestSpawnTime;               //Score here will be based off of Flying Enemies Killed rather than actual game score
		float lowest = gameData.lowestSpawnTime;
		float highCap = hst - lowest;
		int kills = gameData.flyingEnemiesKilled;
		float exaggeration = gameData.sinGraphExaggeration;

		float graphPos;
		float exagScore = kills * exaggeration;

		if (exagScore % 180 != 90 && exagScore % 180 != 0)
		{
			if(exagScore > 90)
			{
				int intDiv = (int)Math.Truncate(exagScore / 90);
				int remainder = intDiv % 2;
				if(remainder == 0) //even = add
				{
					graphPos = 0 + (exagScore % 90);
				}
				else //odd = subtract
				{
					graphPos = 90 - (exagScore % 90);
				}
			}
			else
			{
				graphPos = exagScore;
			}
		}
		else
		{
			graphPos = ((exagScore % 180) * highCap);
		}
		//print("GraphPos: " + graphPos.ToString());
		timeUntilNextSpawn = highCap - (highCap * Mathf.Sin(Mathf.Deg2Rad * graphPos)) + lowest;

		SetTimerForNextSpawn();
	}
	public void SetTimerForNextSpawn()
	{
		nextSpawn = Time.time + timeUntilNextSpawn;
		//print("next spawn in " + (nextSpawn - Time.time).ToString());
	}

	bool SpawnCountdown()
	{
		if(flyingEnemySpawned == false)
		{
			if(Time.time >= nextSpawn)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		else
		{
			return false;
		}
	}
}
