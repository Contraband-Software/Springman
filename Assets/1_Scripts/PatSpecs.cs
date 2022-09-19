using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatSpecs:MonoBehaviour
{

    public Vector4 standard;
    public Vector4 flippedInX;
    public Vector4 flippedInY;
    public Vector4 Rot180;

    public Vector4[] variants = new Vector4[4];

    private void Awake()
    {
        variants = new Vector4[] { standard, flippedInX, flippedInY, Rot180 };
    }
}
