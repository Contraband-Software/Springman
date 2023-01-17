using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Architecture.Managers;

public class ScaleToScreen_Water : MonoBehaviour
{
    Camera cam;
    Vector3 topRight;
    public float OscillationSpeed;

    private bool waterOscillating = false;
    private SpriteRenderer spr;

    void Start()
    {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane)); //Coords of top right corner of screen

        transform.localScale = new Vector3(topRight.x * 4.01f, topRight.x * 4.01f, 0f);
        transform.position = new Vector3(0f - topRight.x, transform.position.y, 0f);

        spr = gameObject.GetComponent<SpriteRenderer>();

        AdjustColour();
    }

    void Update()
    {
        if (!waterOscillating)
        {
            LeanTween.moveX(gameObject, topRight.x, OscillationSpeed).setOnComplete(ResetPosition);
            waterOscillating = true;
        }
    }

    public void ResetPosition()
    {
        transform.position = new Vector3(0f - topRight.x, transform.position.y, 0f);
        LeanTween.moveX(gameObject, topRight.x, OscillationSpeed).setOnComplete(ResetPosition);
    }

    private void AdjustColour()
    {
        float a = spr.color.a;

        spr.color = UserGameData.Instance.themeColour;
        int r = Mathf.RoundToInt(spr.color.r * 255f);
        int g = Mathf.RoundToInt(spr.color.g * 255f);
        int b = Mathf.RoundToInt(spr.color.b * 255f);

        List<int> colorVals255 = new List<int>() { r, g, b};
        List<int> newColor = new List<int>() { r, g, b};

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
        newColor[newColor.IndexOf(lowest)] = lowest + Mathf.RoundToInt(((float)lowest / (float)highest) * d);;

        Color newCol = new Color(newColor[0] / 255f, newColor[1] / 255f, newColor[2] / 255f, a);
        spr.color = newCol;
    }

}
