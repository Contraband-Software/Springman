using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;

public class PCon_Premium : MonoBehaviour
{
    
    public enum State { Alive, Dead }
    public enum DeathBy { Water, Mine, Projectile }
    public enum Direction { Falling, Rising, Stationary }

    [Header("Player State")]
    public State state = State.Alive;
    public DeathBy deathBy;
    public Direction dir;

    [Header("Player Body Related")]
    public GameObject OGPlayer;
    public PlayerController OGPCon;
    public Rigidbody2D rb;
    public BoxCollider2D boxCollider2D;
    public Vector3 bounds;
    public float loadedInAt;

    [Header("Movement")]
    private const float ForcePerUnitConstant = 38.8928571f;
    public float initForce = 219f;
    public float force;
    public bool groundedAfterDeath = false;
    public bool inWaterPostDeath = false;

    [Header(" - Jumping")]
    public float jumpVelocity;
    public bool isGrounded = false;
    public RaycastHit2D rayCastHit;
    public Transform lastObjectHitTransform;
    public float jumpingOnY;
    public float fallMultiplier;
    float decel = 1f;
    public float LastPlatY = -4.5f;
    public bool bounceOneFrame = false;

    [Header(" - Better Jumping System")]
    public bool betterJumpSystemReady;
    public bool firstLanding = false;
    public Landing prevLanding;
    public System.Random rnd = new System.Random();
    public float xLoc;
    public string xLocSide;
    public string xLocDirection;
    public float smoothPositionX;
    public float midX;

    [Header("Environment")]
    public float halfPlatHeight;
    public GameObject water;
    float bottomOfWater;
    public Bounds waterBounds;
    [SerializeField] public LayerMask platformLayerMask;

    [Header("Camera")]
    public Camera cam;
    public CameraFollow camScript;
    public float zeroToEdge;

    [Header("Effects Related")]
    public bool splashed;

    [Header("Measurements")]
    public float maxNegVel;

    [Header("Skin Specifics")]
    public InGamePremCon igpc;
    public int skinIndex;
    public Animator animator;

    [Header("Effects")]
    public Bounce_Effects bounce_effects;

