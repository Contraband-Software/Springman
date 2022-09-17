using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReviveCountdownScript : MonoBehaviour
{
    public Canvas reviveCountdownCanvas;
    private int count = 3;

    public LeanTweenType scaleUpTween;
    public LeanTweenType fadeOutTween;

    public RectTransform numberObj;
    public TextMeshProUGUI numberText;
    public CanvasGroup cg;

    private Vector3 initialSize;

    public void BeginCountdown()
    {
        initialSize = numberObj.localScale;
        reviveCountdownCanvas.enabled = true;

        FadeOutNumber();
    }

    private void FadeOutNumber()
    {
        LeanTween.value(gameObject, UpdateNumberSize, numberObj.localScale.x, 1.6f, 1f).setIgnoreTimeScale(true).setEase(scaleUpTween);
        LeanTween.value(gameObject, UpdateOpacity, 1f, 0f, 1f).setIgnoreTimeScale(true).setOnComplete(NextNumber).setEase(fadeOutTween);
    }

    private void UpdateNumberSize(float val)
    {
        Vector3 newSize = new Vector3(val, val, val);
        numberObj.localScale = newSize;
    }
    private void UpdateOpacity(float val)
    {
        cg.alpha = val;
    }

    private void NextNumber()
    {
        if(count != 1)
        {
            numberObj.localScale = initialSize;
            cg.alpha = 1f;
            count--;
            numberText.text = count.ToString();
            FadeOutNumber();
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}
