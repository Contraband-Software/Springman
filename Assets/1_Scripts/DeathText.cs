using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeathText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI deathText;
    [SerializeField] LeanTweenType easeIn;
    [SerializeField] LeanTweenType easeFadeText;

    public void Start()
    {
        var color = deathText.color;
        color.a = 0;
        deathText.color = color;
        deathText.rectTransform.localScale = Vector3.zero;
    }
    public void OnDeath()
    {
        var color = deathText.color;
        var fadeInColor = color;
        fadeInColor.a = 1;

        LeanTween.scale(deathText.gameObject, new Vector3(0.4f, 0.4f, 0.4f), 0.2f).setEase(easeIn);
        LeanTween.value(deathText.gameObject, updateColorValue, color, fadeInColor, 0.5f).setIgnoreTimeScale(true).setEase(easeFadeText);
    }

    void updateColorValue(Color val)
    {
        deathText.color = val;
    }
}
