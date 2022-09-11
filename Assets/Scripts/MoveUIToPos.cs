using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveUIToPos : MonoBehaviour
{
    RectTransform rect;
    public Vector2 desiredPos;
    public Vector2 startPos;

    public LeanTweenType easeIn;
    public LeanTweenType easeOut;

    public float timeIn;
    public float timeOut;

    void Start()
    {
        rect = gameObject.GetComponent<RectTransform>();
        desiredPos = rect.anchoredPosition;

        rect.anchoredPosition = startPos;

        PauseButton pauseButton = GameObject.Find("Canvas/PauseButton").GetComponent<PauseButton>();
        pauseButton.OnPause += OnPauseOpen;

        ResumeButton resumeButton = GameObject.Find("PauseMenu/Canvas/ResumeButton").GetComponent<ResumeButton>();
        resumeButton.OnResume += OnResumeClose;

        PlayerController playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        playerController.OnDeath += OnDeathOpen;

    }

    public void OnPauseOpen()
    {
        LeanTween.value(gameObject, ChangeX, startPos.x, desiredPos.x, timeIn).setIgnoreTimeScale(true).setEase(easeIn);
        LeanTween.value(gameObject, ChangeY, startPos.y, desiredPos.y, timeIn).setIgnoreTimeScale(true).setEase(easeIn);
    }

    public void OnResumeClose()
    {
        LeanTween.value(gameObject, ChangeX, rect.anchoredPosition.x, startPos.x, timeOut).setIgnoreTimeScale(true).setEase(easeIn);
        LeanTween.value(gameObject, ChangeY, rect.anchoredPosition.y, startPos.y, timeOut).setIgnoreTimeScale(true).setEase(easeIn);
    }

    public void OnDeathOpen()
    {
        LeanTween.value(gameObject, ChangeX, startPos.x, desiredPos.x, timeIn).setIgnoreTimeScale(true).setEase(easeIn);
        LeanTween.value(gameObject, ChangeY, startPos.y, desiredPos.y, timeIn).setIgnoreTimeScale(true).setEase(easeIn);
    }

    public void ChangeX(float x)
    {
        rect.anchoredPosition = new Vector2(x, rect.anchoredPosition.y); 
    }
    public void ChangeY(float y)
    {
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, y);
    }
}
