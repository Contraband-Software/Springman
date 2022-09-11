using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PauseFeatures : MonoBehaviour
{
    public TextMeshProUGUI pauseText;
    RectTransform pauseTextRect;

    public LeanTweenType EaseFadeText;

    public LeanTweenType pauseTextEaseIn;
    public LeanTweenType pauseTextEaseOut;

    public float yPosition;

    public void OnPause()
    {
        var color = pauseText.color;
        var fadeInColor = color;
        fadeInColor.a = 1;

        float startYPosition = pauseTextRect.anchoredPosition.y;
        float endYPosition = yPosition;

        LeanTween.value(gameObject, updatePositionValue, startYPosition, endYPosition, 0.2f).setIgnoreTimeScale(true).setEase(pauseTextEaseIn);
        LeanTween.value(gameObject, updateColorValue, color, fadeInColor, 0.2f).setIgnoreTimeScale(true).setEase(EaseFadeText);
    }

    public void OnResume()
    {
        var color = pauseText.color;
        var fadeOutColor = color;
        fadeOutColor.a = 0;

        float startYPosition = pauseTextRect.anchoredPosition.y;
        float endYPosition = 0f;

        LeanTween.value(gameObject, updatePositionValue, startYPosition, endYPosition, 0.2f).setIgnoreTimeScale(true).setEase(pauseTextEaseOut);
        LeanTween.value(gameObject, updateColorValue, color, fadeOutColor, 0.2f).setIgnoreTimeScale(true).setEase(EaseFadeText);
    }

    void Start()
    {
        var color = pauseText.color;
        color.a = 0;
        pauseText.color = color;

        pauseTextRect = pauseText.GetComponent<RectTransform>();
        pauseTextRect.anchoredPosition = new Vector2(0f, 0f);
    }

    void updateColorValue(Color val)
    {
        pauseText.color = val;
    }

    void updatePositionValue(float yPosition)
    {
        pauseTextRect.anchoredPosition = new Vector2(0f, yPosition);
    }
}
