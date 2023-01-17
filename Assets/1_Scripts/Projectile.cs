using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {


	public Rigidbody2D rb;
	public Collider2D col;
	public Camera cam;
	public float speed = 20f;
	public float startPosY;

	Vector3 topRight;
	Vector3 bottomLeft;

	public ParticleSystem CircularExplosion;
	public ParticleSystem FireworkExplosion;

	public AudioSource projhit;

	private Vector2 movementVector;

	// Use this for initialization
	void Start () 
	{
		//rb.velocity = transform.right * speed;
		//print(rb.velocity);
		movementVector = transform.right * speed;
		//print(movementVector);
		startPosY = transform.position.y;

	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.Translate(Vector3.right * Time.deltaTime * speed);
		//transform.Translate(new Vector3(movementVector.x * Time.deltaTime, movementVector.y * Time.deltaTime, 0f), Space.World);
		OutOfBounds();
	}

	public void OnCollisionEnter2D(Collision2D hitInfo)
	{
		//rb.velocity = Vector3.zero;
		if (hitInfo.collider.name == "Player")
		{
			PlayerController pCon = hitInfo.gameObject.GetComponent<PlayerController>();
			//POR1
			if(pCon.state != PlayerController.State.Dead)
			{
				pCon.state = PlayerController.State.Dead;
				pCon.deathBy = PlayerController.DeathBy.Projectile;
			}

			//print(hitInfo.GetContact(hitInfo.contactCount -1).point);
			Vector2 hitpoint = hitInfo.GetContact(hitInfo.contactCount - 1).point;


			CircularExplosion.transform.SetParent(null);
			gameObject.SetActive(false);
			CircularExplosion.gameObject.transform.position = new Vector2(hitpoint.x, hitpoint.y);
			CircularExplosion.Play();

			FireworkExplosion.transform.SetParent(null);
			FireworkExplosion.gameObject.transform.position = new Vector2(hitpoint.x, hitpoint.y);
			FireworkExplosion.Play();


			Destroy(gameObject);

		}
		if(hitInfo.collider.tag == "Platform")
		{
			Vector2 hitpoint = hitInfo.GetContact(hitInfo.contactCount - 1).point;

			CircularExplosion.transform.SetParent(null);
			
			gameObject.SetActive(false);
			CircularExplosion.gameObject.transform.position = new Vector2(hitpoint.x, hitpoint.y);
			CircularExplosion.gameObject.transform.GetComponent<SelfDestruct>().DestructionCountdown();
			CircularExplosion.Play();

			FireworkExplosion.transform.SetParent(null);
			FireworkExplosion.gameObject.transform.position = new Vector2(hitpoint.x, hitpoint.y);
			FireworkExplosion.Play();

            if (Architecture.Managers.UserGameData.Instance.soundsOn)
            {
				projhit.Play();
			}
			

			Destroy(gameObject);
		}

	}

	void OutOfBounds()
	{
		topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane)); //Coords of top right corner of screen
		bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane)); //coords of bottom left corner of screen

		if(transform.position.x > topRight.x + 2f || transform.position.x < bottomLeft.x - 2f)
		{
			Destroy(gameObject);
		}
		if(transform.position.y > startPosY + 50f)
		{
			Destroy(gameObject);
		}
	}
}
