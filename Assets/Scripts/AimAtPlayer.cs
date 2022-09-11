using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimAtPlayer : MonoBehaviour {

	public Transform player;
	public Rigidbody2D rb;

	// Use this for initialization
	void Awake () 
	{
		player = GameObject.Find("Player").GetComponent<Transform>();
		rb = this.GetComponent<Rigidbody2D>();
	}
	void Start()
	{
		Vector3 angles = transform.eulerAngles;
	}
	
	// Update is called once per frame
	void Update ()
	{
		AimLock();
	}

	private void AimLock()
	{
		Vector2 direction = new Vector2(player.transform.position.x, player.transform.position.y) - new Vector2(transform.position.x, transform.position.y);
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		//rb.rotation = angle;
		rb.rotation = Mathf.Lerp(rb.rotation, angle, 0.8f);
	}
}
