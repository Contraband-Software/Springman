using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Architecture.Audio;
using Architecture.Managers;
using Backend;

public class Bounce_Effects : MonoBehaviour
{

    //ADD FIELD FOR SOUND CLIP NAME
    //DIFFERENTIATE BY DOING NAME:FUNCTION
    [Header("Name of Premium")]
    public string premium_name;

    [Header("Important References")]
    [HideInInspector]
    public PremSkinDetailsDemo premDetails;
    [HideInInspector]
    public PlayerController playerCon;
    
    public EffectController effectCon;

    [Header("Details")]
    public float colourVariation = 0.4f;
    public string bounceSoundName;
    public ParticleSystem bounce_pSys;
    public GameObject cloneParent;          //from what object should the particle system be spawned at

    [Header("OVERRIDES")]
    public bool override_standard_bounce_effect;

    [Header("Bounce Animator")]
    public Animator bounce_animator;

    [Header("Artifact Details")]
    public GameObject bounce_effect_artifact;

    [Header("Charged Details")]
    public GameObject bounce_effect_charged;

    [Header("Neon Details")]
    private bool beganShifting = false;
    private Color currentColour;
    private Color targetColour;
    public float shiftSpeed = 1f;

    public void Start()
    {
        premDetails = gameObject.GetComponent<PremSkinDetailsDemo>();
        premium_name = UserGameData.Instance.activePremiumSkinName;

        //Sound Loading
        effectCon = transform.parent.gameObject.GetComponent<EffectController>();
        playerCon = transform.parent.gameObject.GetComponent<PlayerController>();

        Sound loadedBounceSound = playerCon.FindSound(bounceSoundName, effectCon.bounce_sounds);
        playerCon.bounceSound.clip = loadedBounceSound.clip;
        playerCon.bounceSound.volume = loadedBounceSound.volume;

    }

    public void Update()
    {
        if(premium_name == "Neon" && premDetails.colourShift)
        {
            Neon_ColourShift();
        }
    }

    //make list of functions
    public void PlayEffect()
    {
        // CHANGE COLOUR OF PARTICLES TO BE MATCHING THE GLOW COLOUR
        if(bounce_pSys != null)
        {
            InitializeColour();

            GameObject pSys = Instantiate(bounce_pSys.gameObject, cloneParent.transform);
            pSys.GetComponent<ParticleSystem>().Play();
        }


        //function assignment based off name
        switch (premium_name)
        {
            case "Artifact":
                Artifact();
                break;

            case "Charged":
                Charged();
                break;

            case "Alien":
                Alien();
                break;

            case "Gyrocube":
                Gyrocube();
                break;

            case "Stomper":
                Stomper();
                break;

            case "LightBulb":
                LightBulb();
                break;

            case "Molten Core":
                MoltenCore();
                break;

            case "Dual Cannon":
                DualCannon();
                break;

            case "Neon":
                Neon();
                break;

            case "PC":
                PC();
                break;

            default:
                break;
        }
        //Artifact();
        // 
    }

