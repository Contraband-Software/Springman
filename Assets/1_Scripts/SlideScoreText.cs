using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;

public class SlideScoreText : MonoBehaviour
{
    public TextMeshProUGUI text;
    RectTransform textRect;
    public GameData gameData;

    public RectTransform bigTextAbove;
    TextMeshProUGUI bigTextAboveText;

    public enum TextType { HS, Score};
    public TextType textType;

    public float OpenDelay = 0.2f;

    public LeanTweenType easeIn;
    public LeanTweenType easeOut;

    public LeanTweenType fadeIn;
    public LeanTweenType fadeOut;

    public float yPosition;
    float yPosAdjust;
    public float bigTextY = -300f;
    float beforeDropPos;

    void Start()
    {
        gameData = GameObject.Find("GameController").GetComponent<GameData>();

        CloseButton closeButton = GameObject.Find("Options Menu/OptionsTab/CancelButton").GetComponent<CloseButton>();
        closeButton.OnOptionsExit += OnOptionsClose;

        text = GetComponent<TextMeshProUGUI>();
        textRect = text.gameObject.GetComponent<RectTransform>();

        if (textType == TextType.HS)
        {
            yPosAdjust = 0f;
        }
        else
        {
            yPosAdjust = 63f;
        }

        //textRect.anchoredPosition = new Vector2(0f, -300f - (bigTextAbove.rect.height * 0.4f * 0.5f) - (textRect.rect.height / 2f) - yPosAdjust);

        CalculateYPosition();

        var color = text.color;
        color.a = 0;
        text.color = color;
    }

    public void AddOnScore()
    {
        text.gameObject.GetComponent<TextLocaliserUI>().Localize();
        string puretext = text.text;

        

        if (textType == TextType.Score)
        {
            text.text = "";
            if (gameData.currentLanguage == "arabic")
            {
                puretext = puretext.Remove(puretext.Length - 1);

                string scoreBackwards = gameData.score.ToString();
                List<string> charList = new List<string>();
                for(int i = scoreBackwards.Length - 1; i >= 0; i--)
                {
                    charList.Add(scoreBackwards[i].ToString());
                }
                string scoreForwards = string.Join("", charList);

                text.text = puretext + " " + scoreForwards;
            }
            else
            {
                text.text = puretext + " " + gameData.score.ToString();
            }
        }
        if(textType == TextType.HS)
        {
            text.text = "";
            if (gameData.currentLanguage == "arabic")
            {
                puretext = puretext.Remove(puretext.Length - 1);

                string scoreBackwards = gameData.allTimeHighscore.ToString();
                List<string> charList = new List<string>();
                for (int i = scoreBackwards.Length - 1; i >= 0; i--)
                {
                    charList.Add(scoreBackwards[i].ToString());
                }
                string scoreForwards = string.Join("", charList);

                text.text = puretext + " " + scoreForwards;
            }
            else
            {
                text.text = puretext + " " + gameData.allTimeHighscore.ToString();
            }
        }
    }

    public void OnOptionsClose()
    {
        AddOnScore();

        try
        {
            bigTextAbove = transform.root.gameObject.transform.Find("Pause Text").GetComponent<RectTransform>();
        }
        catch
        {
            bigTextAbove = transform.root.gameObject.transform.Find("Death Text").GetComponent<RectTransform>();
        }

        bigTextAboveText = bigTextAbove.gameObject.GetComponent<TextMeshProUGUI>();

        string[] textWords = bigTextAboveText.text.Split(' ');

        if (textWords.Length > 1 && bigTextAboveText.fontSize < 325f)
        {
            textRect.anchoredPosition = new Vector2(0f, -300f - (bigTextAbove.rect.height * 0.4f * 0.5f) - (textRect.rect.height / 2f) - yPosAdjust - 20f);
        }
        else
        {
            textRect.anchoredPosition = new Vector2(0f, -300f - (bigTextAboveText.fontSize * 0.4f * 0.5f) - (textRect.rect.height / 2f) - yPosAdjust - 20f);

        }
    }

    private void CalculateYPosition()
    {

        AddOnScore();

        try
        {
            bigTextAbove = transform.root.gameObject.transform.Find("Pause Text").GetComponent<RectTransform>();
        }
        catch
        {
            bigTextAbove = transform.root.gameObject.transform.Find("Death Text").GetComponent<RectTransform>();
        }

        bigTextAboveText = bigTextAbove.gameObject.GetComponent<TextMeshProUGUI>();

        string[] textWords = bigTextAboveText.text.Split(' ');

        if (textWords.Length > 1 && bigTextAboveText.fontSize < 325f)
        {
            textRect.anchoredPosition = new Vector2(0f, -300f - (bigTextAbove.rect.height * 0.4f * 0.5f) - (textRect.rect.height / 2f) - yPosAdjust -20f);
        }
        else
        {
            textRect.anchoredPosition = new Vector2(0f, -300f - (bigTextAboveText.fontSize * 0.4f * 0.5f) - (textRect.rect.height / 2f) - yPosAdjust -20f);

        }

        yPosition = textRect.anchoredPosition.y;

        textRect = text.GetComponent<RectTransform>();
        beforeDropPos = textRect.anchoredPosition.y + 100f;
        textRect.anchoredPosition = new Vector2(0f, beforeDropPos);
    }

    public void Open()
    {
        StartCoroutine(OpenCoroutine());
    }

    public IEnumerator OpenCoroutine()
    {
        yield return new WaitForSecondsRealtime(OpenDelay);

        OpenMain();
    }

    public void OpenMain()
    {
        var color = text.color;
        var fadeInColor = color;
        fadeInColor.a = 1;

        CalculateYPosition();

        float startYPosition = textRect.anchoredPosition.y;
        float endYPosition = yPosition;

        LeanTween.value(gameObject, updatePositionValue, startYPosition, endYPosition, 0.2f).setIgnoreTimeScale(true).setEase(easeIn);
        LeanTween.value(gameObject, updateColorValue, color, fadeInColor, 0.2f).setIgnoreTimeScale(true).setEase(fadeIn);
    }

    public void Close()
    {
        var color = text.color;
        var fadeOutColor = color;
        fadeOutColor.a = 0;

        float startYPosition = textRect.anchoredPosition.y;
        float endYPosition = beforeDropPos;

        LeanTween.value(gameObject, updatePositionValue, startYPosition, endYPosition, 0.2f).setIgnoreTimeScale(true).setEase(easeOut);
        LeanTween.value(gameObject, updateColorValue, color, fadeOutColor, 0.2f).setIgnoreTimeScale(true).setEase(fadeOut);
    }

    void updatePositionValue(float yPosition)
    {
        textRect.anchoredPosition = new Vector2(0f, yPosition);
    }

    void updateColorValue(Color val)
    {
        text.color = val;
    }
}
