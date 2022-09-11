using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparencyAnimator : MonoBehaviour
{
    public SpriteRenderer target;
    public float alphaValue = 1f;

    private Color targetColour;
    public bool blackOutMode = false;
    public float opaqueValue = 1f;

    // Start is called before the first frame update
    void Start()
    {
        targetColour = target.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (!blackOutMode)
        {
            target.color = new Color(target.color.r, target.color.g, target.color.b, alphaValue);
        }
        else
        {
            target.color = new Color(opaqueValue, opaqueValue, opaqueValue, target.color.a);
        }
        
    }
}
