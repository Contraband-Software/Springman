using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Movement : MonoBehaviour {


	//Camera Variables
	public Camera cam;
	public CameraFollow camScript;
	float zeroToEdge;

	//sound
	public GameObject explosion_sound_emitter;
	public AudioSource hoverSound;
	private float preHoverVolume;
	LeanTweenType humFadeInEase;
	LeanTweenType humFadeOutEase;

	public Vector3 topRight;
	Vector3 febounds;
	public float offsetFromTop = 0.5f;

	//Movement Variables
	public float lerpSpeed = 0.2f;
	public float horizontalSpeed = 2f;
	float maxHorizontalSpeed;
	public float accelRate = 0.5f;
	public float decelRate = 0.5f;
	float target;
	public float sideOffset = 0.1f;

	public enum Direction {Left, Right};
	public Direction direction;

	//Randomization
	public System.Random rnd = new System.Random();

	[Header("Hovering or going away")]
	public bool hovering = true;

	[Header("GameData ref")]
	public Architecture.Managers.GamePlay gameData;

	// Use this for initialization
	void Start ()
	{
		gameData = Architecture.Managers.GamePlay.GetReference();

        hoverSound.volume = 0f;

        if (Architecture.Managers.UserGameData.Instance.soundsOn)
        {
			hoverSound.Play();
		}
		
		FadeHumIn();

		febounds = gameObject.GetComponent<Renderer>().bounds.extents;
		cam = GameObject.Find("Main Camera").GetComponent<Camera>();
		camScript = cam.GetComponent<CameraFollow>();
		zeroToEdge = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane)).x;

		

		maxHorizontalSpeed = horizontalSpeed;
		hovering = true;

		InitialDirection();

	}

	private void InitialDirection()
	{
		if (rnd.Next(0, 2) == 1)
		{
			direction = Direction.Left;
		}
		else
		{
			direction = Direction.Right;
		}
	}

	private void FadeHumIn()
    {
		LeanTween.value(gameObject, ChangeVol,  0f, 0.5f, 0.3f).setEase(humFadeInEase);
    }
	public void ChangeVol(float vol)
    {
		hoverSound.volume = vol;
    }

	public void FadeHumOut()
    {
		//LeanTween.value(gameObject, ChangeVol, hoverSound.volume, 0f, 0.3f).setEase(humFadeOutEase);
	}

	// Update is called once per frame
	void Update () 
	{
		if (Architecture.Managers.UserGameData.Instance.soundsOn == false)
        {
			hoverSound.volume = 0f;
        }
        else
        {
			preHoverVolume = hoverSound.volume;
			hoverSound.volume = preHoverVolume;
        }
		AdjustSpeed();
	}
	float ascAccelRate = 0f;
	void FixedUpdate()
	{
		topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane)); //Coords of top right corner of screen
		if(hovering)
		{
			TravelInDirection();

			float smoothYPos = Mathf.Lerp(transform.position.y, topRight.y - febounds.y - offsetFromTop - PointOnCurve(), lerpSpeed);
			transform.position = new Vector3(transform.position.x, smoothYPos, transform.position.z);
		}
		else
		{
			TravelInDirection();

			ascAccelRate += 0.8f;
			float smoothYPos = transform.position.y + (ascAccelRate * Time.deltaTime);
			transform.position = new Vector3(transform.position.x, smoothYPos, transform.position.z);
		}

		if (!hovering)
		{
			if(transform.position.y > topRight.y + febounds.y + 20f)
			{
				Destroy(gameObject);
			}
		}
	}

	float PointOnCurve()
	{
		float x = transform.position.x;
		float y = (x * x) * 0.1f;
		return y;
	}

	void TravelInDirection()
	{
		if(direction == Direction.Left)
		{
			target = -zeroToEdge + febounds.x + sideOffset;
			transform.Translate(-(horizontalSpeed * Time.deltaTime), 0f, 0f);

			if (transform.position.x <= target)
			{
				direction = Direction.Right;
			}
		}
		else
		{
			target = zeroToEdge - febounds.x - sideOffset;
			transform.Translate(horizontalSpeed * Time.deltaTime, 0f, 0f);

			if(transform.position.x >= target)
			{
				direction = Direction.Left;
			}
		}
	}
	void AdjustSpeed()
	{
		if (direction == Direction.Left)
		{
			if(transform.position.x < -(zeroToEdge - 2.291118f))
			{
				horizontalSpeed -= decelRate * Time.deltaTime;
				horizontalSpeed = Mathf.Max(horizontalSpeed, 0);
			}
			if(transform.position.x > 0)
			{
				horizontalSpeed += accelRate * Time.deltaTime;
				horizontalSpeed = Mathf.Min(horizontalSpeed, maxHorizontalSpeed);
			}
		}
		else
		{
			if(transform.position.x > (zeroToEdge - 2.291118f)) //past middle of screen a bit, now decelerating towards 3.3
			{
				horizontalSpeed -= decelRate * Time.deltaTime;
				horizontalSpeed = Mathf.Max(horizontalSpeed, 0f); //clamps speed to not go below 0
			}
			if(transform.position.x < 0) //accelerating from the point -3.3 to 0
			{
				horizontalSpeed += accelRate * Time.deltaTime;
				horizontalSpeed = Mathf.Min(horizontalSpeed, maxHorizontalSpeed); //clamps speed to not go above max speed
			}
		}

		if(transform.position.x == 0)
		{
			horizontalSpeed = maxHorizontalSpeed;
		}
	}
}
