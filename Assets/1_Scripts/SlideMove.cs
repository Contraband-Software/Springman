using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class SlideMove : MonoBehaviour {

	public GameData gameData;

	public Rigidbody2D rb;
	public GameObject player;
	public PlayerController pController;
	private CameraFollow cameraFollow;
	public Transform waterTransform;
	public GameObject mainCamera;
	public Camera cam;

	public SpriteRenderer indicatorSprite;

	//size variables
	public float halfPlatHeight;
	public float platDistance = 3f;

	//movement variables
	public float deltaX;

	//touch variables
	public bool moving = false;

	//coins variables
	[Header("Screws Stuff")]
	public GameObject silverScrew;
	public GameObject goldScrew;

	private Bounds goldScrewBounds;
	private Bounds silverScrewBounds;

	public float silverChance = 30f;
	public float goldChance = 10f;

	private bool screwSpawned = false;

	//SittingEnemy Variables
	[Header("Sitting Enemy")]
	public float chanceToSpawnSE;
	public GameObject sittingEnemyPrefab;
	public bool sittingEnemySpawned;
	float sittingEnemyHeight;

	public float maxChance;
	public float minChance;
	public float capChanceAtScore;

	[Header("other")]

	////FlyingEnemy Variables
	//public int chanceToSpawnFE = 100;
	//public GameObject flyingEnemyPrefab;
	//public bool flyingEnemySpawned;
	//public float offset = 0.6f;

	//Randomization variables
	public System.Random rnd = new System.Random();

	public Vector3 topRight;
	public Vector3 bottomLeft;

	public float thisPlatLength;

	void Awake()
	{
		//rb = GetComponent<Rigidbody2D>();
		//gameData = GameObject.Find("GameController").GetComponent<GameData>();
		halfPlatHeight = transform.Find("PlatformMID").GetComponent<BoxCollider2D>().bounds.extents.y;
		//mainCamera = GameObject.Find("Main Camera");
		//cam = mainCamera.GetComponent<Camera>();
		sittingEnemyHeight = sittingEnemyPrefab.GetComponent<Renderer>().bounds.extents.y * 2;
		waterTransform = GameObject.Find("Water").GetComponent<Transform>();

		goldScrewBounds = goldScrew.transform.Find("ScrewObjects/Screw").GetComponent<Renderer>().bounds;
		silverScrewBounds = silverScrew.transform.Find("Screw").GetComponent<Renderer>().bounds;

		sittingEnemySpawned = false;

		//flyingEnemySpawned = false;
}
	void Start()
	{
		cameraFollow = mainCamera.GetComponent<CameraFollow>();
		topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane)); //Coords of top right corner of screen
		bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane)); //coords of bottom left corner of screen

		RampSESpawnChance();
		Visibility();
		PotentialSittingEnemySpawn();

		if(gameData.tutorialComplete == false && transform.position.y == 1.5f)
		{
			gameObject.name = "DemoPlatform";
		}
		if(gameData.tutorialComplete == false && transform.position.y == -1.5f)
		{
			gameObject.name = "LowestPlatform";
		}

		pController.revive_Reassign += ReassignPCon;
	}

	public void ReassignPCon(PlayerController pCon)
	{
		pController = pCon;
		player = pController.gameObject;
		pController.revive_Reassign += ReassignPCon;
	}
	// Update is called once per frame
	void Update()
	{
		ClampToScreen();
		topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane)); //Coords of top right corner of screen
		bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane)); //coords of bottom left corner of screen

		Visibility();
		WhenToDestroy();
		//PotentialEnemySpawn

		if (!gameData.Paused && pController.state == PlayerController.State.Alive && gameData.allowSlideMove == true)
		{
			if (Input.touchCount > 0)
			{
				Touch touch = Input.GetTouch(0);
				Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);

				if (moving == false)
				{
					if (touchPosition.y > ((transform.position.y + halfPlatHeight) - (platDistance - (halfPlatHeight * 2))) && touchPosition.y < transform.position.y + halfPlatHeight)
					{
						Move(touch, touchPosition);
					}
				}
				else
				{
					Move(touch, touchPosition);
				}
			}
		}
	}

	private void Move(Touch touch, Vector3 touchPosition)
	{
		switch (touch.phase)
		{
			case TouchPhase.Began:
				deltaX = touchPosition.x - transform.position.x;
				moving = true;
				break;
			case TouchPhase.Moved:
				if (moving == true)
				{
					if(Time.timeScale == 1f)
					{
						rb.MovePosition(new Vector3(touchPosition.x - deltaX, transform.position.y, transform.position.z));
					}
					else
					{
						float deltaLastX = touchPosition.x - transform.position.x;
						transform.Translate(new Vector3(deltaLastX - deltaX, 0f, 0f));
					}
				}
				break;
			case TouchPhase.Ended:
				rb.velocity = Vector3.zero;
				moving = false;
				break;
		}
	}
	public void MoveByDemo(float deltaX, Vector3 touchPosition)
	{
		if (moving == true)
		{
			//rb.MovePosition(new Vector3(touchPosition.x - deltaX, transform.position.y, transform.position.z));
			float deltaLastX = touchPosition.x - transform.position.x;
			transform.Translate(new Vector3(deltaLastX - deltaX, 0f, 0f));
		}
	}
	public void MoveByDemoEnd()
	{
		moving = false;
	}

	void Visibility()
	{
		if (sittingEnemySpawned)
		{
			if (transform.position.y - halfPlatHeight < topRight.y && transform.position.y + halfPlatHeight + sittingEnemyHeight > bottomLeft.y)
			{
				if (!cameraFollow.yPositions.Contains(transform.position.y))
				{
					cameraFollow.yPositions.Add(transform.position.y);
				}
			}
			else
			{
				if (cameraFollow.yPositions.Contains(transform.position.y))
				{
					cameraFollow.yPositions.Remove(transform.position.y);
				}
			}
		}
		else
		{
			if (transform.position.y - halfPlatHeight < topRight.y && transform.position.y + halfPlatHeight > bottomLeft.y)
			{
				if (!cameraFollow.yPositions.Contains(transform.position.y))
				{
					cameraFollow.yPositions.Add(transform.position.y);
				}
			}
			else
			{
				if (cameraFollow.yPositions.Contains(transform.position.y))
				{
					cameraFollow.yPositions.Remove(transform.position.y);
				}
			}
		}
	}

	public void RampSESpawnChance()
	{
		float score = gameData.score;
		if (score > 0)
		{
			float percentage = score / capChanceAtScore;
			//print("percentage = " + percentage.ToString());

			float chanceToSpawn = minChance + ((maxChance - minChance) * percentage);
			chanceToSpawnSE = Mathf.Min(maxChance, chanceToSpawn);

#if UNITY_EDITOR
			if (GameDebugController.instance.GetSEOnAllPlatforms()) {
				chanceToSpawnSE = 100f;
			}
#endif
		}
		else
		{
			chanceToSpawnSE = minChance;
		}
	}

	public void WhenToDestroy()
	{
		if(transform.position.y < waterTransform.transform.position.y)
		{
			GameObject g = gameObject;
			for (var i = g.transform.childCount - 1; i >= 0; i--)
			{
				Destroy(g.transform.GetChild(i).gameObject);
			}
			Destroy(gameObject);
		}
	}

	void OnDestroy()
	{
		cameraFollow.yPositions.Remove(transform.position.y);
	}
	//public void PotentialFlyingEnemySpawn()
	//{
	//	//Make them spawn only if they are going to be above the top most platfrom
	//	if(cameraFollow.yPositions.Count > 0)
	//	{
	//		Vector3 feBounds = flyingEnemyPrefab.GetComponent<BoxCollider2D>().bounds.extents;

	//		float spawnPosX = rnd.Next(Convert.ToInt32(-330 + (Math.Round(feBounds.x, 2) * 100)), Convert.ToInt32(330 - (Math.Round(feBounds.x, 2) * 100))) / 100f;
	//		float spawnPosY = (transform.position.y + platDistance) - halfPlatHeight - offset;

	//		if (CanSpawnFlyingEnemy() && flyingEnemySpawned == false)
	//		{
	//			Vector3 spawnPos = new Vector3(spawnPosX, spawnPosY, 0f);
	//			GameObject flyingEnemy = Instantiate(flyingEnemyPrefab, new Vector3(0f, 0f, 0f), new Quaternion(0f, 0f, 0f, 0f));

	//			flyingEnemy.transform.position = spawnPos;

	//			MaterializeEnemy(flyingEnemy);
	//			flyingEnemySpawned = true;
	//		}


	//	}
	//}

	public void PotentialSittingEnemySpawn()
	{
		if(ChanceRoll(chanceToSpawnSE) && sittingEnemySpawned == false && thisPlatLength >= 1f && gameData.score > 3f)
		{
			Vector3 offset = new Vector3(transform.position.x, transform.position.y + halfPlatHeight + (sittingEnemyPrefab.GetComponent<Renderer>().bounds.size.y / 2), transform.position.z);
			GameObject sittingEnemy = Instantiate(sittingEnemyPrefab, new Vector3(0f, 0f, 0f), new Quaternion(0f, 0f, 0f, 0f));

			sittingEnemy.transform.position = offset;
			sittingEnemy.transform.SetParent(gameObject.transform);
			ActiveOnProxy aop = sittingEnemy.GetComponent<ActiveOnProxy>();
			aop.player = player;
			aop.pController = pController;
			aop.gamedata = gameData;
			sittingEnemySpawned = true;

			//FLASH INDICATOR RED
			//print("SPAWNED SITTING");
			gameData.enemiesActive.Add(sittingEnemy);
			indicatorSprite = gameObject.transform.GetChild(3).GetComponent<SpriteRenderer>();
			FlashRed();
		}
		else
		{
			if (ChanceRoll(goldChance) && thisPlatLength >= 0.7f)
			{
				Vector3 offset = new Vector3(transform.position.x, transform.position.y + halfPlatHeight + (goldScrewBounds.extents.y), transform.position.z);
				GameObject goldScrewSpawned = Instantiate(goldScrew, new Vector3(0f, 0f, 0f), new Quaternion(0f, 0f, 0f, 0f));

				goldScrewSpawned.transform.position = offset;
				goldScrewSpawned.transform.SetParent(gameObject.transform);

				goldScrewSpawned.transform.localPosition = new Vector3(placeRandomly(goldScrewSpawned), goldScrewSpawned.transform.localPosition.y,
					goldScrewSpawned.transform.localPosition.z);

				screwSpawned = true;
			}
			if(ChanceRoll(silverChance) && !screwSpawned && thisPlatLength >= 0.7f)
			{
				Vector3 offset = new Vector3(transform.position.x, transform.position.y + halfPlatHeight + (silverScrewBounds.extents.y), transform.position.z);
				GameObject silverScrewSpawned = Instantiate(silverScrew, new Vector3(0f, 0f, 0f), new Quaternion(0f, 0f, 0f, 0f));

				silverScrewSpawned.transform.position = offset;
				silverScrewSpawned.transform.SetParent(gameObject.transform);

				silverScrewSpawned.transform.localPosition = new Vector3(placeRandomly(silverScrewSpawned), silverScrewSpawned.transform.localPosition.y,
					silverScrewSpawned.transform.localPosition.z);
				//change position of the screw in local scope

				screwSpawned = true;
			}
		}
	}
	public float placeRandomly(GameObject screw)
	{
		float xExtent;

		if (screw.transform.GetChild(0).transform.childCount > 0) //for gold screw
		{
			xExtent = screw.transform.Find("ScrewObjects/Screw").GetComponent<BoxCollider2D>().bounds.extents.x;
		}
		else
		{
			xExtent = screw.transform.Find("Screw").GetComponent<BoxCollider2D>().bounds.extents.x;
		}

		double leftMostX = (0 - (thisPlatLength / 2)) + (xExtent - 0.00005 + 0.08f);
		double rightMostX = (0 + (thisPlatLength / 2) - (xExtent - 0.00005) - 0.08f);

		leftMostX = Math.Round(leftMostX, 4);
		rightMostX = Math.Round(rightMostX, 4);

		//print("LM : " + leftMostX.ToString());
		//print("RM : " + rightMostX.ToString());

		string leftMostXString = leftMostX.ToString();
		string[] splitLX;
		int multiplier = 1;

		if (leftMostXString.Contains('.'))
		{
			splitLX = leftMostXString.Split('.');

			int decimals = splitLX[1].Length;
			multiplier = (int)Mathf.Pow(10, decimals);
		}

		int randX = rnd.Next((int)(leftMostX * multiplier), (int)((rightMostX * multiplier) + 1));

		float finalXPos = (float)randX / (float)multiplier;

		return finalXPos;
	}

	public bool ChanceRoll(float successChance)
	{
		string successChanceString = successChance.ToString();
		string[] splitString;
		int multiplier = 1;
		if (successChanceString.Contains('.'))
		{
			splitString = successChanceString.Split('.');
			int decimals = splitString[1].Length;
			multiplier = (int)Mathf.Pow(10, decimals);
		}

		int randNum = rnd.Next(1, (100 * multiplier) + 1);
		if(randNum <= (successChance * multiplier))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public int flashesLeft = 3;
	public float flashTime = 0.01f;
	public void FlashRed()
	{
		if(flashesLeft > 0)
        {
			flashesLeft--;
			indicatorSprite.color = new Color(1f, 0f, 0.17f);
			LeanTween.delayedCall(flashTime, FlashRedGoWhite).setIgnoreTimeScale(true);
        }
    }
	public void FlashRedGoWhite()
    {
		indicatorSprite.color = Color.white;
		LeanTween.delayedCall(flashTime, FlashRed).setIgnoreTimeScale(true);
    }

	private void ClampToScreen()
	{
		float trueSize = thisPlatLength - 0.02f;

		if (pController.transform.position.x > pController.zeroToEdge - pController.bounds.x - 0.1f ||
					pController.transform.position.x < -pController.zeroToEdge + pController.bounds.x + 0.1f) //player near the walls
		{
			if (pController.transform.position.y - pController.bounds.y < transform.position.y + halfPlatHeight - 0.005f &&
				pController.transform.position.y + pController.bounds.y > transform.position.y - halfPlatHeight + 0.005f)
			{
				if (pController.transform.position.x < 0 && transform.position.x > pController.transform.position.x) //hugging left wall and within airspace
				{
					transform.position = new Vector3(Mathf.Clamp(transform.position.x, bottomLeft.x + (pController.bounds.x * 2) + (trueSize / 2),
						topRight.x + (trueSize / 2) + 0.05f), transform.position.y, transform.position.z);
				}
				if (pController.transform.position.x > 0 && transform.position.x < pController.transform.position.x) //hugging right wall and within airspace
				{
					transform.position = new Vector3(Mathf.Clamp(transform.position.x, bottomLeft.x - (trueSize / 2) -0.05f,
						topRight.x - (pController.bounds.x * 2) - (trueSize / 2)), transform.position.y, transform.position.z);
				}
				else
				{
					transform.position = new Vector3(Mathf.Clamp(transform.position.x, bottomLeft.x - (trueSize / 2) - 0.05f, topRight.x + (trueSize / 2) + 0.05f), transform.position.y, transform.position.z);
				}
			}
			else
			{
				transform.position = new Vector3(Mathf.Clamp(transform.position.x, bottomLeft.x - (trueSize / 2) - 0.05f, topRight.x + (trueSize / 2) + 0.05f), transform.position.y, transform.position.z);
			}
		}
		else
		{
			transform.position = new Vector3(Mathf.Clamp(transform.position.x, bottomLeft.x - (trueSize / 2) - 0.05f, topRight.x + (trueSize / 2) +0.05f), transform.position.y, transform.position.z);
		}
	}
	//public bool CanSpawnFlyingEnemy()
	//{
	//	int randNum = rnd.Next(1, 101);
	//	if (randNum <= chanceToSpawnFE)
	//	{
	//		//print("can spawn FE");
	//		return true;
	//	}
	//	else
	//	{
	//		//print("cant spawn FE");
	//		return false;
	//	}
	//}
}
