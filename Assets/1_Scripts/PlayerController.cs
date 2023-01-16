using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.Events;

using Architecture.Audio;
using Architecture.Managers;

public class PlayerController : MonoBehaviour {
    [Header("Player Duplication Reference")]
    private GameObject playerCopy;

    [Serializable] public class AssignPlayerRefEvent : UnityEvent<PlayerController> { };
    //[Header("On Revive, Reassign References")]
    public event Action<PlayerController> revive_Reassign;

    //public AssignPlayerRefEvent revive_Reassign_Event;
    [Header("Highest Platform Hit")]
    public GameObject highestPlatformHit;
    public CreatePlatforms createPlatsRef;

    [Header("Important References")]
    public EffectController effectCon;
    [SerializeField] DeathScreenScript deathScreenManager;

    [Header("DeathSprite")]
    public Sprite deathSprite;

    [Header("Cosmetics")]
    public CosmeticsController cosCon;

    public SpriteRenderer skinTopSprite;
    public SpriteRenderer eyesSprite;

    public SpriteRenderer baseSkin;

    public string bounce_animation;

    [Header("Sounds")]
    public string alternative_bounce_sound;
    public AudioSource bounceSound;

    public AudioSource deathSound;
    public AudioSource splashSound;

    private float defaultSplashVol;

    [Header("Particles")]
    public ParticleSystem bounceDust;
    public ParticleSystem splash;
    public bool splashed = false;

    [Header("Rest")]
    public Rigidbody2D rb;
    public Animator animator;
    //public SpriteRenderer spriteRend;
    [SerializeField] public LayerMask platformLayerMask;
    public Camera cam;
    public CameraFollow camScript;
    public GameObject water;
    float bottomOfWater;
    float waterYWidth;
    public float zeroToEdge;

    public enum State { Alive, Dead }
    public State state = State.Alive;

    public Vector3 bounds;


    // Jump Variables
    public float jumpVelocity;
    public float fallMultiplier;
    public float jumpingOnY;

    //grounded variables
    public BoxCollider2D boxCollider2D;
    public bool isGrounded;

    //X Location variables
    public float xLoc;
    public string xLocSide;
    public string xLocDirection;
    public float smoothPositionX;

    //MidX variables
    public float midX;

    //speed variable
    private const float ForcePerUnitConstant = 38.8928571f;
    public float initForce = 219f;
    public float force;
    public float velocity = 0.1f;
    public float mtp = 1.89f;

    //raycast
    public RaycastHit2D rayCastHit;
    public Transform lastObjectHitTransform;

    //platSize
    public float halfPlatHeight = 0.15f;

    //death variables
    public enum DeathBy { Water, Mine, Projectile}
    public DeathBy deathBy;

    public enum Direction { Falling, Rising, Stationary}
    public Direction dir;

    //scoring variables
    public float LastPlatY = -4.5f;
    //water
    Bounds waterBounds;
    public System.Random rnd = new System.Random();

    public float delayBounceBy;

    public event DeathEvent OnDeath;
    public delegate void DeathEvent();

    [Header("Bounce Timing")]
    List<float> diffs = new List<float>();
    public bool bouncedInit = false;
    public float firstBounceTime = 0f;
    public float thisBounceTime = 0f;
    public bool caughtTime = false;

    public float loadedInAt;

    public bool bounceOneFrame = false;

    private float lastNonZeroVelY = 0f;

    [Header("Measurements")]
    public float bounceTimeDiff;
    public float hitsFloorAt;
    public float hitsFirstPlat;
    public float avgBTD;


