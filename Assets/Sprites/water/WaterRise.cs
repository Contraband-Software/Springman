using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WaterRise : MonoBehaviour {

	//Player Script
	public PlayerController pController;
	public BoxCollider2D boxCollider;
	public Camera cam;

	public Vector3 bottomLeft;

	//Speed Variables
	[Header("Water Rise variables")]
	public float baseSpeed = 1f;
	public float speed = 10f;
	public float topOfCollider;
	public float distanceToScreenBottom;
	public float allowedDistance = 4f;
	public float speedAmp = 0.5f;

	void Start () 
	{
		boxCollider = gameObject.GetComponent<BoxCollider2D>();
		cam = GameObject.Find("Main Camera").GetComponent<Camera>();
		bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane)); //coords of bottom left corner of screen
	}
	
	void Update () 
	{
		if(pController.state ==	PlayerController.State.Alive)
		{
			transform.Translate(0f, speed * Time.deltaTime, 0f);
		}
		bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane)); //coords of bottom left corner of screen

		topOfCollider = boxCollider.bounds.center.y + boxCollider.bounds.extents.y;
		distanceToScreenBottom = bottomLeft.y - topOfCollider;

		CatchUpToPlayer();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		//POR2
		if (other.name.ToLower() == "player" && pController.state != PlayerController.State.Dead)
		{
			pController.state = PlayerController.State.Dead;
			pController.deathBy = PlayerController.DeathBy.Water;
			pController.pointOfContact = pController.transform.position.y;
		}
		if(other.tag.ToLower() == "flyingenemy")
		{
			Destroy(other.gameObject);
		}
	}

	public void CatchUpToPlayer()
	{
		if(distanceToScreenBottom > allowedDistance)
		{
			speed = 1 + (1 * distanceToScreenBottom * speedAmp);
		}
		else
		{
			speed = baseSpeed;
		}
	}
}
