using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

public class Beam : MonoBehaviour {

	public Transform playerTransform;
	public LineRenderer linerenderer;
	public GameObject projectile;
	public GameData gameData;
	public SpawnFlyingEnemy spawnFEScript;
	private Movement movementScript;

	public AudioSource gunFire;


	//Beam Variables
	public System.Random rnd = new System.Random();
	
	public float beamCountdown;
	[SerializeField] public LayerMask layerMask;

	[Header("Beam Variables")]
	public bool attack;
	public float beamTime = 5f;
	public float capBeamAtScore;
	public float maxBeamTime;
	public float minBeamTime;
	public int score;
	public float shotAmount_ScoreAmplifier;

	[Header("Recoil Anim")]
	public Animator animator;

	[Header("Other Variables")]
	public float autoKillDelay;
	//Projectile Variables


	void Start ()
	{
		linerenderer.useWorldSpace = true;
		gameData = GameObject.Find("GameController").GetComponent<GameData>();
		spawnFEScript = GameObject.Find("Player").GetComponent<SpawnFlyingEnemy>();
		movementScript = gameObject.transform.root.GetComponent<Movement>();
		PreDetermineShots();
		RampBeamTime();

		shot = 1;
		attack = false;
	}
	void OnBecameVisible()
	{
		linerenderer.enabled = true;
		attack = true;
		beamCountdown = beamTime;
	}

	// Update is called once per frame
	void Update()
	{
		RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, Mathf.Infinity, layerMask);

		//Debug.DrawRay(transform.position, transform.right, Color.red);
		//print(hit.transform.gameObject.name);

		linerenderer.SetPosition(0, transform.position - new Vector3(0f, 0f, 0f));
		linerenderer.SetPosition(1, hit.point);

		ReadyToFire();
		if(attack == true)
		{
			PrepareToFire();
		}
	}

	public void RampBeamTime()
	{
		if(score > 0)
		{
			score = gameData.score;
			float percentage = (capBeamAtScore - score) / capBeamAtScore;

			beamTime = Mathf.Max(minBeamTime, maxBeamTime * percentage);
		}
		else
		{
			beamTime = maxBeamTime;
		}
	}

	int shot = 1;
	float nextShot;
	public float delay = 0.5f;
	bool readyToFire = true;
	public int shots = 3;

	public void PreDetermineShots()
	{
		int lowerBound = Mathf.Min(2000 + (score * Mathf.RoundToInt(shotAmount_ScoreAmplifier)), 5000);
		int ranInt = rnd.Next(lowerBound, 5000);
		shots = Mathf.RoundToInt(ranInt / 1000);
	}

	private void PrepareToFire()
	{
		if (attack == true)
		{
			beamCountdown -= Time.deltaTime;

			TurnToRed();
			LeadToPlayer();

			if (beamCountdown <= 0)
			{
				linerenderer.enabled = false;

				if(shot <= shots && readyToFire)
				{
					FireProjectile();
					nextShot = Time.time + delay;
					shot++;
				}
				if(shot > shots)
				{
					attack = false;
					Invoke("Die", autoKillDelay);
				}
			}
		}
	}
	
	void ReadyToFire()
	{
		if(Time.time >= nextShot)
		{
			readyToFire = true;
		}
		else
		{
			readyToFire = false;
		}
	}

	Color colour = new Color(1, 1, 1, 0);
	void TurnToRed()
	{
		float lp = 0.3f / (beamTime / 3f);

		colour.a += lp * Time.deltaTime;
		colour -= new Color(0, lp * Time.deltaTime, lp * Time.deltaTime, 0);
		colour.g = Mathf.Max(colour.g, 0); colour.b = Mathf.Max(colour.b, 0); colour.a = Mathf.Min(colour.a, 1);

		linerenderer.startColor = colour;
		linerenderer.endColor = colour;
	
	}

	float offsetX = 0f;
	public float moveSpeed = 2f;
	void LeadToPlayer()
	{
		if(linerenderer.enabled == true)
		{
			linerenderer.material.mainTextureOffset = new Vector2(offsetX -= moveSpeed * Time.deltaTime, 0f);
		}
	}

	void FireProjectile()
	{
		Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y, 0f);
		GameObject spawnedProjectile = Instantiate(projectile, spawnPos, transform.rotation, null);

		spawnedProjectile.GetComponent<Projectile>().gamedata = gameData;

		animator.Play("Recoil");

        if (gameData.soundsOn)
        {
			gunFire.Play();
		}
		
	}

	public void Die()
	{
		spawnFEScript.flyingEnemySpawned = false;
		gameData.flyingEnemiesKilled++;

		movementScript.hovering = false;
		movementScript.FadeHumOut();
		
		spawnFEScript.ControlSpawnRate();
	}

}
