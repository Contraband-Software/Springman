using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FontDetails
{
    public Color32 faceColor;
    public float softness;
    public float faceDilate;
    public Color32 outlineColor;
    public float outlineWidth;

    public FontDetails(Color32 faceColor, float softness, float faceDilate, Color32 outlineColor, float outlineWidth)
    {
        this.faceColor = faceColor;
        this.softness = softness;
        this.faceDilate = faceDilate;
        this.outlineColor = outlineColor;
        this.outlineWidth = outlineWidth;
    }
}