    public void Awake()
    {
        OGPlayer = GameObject.Find("Player");
        OGPCon = OGPlayer.GetComponent<PlayerController>();
        rb = OGPCon.rb;
        boxCollider2D = OGPCon.boxCollider2D;
        platformLayerMask = OGPCon.platformLayerMask;
        jumpVelocity = OGPCon.jumpVelocity;
        fallMultiplier = OGPCon.fallMultiplier;

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        halfPlatHeight = GameObject.FindGameObjectWithTag("Platform").GetComponent<BoxCollider2D>().bounds.extents.y;
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        camScript = cam.GetComponent<CameraFollow>();
        zeroToEdge = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane)).x;
        bounds = boxCollider2D.bounds.extents;

        initForce = ((zeroToEdge - bounds.x) * 2) * ForcePerUnitConstant;
        force = initForce;

        water = GameObject.Find("Water");
        waterBounds = water.GetComponent<BoxCollider2D>().bounds;
        //waterYWidth = water.transform.position.y - waterBounds.extents.y;
    }

    public void Start()
    {
        igpc = GameObject.Find("GameExclusivePremiumController").GetComponent<InGamePremCon>();
        skinIndex = igpc.skinIndex;

        firstLanding = false;
        splashed = false;

        loadedInAt = Time.time;

        //loading bounce sound
    }

    void FixedUpdate()
    {
        Jump();
        RespondToJump();
    }

    void Update()
    {

        bottomOfWater = water.transform.position.y - waterBounds.extents.y;

        if (rb.velocity.y < 0f)
        {
            dir = Direction.Falling;
        }
        if (state == State.Alive)
        {
            splashed = false;
        }

        AltMove();
        IsGrounded();
        KeepOnScreen();
        IfIsGrounded();
        //CheckDeath();

    }

    public void AltMove()
    {
        if (transform.position.y > jumpingOnY && state == State.Alive)
        {
            if (xLocDirection == "farLeft")
            {
                if (transform.position.x > midX && transform.position.x > xLoc)
                {
                    rb.AddForce(new Vector2(force * Time.deltaTime, 0f), ForceMode2D.Force);
                }
            }
            if (xLocDirection == "closeLeft")
            {
                if (transform.position.x < midX && transform.position.x < xLoc)
                {
                    rb.AddForce(new Vector2(force * Time.deltaTime, 0f), ForceMode2D.Force);
                }
            }
            if (xLocDirection == "closeRight")
            {
                if (transform.position.x > midX && transform.position.x > xLoc)
                {
                    rb.AddForce(new Vector2(force * Time.deltaTime, 0f), ForceMode2D.Force);
                }
            }
            if (xLocDirection == "farRight")
            {
                if (transform.position.x < midX && transform.position.x < xLoc)
                {
                    rb.AddForce(new Vector2(force * Time.deltaTime, 0f), ForceMode2D.Force);
                }
            }

        }
    }

    void Jump()
    {
        if (state == State.Alive && isGrounded)
        {
            rb.velocity = Vector2.up * jumpVelocity;
            splashed = false;
        }
    }
    
    public void IsGrounded()
    {
        float extraHeight = 0.05f;
        rayCastHit = Physics2D.BoxCast(new Vector2(boxCollider2D.bounds.center.x, boxCollider2D.bounds.center.y - extraHeight), boxCollider2D.bounds.size, 0f, Vector2.down, extraHeight, platformLayerMask);
        Color rayColour;
        if (rayCastHit.collider != null)
        {
            rayColour = Color.green;
            lastObjectHitTransform = rayCastHit.collider.transform;
        }
        else
        {
            rayColour = Color.red;
        }
        Debug.DrawRay(boxCollider2D.bounds.center + new Vector3(boxCollider2D.bounds.extents.x, 0), Vector2.down * (boxCollider2D.bounds.extents.y + extraHeight), rayColour);
        Debug.DrawRay(boxCollider2D.bounds.center - new Vector3(boxCollider2D.bounds.extents.x, 0), Vector2.down * (boxCollider2D.bounds.extents.y + extraHeight), rayColour);
        Debug.DrawRay(boxCollider2D.bounds.center - new Vector3(boxCollider2D.bounds.extents.x, boxCollider2D.bounds.extents.y + extraHeight), Vector2.right * (boxCollider2D.bounds.extents.x), rayColour);
        Debug.DrawRay(boxCollider2D.bounds.center - new Vector3(boxCollider2D.bounds.extents.x, boxCollider2D.bounds.extents.y - extraHeight), Vector2.right * (boxCollider2D.bounds.extents.x), rayColour);
        if (rayCastHit.collider != null && rb.velocity.y <= 0)
        {
            jumpingOnY = transform.position.y;

            if (transform.position.y - bounds.y < rayCastHit.collider.transform.position.y + rayCastHit.collider.bounds.extents.y &&
                transform.position.y + bounds.y > rayCastHit.collider.transform.position.y - rayCastHit.collider.bounds.extents.y)
            {
                isGrounded = false;
                bounceOneFrame = false;
            }
            else
            {
                isGrounded = true;

                if (state == State.Alive && !bounceOneFrame)
                {
                   
                    //Play Bounce Animation && Bounce Sound && Bounce VFX
                    animator.Play("bounce");
                    bounce_effects.PlayEffect();
                    // ^^
                    bounceOneFrame = true;
                }


                if (state == State.Dead && deathBy == DeathBy.Water)
                {
                    groundedAfterDeath = true;
                }
            }

            UpdateScore(rayCastHit);
        }
        else
        {
            isGrounded = false;
            bounceOneFrame = false;
            betterJumpSystemReady = true;
        }
    }

    void RespondToJump()
    {
        if (rb.velocity.y < 0 && isGrounded == false && groundedAfterDeath == false)
        {
            if (state == State.Alive || inWaterPostDeath == false)
            {
                rb.velocity += Vector2.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
            else
            {
                if (deathBy == DeathBy.Water || inWaterPostDeath)
                {
                    rb.velocity = Vector2.up * Physics.gravity.y * (fallMultiplier - 1) * jumpVelocity * decel * Time.deltaTime;
                }
                else
                {
                    rb.velocity += Vector2.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
                }
            }
        }
    }
    void UpdateScore(RaycastHit2D rayhit)
    {
        if (rayhit.collider.tag == "Platform" && LastPlatY < rayhit.collider.transform.position.y)
        {
            Architecture.Managers.GamePlay.GetReference().Score++;
            LastPlatY = rayhit.collider.transform.position.y;
        }
    }

    public void AssignNextX()
    {
        // assigns the next X location the player will gravitate to

        //xLoc = rnd.Next(-301, 301) / 100f;
        xLoc = ZoneBasedTarget();
        if (xLoc <= 0)
        {
            if (transform.position.x > xLoc)
            {
                xLocDirection = "farLeft";
                force = AdjustForce() * -1f;
            }
            if (transform.position.x < xLoc)
            {
                xLocDirection = "closeLeft";
                force = AdjustForce();
            }
        }
        else
        {
            if (transform.position.x > xLoc)
            {
                xLocDirection = "closeRight";
                force = AdjustForce() * -1f;
            }
            if (transform.position.x < xLoc)
            {
                xLocDirection = "farRight";
                force = AdjustForce();
            }
        }

        midX = (transform.position.x + xLoc) / 2f;
        //print("Bounced at: " + (transform.position.x).ToString() + ", Next Target: " + xLoc.ToString());
    }

    public float AdjustForce()
    {
        float dist = Mathf.Max(transform.position.x, xLoc) - Mathf.Min(transform.position.x, xLoc);
        float f = Convert.ToSingle(Math.Round(initForce * (dist / ((zeroToEdge - bounds.x) * 2)), 4));
        //print("Distance = " + dist.ToString());////////////////////
        return f;
    }

    public void KeepOnScreen()
    {
        float dist = zeroToEdge - bounds.x;

        //clamp the original parent objects position
        OGPlayer.transform.position = new Vector3(Mathf.Clamp(OGPlayer.transform.position.x, -dist, dist), OGPlayer.transform.position.y, OGPlayer.transform.position.z);
        transform.localPosition = Vector3.zero;
    }

    public void IfIsGrounded()
    {
        if (isGrounded == true && state == State.Alive)
        {
            dir = Direction.Rising;

            //print("Landed at zone: " + CalcZone(transform.position.x).ToString());///////////////////////
            if (betterJumpSystemReady == true)
            {
                BetterJumpSystem();
            }
            betterJumpSystemReady = false;
        }
    }

    public void BetterJumpSystem()
    {
        int landingZone = CalcZone(transform.position.x);

        //if player hasnt landed yet, the first landing object will be initialised
        if (firstLanding == false)
        {
            prevLanding = new Landing(landingZone);
            firstLanding = true;
        }

        //if grounds and has already landed at least once beforehand
        else
        {
            if (prevLanding.adjacents.Contains(landingZone))
            {
                //change the targetting to be everything but the adjacent zones
                TargetUnAdjacentZones();
            }
            else
            {
                //create a fresh landing object 
                prevLanding = new Landing(landingZone);
            }
        }
        AssignNextX();
    }

    public int CalcZone(float xPos)
    {
        int zone;
        if (xPos >= -3 && xPos < -2) { zone = 1; }
        else if (xPos >= -2 && xPos < -1) { zone = 2; }
        else if (xPos >= -1 && xPos < 0) { zone = 3; }
        else if (xPos >= 0 && xPos < 1) { zone = 4; }
        else if (xPos >= 1 && xPos < 2) { zone = 5; }
        else if (xPos >= 2 && xPos <= 3) { zone = 6; }
        else
        {
            zone = 0;
            print("ERROR - LANDING POSITION OUTSIDE DEDICATED ZONES");
            print("LANDED AT" + transform.position.x.ToString());
        }
        return zone;
    }

    public void TargetUnAdjacentZones()
    {
        List<int> allZones = new List<int> { 1, 2, 3, 4, 5, 6 };

        List<int> prevAdjacents = prevLanding.adjacents;
        prevLanding.targetZones.Clear();
        foreach (int zone in allZones)
        {
            if (prevAdjacents.Contains(zone) == false)
            {
                prevLanding.targetZones.Add(zone);
            }
        }
    }

    public float ZoneBasedTarget()
    {
        float targetX;
        int minVal;
        int maxVal;
        List<int> possibleZones = prevLanding.targetZones;
        int targetZone = possibleZones[rnd.Next(0, possibleZones.Count)];
        //print("TARGET ZONE: " + targetZone.ToString());//////////////////

        int dist = Convert.ToInt32(Math.Round((zeroToEdge - bounds.x - 0.00005f), 4) * 1000);

        if (targetZone == 1) { minVal = -dist; maxVal = -2001; }
        else if (targetZone == 2) { minVal = -2000; maxVal = -1001; }
        else if (targetZone == 3) { minVal = -1000; maxVal = -0001; }
        else if (targetZone == 4) { minVal = 0000; maxVal = 0999; }
        else if (targetZone == 5) { minVal = 1000; maxVal = 1999; }
        else if (targetZone == 6) { minVal = 2000; maxVal = dist; }
        else { minVal = 0; maxVal = 0; }

        targetX = rnd.Next(minVal - 1, maxVal + 1) / 1000f;
        return targetX;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.collider.tag == "SittingEnemy" && dir == Direction.Falling)
        {
            if (state != State.Dead)
            {
                state = State.Dead;
                deathBy = DeathBy.Mine;
            }
        }
    }
}

