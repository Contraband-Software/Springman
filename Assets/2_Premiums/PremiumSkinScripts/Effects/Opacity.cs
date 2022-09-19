using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opacity : MonoBehaviour
{
    public float opacity;
    public SpriteRenderer spr;

    public void Update()
    {
        spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, opacity);
    }
}
