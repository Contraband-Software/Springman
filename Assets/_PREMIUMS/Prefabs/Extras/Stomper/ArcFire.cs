using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcFire : MonoBehaviour
{
    [HideInInspector] public PremSkinDetailsDemo premDetails;
    [Header("Lines")]
    public LineRenderer arcLine;
    public LineRenderer arcWhiteLine;

    public float firingTime = 1f;

    private Transform startTran;
    private Vector3 startPosition;
    private Color whiteLineStarCol = Color.white;

    [Header("Particles")]
    public float colourVariation;
    public ParticleSystem sparksStart;
    public ParticleSystem sparksEnd;

    [Header("Sound")]
    public AudioSource killSound;

    private void Update()
    {
        UpdateArc();
        PlacePFX();
    }

    public void StartArc(Transform startTransform, Vector3 enemyPos)
    {
        ChangeColourToSkin();
        StartPFX();

        startTran = startTransform; 
        startPosition = startTransform.position;


        arcLine.positionCount = 2;
        arcLine.SetPosition(0, startPosition);
        arcLine.SetPosition(1, enemyPos);
        sparksEnd.gameObject.transform.position = enemyPos;

        arcWhiteLine.positionCount = 2;
        arcWhiteLine.SetPosition(0, startPosition);
        arcWhiteLine.SetPosition(1, enemyPos);

        whiteLineStarCol = arcWhiteLine.startColor;
        LeanTween.delayedCall(firingTime * 0.8f, CallStartFadeOut);
    }

    private void PlacePFX()
    {
        sparksStart.gameObject.transform.position = startPosition;
    }

    private void StartPFX()
    {
        sparksStart.Play();
        sparksEnd.Play();

        killSound.transform.SetParent(null);
        killSound.Play();
        killSound.gameObject.GetComponent<SelfDestruct>().DestructionCountdown();
    }

    private void CallStartFadeOut()
    {
        LeanTween.value(gameObject, UpdateArcColour, 1f, 0f, firingTime * 0.2f);
    }

    private void ChangeColourToSkin()
    {
        Color lineColour = premDetails.targetColor;
        arcLine.startColor = lineColour;
        arcLine.endColor = lineColour;

        InitializeColour();
    }

    private void InitializeColour()
    {
        var main = sparksStart.main;
        var main2 = sparksEnd.main;

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
        main2.startColor = new ParticleSystem.MinMaxGradient(baseColor, otherColor);
    }

    private void UpdateArc()
    {
        startPosition = startTran.position;
        arcLine.SetPosition(0, startPosition);
        arcWhiteLine.SetPosition(0, startPosition);

        firingTime -= Time.deltaTime;
        if(firingTime < 0f)
        {
            Destroy(gameObject);
        }
    }

    private void UpdateArcColour(float val)
    {
        Color arcLineCol = arcLine.startColor;
        arcLineCol.a = val;
        arcLine.startColor = arcLineCol;
        arcLine.endColor = arcLineCol;

        Color arcWhiteLineCol = arcWhiteLine.startColor;
        arcWhiteLineCol.a = whiteLineStarCol.a * val;
        arcWhiteLine.startColor = arcWhiteLineCol;
        arcWhiteLine.endColor = arcWhiteLineCol;

    }

}
