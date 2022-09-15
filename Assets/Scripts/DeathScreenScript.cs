using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeathScreenScript : MonoBehaviour
{
    public static DeathScreenScript instance;

    public Canvas DeathScreenCanvas;
    public PlayerController pcontroller;
    public CanvasGroup filter;
    public CanvasGroup gameCanvas;
    public SlideScoreText sstHighscore;
    public SlideScoreText sstScore;
    public Canvas PauseScreenCanvas;
    public GameObject GameCanvas;

    AdvertisementsManager adManager;

    bool DeathScreenShowing = false;

    void Start()
    {
        if (!instance)
        {
            instance = this;
        }

        adManager = GameObject.FindGameObjectWithTag("AdvertisementsManager").GetComponent<AdvertisementsManager>();

        pcontroller = GameObject.Find("Player").GetComponent<PlayerController>();
        DeathScreenCanvas.enabled = false;
    }

    public void DeathScreenHide()
    {
        if (DeathScreenShowing == true)
        {
            gameCanvas.gameObject.SetActive(true);
            DeathScreenCanvas.enabled = false;

            DeathScreenCanvas.gameObject.transform.GetChild(1).GetComponent<ScaleTween>().OnClose();
            filter.gameObject.GetComponent<FilterFade>().FadeToClear();

            //gameObject.GetComponent<DeathText>().OnDeath();

            LeanTween.alphaCanvas(gameCanvas, 1f, 0.2f);

            sstHighscore.Close();
            sstScore.Close();
        }

        DeathScreenShowing = false;
    }

    public void DeathScreenShow()
    {
        if (DeathScreenShowing == false && DeathScreenCanvas.enabled == false)
        {
            if (PauseScreenCanvas.enabled)
            {
                PauseScreenCanvas.enabled = false;
            }

            //evil video death ad
            //adManager.PlayAd("DeathInterstitial");
            adManager.PlayAd("DeathBanner");

            gameCanvas.gameObject.SetActive(false);
            DeathScreenCanvas.enabled = true;
            DeathScreenCanvas.gameObject.transform.GetChild(1).GetComponent<ScaleTween>().OnOpen();
            filter.gameObject.GetComponent<FilterFade>().FadeToBlack();
            gameObject.GetComponent<DeathText>().OnDeath();
            LeanTween.alphaCanvas(gameCanvas, 0f, 0.2f).setIgnoreTimeScale(true);

            sstHighscore.Open();
            sstScore.Open();
        }

        DeathScreenShowing = true;
    }
}
