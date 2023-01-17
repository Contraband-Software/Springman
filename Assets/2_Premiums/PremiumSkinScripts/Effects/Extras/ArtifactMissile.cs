using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Backend;

public class ArtifactMissile : MonoBehaviour
{
    [Header("Important References")]
    [HideInInspector] public PremSkinDetailsDemo premDetails;
    [HideInInspector] public PlayerController pCon;
    [HideInInspector] public TapToKill tapToKillReference;
    [HideInInspector] public Animator playerConAnimator;

    [Header("Details")]
    public SpriteRenderer warheadSpriteRenderer;
    public SpriteRenderer baseSpriteRenderer;
    public List<Sprite> MissileColourVariants = new List<Sprite>();
    private CircleCollider2D thisCollider;
    private Rigidbody2D rb;
    private bool targetHit = false;

    [Header("Trail Details")]
    public TrailRenderer trailRend;

    [Header("ParticleSystem Details")]
    public ParticleSystem pSysCollision;
    public float colourVariation = 0.5f;

    [Header("Sound Effects")]
    public AudioSource explosion_effect;
    public AudioSource whistle_effect;

    [Header("Tracking")]
    public GameObject target;
    public Vector2 sensitivity = new Vector2(1f, 1f); //sens 1 snaps to enemy
    public Vector2 acceleration = new Vector2(10f, 10f);
    public Vector2 initialVelocity = new Vector2(5f, 5f); //initial direction and velocity the missile gets thrown out in
    public Vector2 velocityDamp = new Vector2(1f, 1f);
    private Vector2 velocityMultiplier = new Vector2(1f, 1f); //ramp the velocity the missile travels at

    [Header("Looping")]
    public GameObject missileLoopPivotPrefab;
    public float loopSize = 1f;
    public float loopStartTime = 0.1f;
    public float loopEndTime = 2f;
    private float launchTime;
    private float timeElapsed;

    [SerializeField] private bool currentlyLooping = false;
    private GameObject pivotPrefabInstance;
    private float loopDirection = 1f;
    private bool terminateLooping = false;

    private float initialVelocityHidden;
    private float velocityHidden;

    // Start is called before the first frame update
    void Start()
    {
        thisCollider = gameObject.GetComponent<CircleCollider2D>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;

        ChangeColourToSkin();

        launchTime = Time.time;
        currentlyLooping = false;
        InitialVelocity();

        AdjustEnemyEffects();

        if (Architecture.Managers.UserGameData.Instance.soundsOn)
        {
            whistle_effect.Play();
        }
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timeElapsed += Time.deltaTime;

        SeekTarget();
    }

    //launches it downwards in direction of more space
    private void InitialVelocity()
    {
        //player more towards right side of screen
        if(pCon.transform.position.x > 0f)
        {
            //Launch to //LEFT side
            initialVelocity = new Vector2(initialVelocity.x * -1, initialVelocity.y);
            playerConAnimator.Play("wing_flutter_left");
        }
        else
        {
            //right side
            playerConAnimator.Play("wing_flutter_right");
        }
        rb.velocity = initialVelocity;
    }

    private void SeekTarget()
    {
        if(target != null)
        {

            //ramp velocity change sensitivity
            //sensitivity += sensRamp * new Vector2(Time.deltaTime, Time.deltaTime);
            velocityMultiplier = new Vector2(velocityMultiplier.x + (acceleration.x * Time.deltaTime), velocityMultiplier.y + (acceleration.y * Time.deltaTime));
            velocityMultiplier = velocityMultiplier * velocityDamp;

            if (!currentlyLooping)
            {
                //find vector distance to target
                Vector2 positionDiff = target.transform.position - transform.position;
                positionDiff.Normalize();

                //sensitivity
                Vector2 sens;
                sens = sensitivity;
                Vector2 direction = positionDiff * sens;
                direction.Normalize();

                //rotate to face direction of travel
                float angle = (Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg) - 90f;
                transform.rotation = Quaternion.identity;
                transform.Rotate(0f, 0f, angle, Space.World);

                rb.velocity = initialVelocity + ((direction) * velocityMultiplier);
            }

            looping();
        }
    }

