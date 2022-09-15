using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparencyAnimator : MonoBehaviour
{
    public SpriteRenderer target;
    public float alphaValue = 1f;

    private Color currentColour;
    public bool blackOutMode = false;
    public float opaqueValue = 1f;

    public bool callManually = false;
    private Color initialColour;
    public float targetOpaque = 0f;
    public float lerpTime = 1f;

    // Start is called before the first frame update
    void Start()
    {
        initialColour = target.color;
        currentColour = initialColour;
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
            if (!callManually)
            {
                target.color = new Color(opaqueValue, opaqueValue, opaqueValue, target.color.a);
            }
        }
        
    }

    public void setInitialColour()
    {
        initialColour = target.color;
    }

    public void fadeToTargetBlack()
    {
        LeanTween.value(gameObject, fadeBlackCallback, opaqueValue, targetOpaque, lerpTime);
    }

    private void fadeBlackCallback(float val)
    {
        currentColour.r = initialColour.r * val;
        currentColour.g = initialColour.g * val;
        currentColour.b = initialColour.b * val;
        target.color = currentColour;
    }
}