    private void InitializeColour()
    {
        var main = bounce_pSys.main;

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

    private void Artifact()
    {
        //Glowy Serrated Bottom Fade Effect
        GameObject bounce_clone = Instantiate(bounce_effect_artifact, bounce_effect_artifact.gameObject.transform.parent.transform);
        bounce_clone.SetActive(true);

        bounce_clone.GetComponent<SelfDestruct>().DestructionCountdown();
        bounce_clone.GetComponent<SpriteRenderer>().color = premDetails.targetColor;
        bounce_clone.GetComponent<Animator>().Play("bounce_effect");

        //Bounce Animation
        bounce_animator.Play("bounce");

        //Bounce Sound
        if (Architecture.Managers.UserGameData.Instance.soundsOn)
        {
            playerCon.bounceSound.Play();
        }

        playerCon.bounceDust.Play();
    }

    private void Charged()
    {
        //Bounce Animation
        bounce_animator.Play("bounce");
        bounce_animator.Play("bounce_effect");

        //Bounce Sound
        if (Architecture.Managers.UserGameData.Instance.soundsOn)
        {
            playerCon.bounceSound.Play();
        }

        playerCon.bounceDust.Play();
    }

    private void Alien()
    {
        bounce_animator.Play("bounce");
        bounce_animator.Play("bounce_alien_dude");
        //Bounce Sound
        if (Architecture.Managers.UserGameData.Instance.soundsOn)
        {
            playerCon.bounceSound.Play();
        }

        playerCon.bounceDust.Play();
    }

    private void Gyrocube()
    {
        bounce_animator.Play("bounce");
        //Bounce Sound
        if (Architecture.Managers.UserGameData.Instance.soundsOn)
        {
            playerCon.bounceSound.Play();
        }

        playerCon.bounceDust.Play();
    }

    private void Stomper()
    {
        bounce_animator.Play("bounce");
        //Bounce Sound
        if (Architecture.Managers.UserGameData.Instance.soundsOn)
        {
            playerCon.bounceSound.Play();
        }

        playerCon.bounceDust.Play();
    }

    private void LightBulb()
    {
        bounce_animator.Play("bounce");
        //Bounce Sound
        if (Architecture.Managers.UserGameData.Instance.soundsOn)
        {
            playerCon.bounceSound.Play();
        }

        playerCon.bounceDust.Play();
    }

    public void LightBulb_ColourSwitch()
    {
        if (premDetails.colourShift)
        {
            int newColourIndex = Random.Range(0, premDetails.colorChoices.Count);
            if(premDetails.colorChoices[newColourIndex] == premDetails.targetColor)
            {
                LightBulb_ColourSwitch();
            }
            else
            {
                premDetails.targetColor = premDetails.colorChoices[newColourIndex];
                premDetails.UpdateGlow_Soft();
                premDetails.AllGlowsTransparent_Soft();
            }
        }
    }

    private void MoltenCore()
    {
        bounce_animator.Play("bounce");
        //Bounce Sound
        if (Architecture.Managers.UserGameData.Instance.soundsOn)
        {
            playerCon.bounceSound.Play();
        }

        playerCon.bounceDust.Play();
    }

    private void DualCannon()
    {
        bounce_animator.Play("bounce");
        //Bounce Sound
        if (Architecture.Managers.UserGameData.Instance.soundsOn)
        {
            playerCon.bounceSound.Play();
        }

        playerCon.bounceDust.Play();
    }

    private void Neon()
    {
        bounce_animator.Play("bounce");
        //Bounce Sound
        if (Architecture.Managers.UserGameData.Instance.soundsOn)
        {
            playerCon.bounceSound.Play();
        }

        playerCon.bounceDust.Play();
    }

    private void Neon_ColourShift()
    {
        if (!beganShifting)
        {
            beganShifting = true;
            Neon_ColourShift_SelectTarget();
        }
    }
    private void Neon_ColourShift_SelectTarget()
    {
        if(playerCon.state != PlayerController.State.Dead)
        {
            currentColour = premDetails.targetColor;
            string currentColString = Utilities.ColorToString(currentColour);
            List<string> colourChoicesString = new List<string>();
            foreach (Color col in premDetails.colorChoices)
            {
                colourChoicesString.Add(Utilities.ColorToString(col));
            }

            int currentIndex = colourChoicesString.IndexOf(currentColString);

            targetColour = premDetails.colorChoices[((currentIndex % (premDetails.colorChoices.Count - 1)) + 1)];

            LeanTween.value(gameObject, Neon_ColourShift_Callback, currentColour, targetColour, shiftSpeed).setOnComplete(Neon_ColourShift_SelectTarget).setEase(LeanTweenType.linear);
        }
    }

    private void Neon_ColourShift_Callback(Color col)
    {
        if (playerCon.state != PlayerController.State.Dead)
        {
            premDetails.targetColor = col;
            premDetails.UpdateGlow_Soft();
        }
    }

    private void PC()
    {
        bounce_animator.Play("bounce");
        //Bounce Sound
        if (Architecture.Managers.UserGameData.Instance.soundsOn)
        {
            playerCon.bounceSound.Play();
        }

        if (!bounce_animator.GetBool("moneyAnimPlaying"))
        {
            bounce_animator.Play("pc_flash_bounce");
        }

        playerCon.bounceDust.Play();
    }
}
