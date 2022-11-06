using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlatformIntegrations;

public class DeathScreenScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Canvas DeathScreenCanvas;
    [SerializeField] CanvasGroup filter;
    [SerializeField] CanvasGroup gameCanvas;
    [SerializeField] SlideScoreText sstHighscore;
    [SerializeField] SlideScoreText sstScore;
    [SerializeField] Canvas PauseScreenCanvas;
    //[SerializeField] GameObject GameCanvas;

    //[Header("Settings")]
    //[SerializeField, Min(0), Tooltip("How many games before an unrewarded ad is shown when the player dies")] int InterstitialAdFrequency;

    AdvertisementsManager adManager;
    SocialManager socialManager;

    PlayerController pcontroller;

    bool DeathScreenShowing = false;

    void Start()
    {
        //GameObject Money = GameObject.FindGameObjectWithTag("AdvertisementsManager");
        adManager = GameObject.FindGameObjectWithTag("Integrations")
            .GetComponent<IntegrationsManager>().GetAdvertisements();

        socialManager = GameObject.FindGameObjectWithTag("Integrations")
            .GetComponent<IntegrationsManager>().GetSocialManager();
        //deathAd = Money.GetComponent<DeathAd>();

        pcontroller = GameObject.Find("Player").GetComponent<PlayerController>();
        DeathScreenCanvas.enabled = false;

        pcontroller.revive_Reassign += ReassignPCon;
    }

    private void ReassignPCon(PlayerController pCon)
    {
        pcontroller = pCon;
        pcontroller.revive_Reassign += ReassignPCon;
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

        adManager.HideBannerAd();

        DeathScreenShowing = false;
    }

    public void DeathScreenShow(int finalScore)
    {
        if (DeathScreenShowing == false && DeathScreenCanvas.enabled == false)
        {
            if (PauseScreenCanvas.enabled)
            {
                PauseScreenCanvas.enabled = false;
            }

            //evil video death ad
            //deathAd.Tick();
            adManager.PlayAd("DeathBanner");

            socialManager.PostLeaderboardScore(finalScore, (bool status) =>
            {
                Debug.Log("GPGS: Post leaderboard score status: " + status.ToString());
            });

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
