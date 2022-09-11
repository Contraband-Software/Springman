using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcController : MonoBehaviour
{
    public LineRenderer lineRend;

    [SerializeField]
    private Texture[] textures;

    private int animationStep = 0;
    public float fps = 30f;
    private float fpsCounter = 0f;


    private void Update()
    {
        fpsCounter += Time.deltaTime;
        if(fpsCounter >= 1f / fps)
        {
            animationStep++;
            if(animationStep == textures.Length)
            {
                animationStep = 0;
            }
            lineRend.material.SetTexture("_MainTex", textures[animationStep]);
            fpsCounter = 0f;
        }
    }
}