    void Awake()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        halfPlatHeight = GameObject.FindGameObjectWithTag("Platform").GetComponent<BoxCollider2D>().bounds.extents.y;
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        camScript = cam.GetComponent<CameraFollow>();
        zeroToEdge = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane)).x;
        bounds = gameObject.GetComponent<BoxCollider2D>().bounds.extents;

        initForce = ((zeroToEdge - bounds.x) * 2) * ForcePerUnitConstant;
        force = initForce;

        waterBounds = water.GetComponent<BoxCollider2D>().bounds;
        //waterYWidth = water.transform.position.y - waterBounds.extents.y;
    }

    private void Start()
    {
        //loads standard bounce anim and sound
        bounce_animation = UserGameData.Instance.cSpecs.bounce_anim;//EFFECT
        firstLanding = false;
        splashed = false;

        loadedInAt = Time.time;

        alternative_bounce_sound = UserGameData.Instance.cSpecs.alt_BounceSound;//EFFECT
        Sound loaded_BounceSound;

        InitialiseSplashColour();
        

        if (!UserGameData.Instance.currentSkinPremium)
        {
            if (alternative_bounce_sound != null && alternative_bounce_sound != "")
            {
                loaded_BounceSound = FindSound(alternative_bounce_sound, effectCon.bounce_sounds);
            }
            else
            {
                loaded_BounceSound = FindSound(bounce_animation, effectCon.bounce_sounds);
            }

            bounceSound.clip = loaded_BounceSound.clip;
            bounceSound.volume = loaded_BounceSound.volume;

        }
        defaultSplashVol = FindSound("water", effectCon.death_sounds).volume;

        DuplicatePlayer_ForRevive();
    }

    private void DuplicatePlayer_ForRevive()
    {
        playerCopy = Instantiate(gameObject);
        playerCopy.SetActive(false);
        //playerCopy.GetComponent<PlayerController>().revive_Reassign = null;
    }

    public void Revive()
    {
        //reset player variables, active other copy
        //give all event listeners a reference to the new player controller copy
        print("REVIVING PLAYER " + gameObject.name);
        PlayerController pConCopy = playerCopy.GetComponent<PlayerController>();
        if (pConCopy == null)
        {
            print("COPY PLAYER NULLL");
        }
        if(revive_Reassign == null)
        {
            print("revive reassing null");
        }
        revive_Reassign.Invoke(pConCopy);

#region REMOVENEMIES
        //REMOVE ENEMIES
        foreach (GameObject enemy in GamePlay.GetReference().EnemiesActive)
        {
            Destroy(enemy);
        }
        GamePlay.GetReference().EnemiesActive = new List<GameObject>();
#endregion

        //Active other player copy
        playerCopy.gameObject.name = "Player";
        gameObject.name = "Player(Dead)";

        pConCopy.createPlatsRef.dontCreateFirstPlat = true;
        pConCopy.createPlatsRef.highestPlat = createPlatsRef.highestPlat;
        pConCopy.highestPlatformHit = highestPlatformHit;

        //prepare platform to spawn onto
        if(highestPlatformHit != null)
        {
            //delete screw if on platform
            Transform screw = highestPlatformHit.transform.Find("SilverScrew(Clone)");
            if (screw != null)
            {
                screw.gameObject.SetActive(false);
            }

            highestPlatformHit.GetComponent<SlideMove>().disableMovement = true;

            Vector3 newScale = highestPlatformHit.transform.localScale;
            newScale.x = 8f;
            highestPlatformHit.transform.localScale = newScale;

            if (highestPlatformHit.GetComponent<ArrangeHole>() != null)
            {
                ArrangeHole ahs = highestPlatformHit.GetComponent<ArrangeHole>();
                Vector3 newPos = highestPlatformHit.transform.position;
                newPos.x = ahs.leftFlat.transform.position.x * -1f;
                print(ahs.leftFlat.transform.position.x);
                highestPlatformHit.transform.position = newPos;

            }
            else
            {
                Vector3 newPos = highestPlatformHit.transform.position;
                newPos.x = 0f;
                highestPlatformHit.transform.position = newPos;
            }
        }

        //place player on platform
        Vector3 newSpawnPos = playerCopy.transform.position;
        newSpawnPos.x = 0f;
        if (highestPlatformHit != null)
        {
            newSpawnPos.y = highestPlatformHit.transform.position.y + 1.75f;
        }
        else
        {
            newSpawnPos.y = 0f;
        }
        
        playerCopy.transform.position = newSpawnPos;

        //Move water to be below player
        Vector3 newWaterPos = water.transform.position;
        newWaterPos.y = playerCopy.transform.position.y - (waterBounds.size.y * 1.2f);
        water.transform.position = newWaterPos;

        //focus camera on new platform
        camScript.AutoFocus_OnRespawn();

        playerCopy.SetActive(true);

        //gameObject.SetActive(false);


        //pause time
        Time.timeScale = 0f;
        Destroy(gameObject);
    }

    void FixedUpdate()
    {
        Jump();
        RespondToJump();
    }
    void Update()
    {
        if(rb.velocity.y != 0)
        {
            lastNonZeroVelY = rb.velocity.y;
        }

        bottomOfWater = water.transform.position.y - waterBounds.extents.y;

        if (rb.velocity.y < 0f)
        {
            dir = Direction.Falling;
        }
        if(state == State.Alive)
        {
            splashed = false;
        }

        AltMove();
        IsGrounded();
        KeepOnScreen();
        IfIsGrounded();
        CheckDeath();

        
    }

    public Sound FindSound(string sound_name, List<Sound> list)
    {
        foreach(Sound s in list)
        {
            if(s.name == sound_name)
            {
                return s;
            }
        }
        throw new ArgumentException("Unknown sound name");
    }


    public float pointOfContact;
    bool gameSaved = false;

    void CheckDeath()
    {
        if(state == State.Dead)
        {
            deathScreenManager.DeathScreenShow(GamePlay.GetReference().Score);

            effectCon.DeathAllEffect();//_EFFECT

            if(UserGameData.Instance.cSpecs.eyes_death != null)
            {
                eyesSprite.sprite = UserGameData.Instance.cSpecs.eyes_death;
            }
            if (!gameSaved)
            {
                OnDeath?.Invoke();
                StartCoroutine(DelayedSaveGame());
                gameSaved = true;
            }
            
            switch (deathBy)
            {
                case DeathBy.Water:
                    DeathByWater();
                    break;
                case DeathBy.Mine:
                    DeathByMine();
                    break;
                case DeathBy.Projectile:
                    DeathByProjectile();
                    break;
            }
        }
    }
    IEnumerator DelayedSaveGame()
    {
        yield return new WaitForSecondsRealtime(0.25f);
        UserGameData.Instance.SaveGameData();
    }

    float decel = 1f;
    float yVelOnEntry;
    bool groundedAfterDeath = false;

    bool deathJumpDone = false;
    public bool inWaterPostDeath = false;
    private void DeathByWater()
    {
        if (state == State.Dead)
        {
            //rb.drag = ((pointOfContact - transform.position.y) * 20f);
            if(transform.position.y <= bottomOfWater - boxCollider2D.bounds.extents.y - 2f)
            {
                rb.constraints = RigidbodyConstraints2D.FreezePositionY;
            }
        }
        if(deathBy == DeathBy.Water)
        {
            if (!splashed)
            {

                //THE SPLASH EFFECT - 

                //splash.transform.SetParent(null);
                Quaternion blankQt = Quaternion.identity;
                blankQt.x = splash.transform.rotation.x;

                Vector3 prevPosition = splash.transform.position;
                GameObject clonedSplash = Instantiate(splash.gameObject, null);
                //splash.transform.SetParent(null);
                clonedSplash.transform.rotation = blankQt;
                clonedSplash.transform.position = prevPosition;

                splashed = true;
                clonedSplash.GetComponent<ParticleSystem>().Play();//EFFECT
                // ----- ^^^^^

                //effectCon.DeathByWaterEffect();//_EFFECT

                splashSound.volume = defaultSplashVol;
                splashSound.volume = Mathf.Abs(lastNonZeroVelY / 20f) * splashSound.volume;

                if (UserGameData.Instance.soundsOn)
                {
                    splashSound.Play();//EFFECT
                }
                
            }

            decel = Mathf.Max(decel - (1f * Time.deltaTime), 0.4f);
            if (deathJumpDone == false)
            {

                //effectCon.DeathByWaterEffect_Continuation();

                if (force < 0f)
                {
                    animator.Play("Rotate");//EFFECT
                }
                if (force > 0f)
                {
                    animator.Play("RotateRight");//EFFECT
                }
                yVelOnEntry = rb.velocity.y;
                decel = -0.07f * yVelOnEntry;

                deathJumpDone = true;
            }

            if (groundedAfterDeath)
            {



                rb.constraints = RigidbodyConstraints2D.None;
                animator.SetFloat("rotSpeedMult", 0f);
                animator.StopPlayback();
                animator.SetBool("HitGround", true);

                animator.enabled = false;
                rb.drag = 45f;
                //animator.Play("Default");
            }
            else
            {
                animator.SetFloat("rotSpeedMult", 0.07f);
            }
        }
        else
        {
            decel = Mathf.Max(decel - (1f * Time.deltaTime), 0.4f);
        }
    }

    void DeathByMine()
    {
        if (inWaterPostDeath)
        {
            animator.SetFloat("rotSpeedMult", (rb.velocity.y) * -0.15f);


            DeathByWater();
        }

        if (deathJumpDone != true)
        {          
            rb.velocity = Vector2.up * 6f;
            boxCollider2D.isTrigger = true;
            animator.Play("Rotate");//EFFECT
            animator.Play("ScaleUpDown");//EFFECT

            InChildren(transform);
            deathJumpDone = true;

            Sound death_by_spike = FindSound("spike", effectCon.death_sounds);
            deathSound.clip = death_by_spike.clip;
            deathSound.volume = death_by_spike.volume;

            if (UserGameData.Instance.soundsOn)
            {
                deathSound.Play();//EFFECT
            }
                
        }
        if(transform.position.y < bottomOfWater -5f)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Water")
        {
            inWaterPostDeath = true;
            yVelOnEntry = rb.velocity.y;
            decel = -0.07f * yVelOnEntry;
            state = State.Dead;
            deathBy = DeathBy.Water;
            boxCollider2D.isTrigger = false;
        }
        if(collision.gameObject.name == "SplashZone")
        {
            if (!splashed)
            {
                //splash.transform.SetParent(null);
                Quaternion blankQt = Quaternion.identity;
                blankQt.x = splash.transform.localRotation.x;

                Vector3 prevPosition = splash.transform.position;
                GameObject clonedSplash = Instantiate(splash.gameObject, null);
                //splash.transform.SetParent(null);
                clonedSplash.transform.rotation = blankQt;
                clonedSplash.transform.position = prevPosition;
                clonedSplash.transform.localScale = new Vector3(1f, 0f, 1f);

                if(state == State.Alive)
                {
                    splashSound.volume = defaultSplashVol;
                    splashSound.volume = Mathf.Abs(lastNonZeroVelY / 20) * splashSound.volume;

                    if (UserGameData.Instance.soundsOn)
                    {
                        splashSound.Play();//EFFECT
                    }
                    
                }
                else
                {
                    
                    splashSound.volume = defaultSplashVol;
                    splashSound.volume = Mathf.Abs(lastNonZeroVelY / 20f) * splashSound.volume;

                    if (UserGameData.Instance.soundsOn)
                    {
                        splashSound.Play();//EFFECT
                    }
                    
                }

                splashed = true;
                clonedSplash.GetComponent<ParticleSystem>().Play();
            }
        }
    }

    void InChildren(Transform transform)
    {

        if(transform.GetComponent<SpriteRenderer>() != null)
        {
            transform.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "OverrideToFront";
        }

        if (transform.childCount > 0)
        {
            for (int child = 0; child <= transform.childCount - 1; child++)
            {
                InChildren(transform.GetChild(child).GetComponent<Transform>());
            }
        }
    }
    
    void DeathByProjectile()
    {

        animator.SetBool("HitByProjectile", true);
        if (inWaterPostDeath)
        {
            animator.SetFloat("rotSpeedMult", (rb.velocity.y) * -0.15f);
            DeathByWater();
        }

        if (deathJumpDone != true)
        {
            boxCollider2D.isTrigger = true;
            animator.Play("Rotate");//EFFECT

            InChildren(transform);
            Sound death_by_proj = FindSound("projectile", effectCon.death_sounds);
            deathSound.clip = death_by_proj.clip;
            deathSound.volume = death_by_proj.volume;

            if (UserGameData.Instance.soundsOn)
            {
                deathSound.Play();//EFFECT
            }
            
            deathJumpDone = true;
        }
        if (transform.position.y < bottomOfWater - 5f)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    public void AssignNextX()
    {
        // assigns the next X location the player will gravitate to

        //xLoc = rnd.Next(-301, 301) / 100f;
        xLoc = ZoneBasedTarget();
        if(xLoc <= 0)
        {
            if(transform.position.x > xLoc)
            {
                xLocDirection = "farLeft";
                force = AdjustForce() * -1f;
            }
            if(transform.position.x < xLoc)
            {
                xLocDirection = "closeLeft";
                force = AdjustForce();
            }
        }
        else
        {
            if(transform.position.x > xLoc)
            {
                xLocDirection = "closeRight";
                force = AdjustForce() * -1f;
            }
            if(transform.position.x < xLoc)
            {
                xLocDirection = "farRight";
                force = AdjustForce();
            }
        }
        
        if(xLoc > transform.position.x && UserGameData.Instance.cSpecs.skin_name != "imposter") //next target RIGHT
        {
            eyesSprite.flipX = true;
            if (UserGameData.Instance.cSpecs.skin_Top_Flippable)
            {
                skinTopSprite.flipX = true;

                if(skinTopSprite.sprite == null)
                {
                    baseSkin.flipX = true;
                }
            }
        }
        if(xLoc < transform.position.x && UserGameData.Instance.cSpecs.skin_name != "imposter")
        {
            eyesSprite.flipX = false;
            if (UserGameData.Instance.cSpecs.skin_Top_Flippable)
            {
                skinTopSprite.flipX = false;

                if (skinTopSprite.sprite == null)
                {
                    baseSkin.flipX = false;
                }
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

            if(transform.position.y - bounds.y < rayCastHit.collider.transform.position.y + rayCastHit.collider.bounds.extents.y &&
                transform.position.y + bounds.y > rayCastHit.collider.transform.position.y - rayCastHit.collider.bounds.extents.y)
            {
                isGrounded = false;
                bounceOneFrame = false;
            }
            else
            {
                isGrounded = true;

                ////Timing
                if (!caughtTime)
                {
                    caughtTime = true;

                    if (bouncedInit)
                    {
                        if(firstBounceTime == 0f)
                        {
                            firstBounceTime = Time.time;
                            hitsFirstPlat = firstBounceTime - hitsFloorAt;
                        }
                        else
                        {
                            thisBounceTime = Time.time;
                            bounceTimeDiff = thisBounceTime - firstBounceTime;

                            firstBounceTime = thisBounceTime;

                            //calc avg
                            float thisDiff = bounceTimeDiff;
                            diffs.Add(thisDiff);
                            float total = 0f;
                            foreach(float dif in diffs)
                            {
                                total += dif;
                            }
                            avgBTD = total / diffs.Count;
                        }
                    }

                    if (!bouncedInit)
                    {
                        bouncedInit = true;
                        hitsFloorAt = Time.time - loadedInAt;
                    }
                }

                ////

                if (state == State.Alive && !bounceOneFrame)
                {
                    effectCon.BounceEffect();//_EFFECT

                    //animator.Play(bounce_animation);//EFFECT
                    //bounceDust.Play();//EFFECT

                    /*
                    if (gamedata.soundsOn)
                    {
                        bounceSound.Play();//EFFECT
                    }
                    */
                    bounceOneFrame = true;
                }

                else if(state == State.Dead && deathBy == DeathBy.Water)
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
            caughtTime = false;
            betterJumpSystemReady = true;
        }
    }

    void UpdateScore(RaycastHit2D rayhit)
    {
        if(rayhit.collider.tag == "Platform" && LastPlatY < rayhit.collider.transform.position.y)
        {
            GamePlay.GetReference().Score++;
            LastPlatY = rayhit.collider.transform.position.y;
            highestPlatformHit = rayhit.collider.transform.root.gameObject;
        }
    }

    void RespondToJump()
    {
        if (rb.velocity.y < 0 && isGrounded == false && groundedAfterDeath == false)
        {
            if(state == State.Alive || inWaterPostDeath == false)
            {
                rb.velocity += Vector2.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
            else
            {
                if(deathBy == DeathBy.Water || inWaterPostDeath)
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

    //Keeping player on screen
    public void KeepOnScreen()
    {
        float dist = zeroToEdge - bounds.x;
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -dist, dist), transform.position.y, transform.position.z);
        //transform.position = new Vector3(zeroToEdge - bounds.x, transform.position.y, transform.position.z); //testing purposes only
    }


    public bool betterJumpSystemReady;
    public void IfIsGrounded()
    {
        if(isGrounded == true && state == State.Alive)
        {
            dir = Direction.Rising;

            //print("Landed at zone: " + CalcZone(transform.position.x).ToString());///////////////////////
            if(betterJumpSystemReady == true)
            {
                BetterJumpSystem();
            }
            betterJumpSystemReady = false;
        }
    }

    public bool firstLanding = false;
    public Landing prevLanding;
    public void BetterJumpSystem()
    {
        int landingZone = CalcZone(transform.position.x);

        //if player hasnt landed yet, the first landing object will be initialised
        if(firstLanding == false)
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
        List<int> allZones = new List<int>{ 1, 2, 3, 4, 5, 6 };

        List<int> prevAdjacents = prevLanding.adjacents;
        prevLanding.targetZones.Clear();
        foreach(int zone in allZones)
        {
            if(prevAdjacents.Contains(zone) == false)
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
        
        if(targetZone == 1) { minVal = -dist; maxVal = -2001; }
        else if(targetZone == 2) { minVal = -2000; maxVal = -1001; }
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
            if(state != State.Dead)
            {
                state = State.Dead;
                deathBy = DeathBy.Mine;
            }
        }
    }

    private void InitialiseSplashColour()
    {
        var main = splash.main;

        float a = 1f;

        Color themeColour = UserGameData.Instance.themeColour;
        int r = Mathf.RoundToInt(themeColour.r * 255f);
        int g = Mathf.RoundToInt(themeColour.g * 255f);
        int b = Mathf.RoundToInt(themeColour.b * 255f);

        List<int> colorVals255 = new List<int>() { r, g, b };
        List<int> newColor = new List<int>() { r, g, b };

        //find highest
        int highest = 0;
        foreach (int val in colorVals255)
        {
            if (val > highest)
            {
                highest = val;
            }
        }
        colorVals255.Remove(highest);

        //find 2nd highest
        int _2ndHighest = 0;
        foreach (int val in colorVals255)
        {
            if (val > _2ndHighest)
            {
                _2ndHighest = val;
            }
        }
        colorVals255.Remove(_2ndHighest);

        //last remaining is lowest
        int lowest = colorVals255[0];

        int d = Mathf.RoundToInt(highest * 0.34f);

        //insert back in correctly
        newColor[newColor.IndexOf(highest)] = highest + d;
        newColor[newColor.IndexOf(_2ndHighest)] = _2ndHighest + Mathf.RoundToInt(((float)_2ndHighest / (float)highest) * d);
        newColor[newColor.IndexOf(lowest)] = lowest + Mathf.RoundToInt(((float)lowest / (float)highest) * d); ;

        Color newCol = new Color(newColor[0] / 255f, newColor[1] / 255f, newColor[2] / 255f, a);
        main.startColor = new ParticleSystem.MinMaxGradient(newCol, newCol);
    }
}

public class Landing
{
    public int zone;
    public List<int> adjacents;
    public List<int> targetZones { get; set; }

    public Landing(int zone)
    {
        this.zone = zone;
        calcAdjacents();
        targetZones = new List<int> { 1, 2, 3, 4, 5, 6 };
    }
    private void calcAdjacents()
    {
        //furthermost zone can only have 1 adjacent (+ itself)
        if(zone == 1)
        {
            adjacents = new List<int> { 1, 2 };
        }

        //outermost zone can only have 1 adjacent (+ itself)
        if(zone == 6)
        {
            adjacents = new List<int> { 5, 6 };
        }
        else
        {
            int leftAdj = zone - 1;
            int rightAdj = zone + 1;
            adjacents = new List<int> { leftAdj, zone, rightAdj };          
        }
    }
}
