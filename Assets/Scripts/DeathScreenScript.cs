using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeathScreenScript : MonoBehaviour
{
    public Canvas DeathScreenCanvas;
    public PlayerController pcontroller;
    public CanvasGroup filter;
    public CanvasGroup gameCanvas;
    public SlideScoreText sstHighscore;
    public SlideScoreText sstScore;
    public Canvas PauseScreenCanvas;
    public GameObject GameCanvas;

    void Start()
    {
        pcontroller = GameObject.Find("Player").GetComponent<PlayerController>();
        DeathScreenCanvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        DeathScreenShow();
    }

    void DeathScreenShow()
    {
        if (DeathScreenCanvas.enabled == false && pcontroller.state == PlayerController.State.Dead)
        {
            if (PauseScreenCanvas.enabled)
            {
                PauseScreenCanvas.enabled = false;
            }

            gameCanvas.gameObject.SetActive(false);
            DeathScreenCanvas.enabled = true;
            DeathScreenCanvas.gameObject.transform.GetChild(1).GetComponent<ScaleTween>().OnOpen();
            filter.gameObject.GetComponent<FilterFade>().FadeToBlack();
            gameObject.GetComponent<DeathText>().OnDeath();
            LeanTween.alphaCanvas(gameCanvas, 0f, 0.2f).setIgnoreTimeScale(true);

            sstHighscore.Open();
            sstScore.Open();
        }
    }
}