    //applies force perpendicular to velocity to cause circular motion
    private void looping()
    {
        if(timeElapsed > loopStartTime && !currentlyLooping && !terminateLooping)
        {
            currentlyLooping = true;

            Vector3 velocityVector = rb.velocity;
            Vector3 zVector = new Vector3(0f, 0f, 1f);
            Vector3 perpendicularVector = Vector3.Cross(velocityVector, zVector).normalized;
            if(velocityVector.x > 0)
            {
                perpendicularVector *= -1f;
            }

            Vector3 missileLoopPivotPos = transform.position + (perpendicularVector * loopSize);

            pivotPrefabInstance = GameObject.Instantiate(missileLoopPivotPrefab, missileLoopPivotPos, Quaternion.identity, null);

            if(rb.velocity.x < 0)
            {
                loopDirection = -1f;
            }

            //set missile as a child of the pivot node
            gameObject.transform.parent = pivotPrefabInstance.transform;

            //set hidden velocity to start working
            initialVelocityHidden = rb.velocity.magnitude;
            velocityHidden = initialVelocityHidden;
        }

        if (currentlyLooping)
        {
            //USE VECTOR 2 FOR HIDDEN VELOCITY
            //CALCULATE DIRECTION OF TRAVEL AND MULTIPLY BY THAT LIKE WITH NORMAL VELOCITY INCREASE

            velocityHidden = initialVelocityHidden + (velocityMultiplier.magnitude * 0.3f);


            float rotationAmount = loopDirection * Mathf.Abs(Mathf.Atan2(1f, loopSize)) * Mathf.Rad2Deg * Time.deltaTime * velocityHidden;
            //print("HIDDEN VEL: " + velocityHidden.ToString());
            //print("ROTATION AMOUNT: " + rotationAmount.ToString());

            pivotPrefabInstance.transform.Rotate(0f, 0f, rotationAmount);

            if(timeElapsed > loopEndTime)
            {
                currentlyLooping = false;
                terminateLooping = true;

                transform.parent = null;

                float angle = transform.eulerAngles.z;
                if(loopDirection == -1)
                {
                    angle = 360f - angle;
                }
                else
                {
                    angle *= -1f;
                }

                float xVel = Mathf.Sin(angle * Mathf.Deg2Rad) * velocityHidden;
                float yVel = Mathf.Cos(angle * Mathf.Deg2Rad) * velocityHidden;

                Vector2 newVelocity = new Vector2(xVel, yVel);
                rb.velocity = newVelocity;

                initialVelocity = rb.velocity;

                sensitivity = new Vector2(1f, 1f);

            }
        }
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.transform.root.tag == "FlyingEnemy")
        {
            if (!targetHit)
            {
                targetHit = true;
                print("HIT ENEMY");
                tapToKillReference.KillFlyingEnemy(target);
                rb.constraints = RigidbodyConstraints2D.FreezeAll;

                Vector3 localScale = pSysCollision.transform.localScale;
                pSysCollision.transform.SetParent(null);
                pSysCollision.transform.localScale = localScale;
                pSysCollision.transform.rotation = Quaternion.identity;
                pSysCollision.Play();

                warheadSpriteRenderer.enabled = false;
                baseSpriteRenderer.enabled = false;


                //DONT DESTROY, CALL A DELAYEDCALL LEAN TWEEN
                LeanTween.delayedCall(5f, selfDestruct).setIgnoreTimeScale(false);
            }
            
        }
    }

    private void AdjustEnemyEffects()
    {
        //Replace Explosion sound on FE
        AudioSource explosion_SFX_FlyingEnemy = target.transform.Find("Sound_Emitter").GetComponent<AudioSource>();
        explosion_SFX_FlyingEnemy.clip = explosion_effect.clip;
        explosion_SFX_FlyingEnemy.volume = explosion_effect.volume;

        //Change PFX on FE so that its invisible (change colour to transparent)
        ParticleSystem pSys_FE_Explosion = target.transform.Find("Explosion").GetComponent<ParticleSystem>();
        var main = pSys_FE_Explosion.main;

        Color oldEffectColour = main.startColor.color;
        Color newEffectColour = new Color(oldEffectColour.r, oldEffectColour.g, oldEffectColour.b, 0f);
        main.startColor = new ParticleSystem.MinMaxGradient(newEffectColour, newEffectColour);
    }

    private void ChangeColourToSkin()
    {
        string targetColString = Utilities.ColorToString(premDetails.targetColor);
        List<string> colourChoicesString = new List<string>();

        foreach (Color col in premDetails.colorChoices)
        {
            colourChoicesString.Add(Utilities.ColorToString(col));
        }

        int colourIndex = colourChoicesString.IndexOf(targetColString);

        warheadSpriteRenderer.sprite = MissileColourVariants[colourIndex];

        trailRend.startColor = premDetails.colorChoices[colourIndex];
        Color endColor = premDetails.colorChoices[colourIndex];
        endColor.a = 0f;
        trailRend.endColor = endColor;

        InitializeColour();
    }

    private void InitializeColour()
    {
        var main = pSysCollision.main;

        Color baseColor = premDetails.targetColor;
        float r = baseColor.r * 255f;
        float g = baseColor.g * 255f;
        float b = baseColor.b * 255f;
        float a = baseColor.a * 255f;

        List<float> colorVals = new List<float>() { r, g, b };
        List<float> newColor = new List<float>() { r, g, b, a };

        float highest = 0f;

        foreach (float val in colorVals)
        {
            if (val > highest)
            {
                highest = val;
            }
        }
        colorVals.Remove(highest);

        float _2ndHighest = 0f;
        foreach (float val in colorVals)
        {
            if (val > _2ndHighest)
            {
                _2ndHighest = val;
            }
        }
        float distanceToTravel = Mathf.RoundToInt((255f - _2ndHighest) * colourVariation);
        colorVals.Remove(_2ndHighest);
        float stepRatio = (255f - colorVals[0]) / (255f - _2ndHighest);

        float lowest = colorVals[0];

        newColor[newColor.IndexOf(_2ndHighest)] = _2ndHighest + distanceToTravel;
        newColor[newColor.IndexOf(lowest)] = lowest + (distanceToTravel * stepRatio);

        Color otherColor = new Color(newColor[0] / 255f, newColor[1] / 255f, newColor[2] / 255f, newColor[3] / 255f);

        main.startColor = new ParticleSystem.MinMaxGradient(baseColor, otherColor);
    }

    private void selfDestruct()
    {
        Destroy(pivotPrefabInstance);
        Destroy(gameObject.transform.root.gameObject);
    }
}
