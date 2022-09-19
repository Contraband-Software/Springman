using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TapToContinue : MonoBehaviour
{
    // Start is called before the first frame update
    public tutorialController tutCon;
    public Button continueButton;
    public TextMeshProUGUI text;

    public string phase;

    private void Start()
    {
        var color = text.color;
        color.a = 0f;
        text.color = color;
    }

    public void OnClick()
    {
        tutCon.ContinueIfAllowed();
    }
    public void FadeIn()
    {
        LeanTween.cancel(gameObject);

        var color = text.color;
        color.a = 0f;
        text.color = color;
        var fadeInColor = color;
        fadeInColor.a = 1;

        LeanTween.value(gameObject, updateColorValue, color, fadeInColor, 0.7f).setIgnoreTimeScale(true).setOnComplete(FadeOutLocal);

        continueButton.enabled = true;
    }
    public void FadeOut()
    {

        LeanTween.cancel(gameObject);

        var color = text.color;
        var fadeInColor = color;
        fadeInColor.a = 0;

        float dist = text.color.a;

        LeanTween.value(gameObject, updateColorValue, color, fadeInColor, 0.2f * dist).setIgnoreTimeScale(true);

        continueButton.enabled = false;
    }

    void FadeOutLocal()
    {

        LeanTween.cancel(gameObject);

        var color = text.color;
        color.a = 1f;
        text.color = color;
        var fadeInColor = color;
        fadeInColor.a = 0;

        LeanTween.value(gameObject, updateColorValue, color, fadeInColor, 0.7f).setIgnoreTimeScale(true).setOnComplete(FadeIn);
    }

    public void FadeOutFinal()
    {

        LeanTween.cancel(gameObject);

        var color = text.color;
        var fadeInColor = color;
        fadeInColor.a = 0;

        float dist = text.color.a;

        LeanTween.value(gameObject, updateColorValue, color, fadeInColor, 0.2f * dist).setIgnoreTimeScale(true);

        gameObject.SetActive(false);
        this.enabled = false;
    }

    void updateColorValue(Color val)
    {
        text.color = val;
    }
}
