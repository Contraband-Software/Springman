//using GooglePlayGames.BasicApi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReviveController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Button button;
    [SerializeField] Image buttonImage;
    [SerializeField] PlayerController playerController;
    [SerializeField] DeathScreenScript deathScreenManager;
    [SerializeField] ReviveCountdownScript reviveCountdownScript;
    [Header("Settings")]
    [SerializeField] int MaxRevives = 1;

    private AdvertisementsManager adManager;

    int ReviveCount = 0;

    private void Start()
    {
        playerController.revive_Reassign += ReassignPCon;

        adManager = GameObject.FindGameObjectWithTag("Integrations")
            .GetComponent<IntegrationsManager>().GetAdvertisements();

        adManager.GetShowCompleteEvent("ReviveAd").AddListener((bool status) =>
        {
            if (status)
            {
                ReviveIncrement();
                playerController.Revive();
                reviveCountdownScript.BeginCountdown();
                deathScreenManager.DeathScreenHide();
            }
            else
            {
                Debug.LogWarning("revive fail");
            }
        });
        adManager.GetLoadCompleteEvent("ReviveAd").AddListener(() =>
        {
            if (ReviveCount < MaxRevives)
            {
                ToggleButton(true);
            }
        });
        ToggleButton(adManager.GetLoadedStatus("ReviveAd"));
    }

    private void ReassignPCon(PlayerController pCon)
    {
        playerController = pCon;
        playerController.revive_Reassign += ReassignPCon;
    }

    public int GetReviveCount()
    {
        return ReviveCount;
    }
    public int GetMaxRevives()
    {
        return MaxRevives;
    }

    void ReviveIncrement()
    {
        ReviveCount++;
        if (ReviveCount > MaxRevives)
        {
            ToggleButton(false);
        }
    }

    public void AdRevive()
    {
        Debug.Log("User tried to revive");

        adManager.PlayAd("ReviveAd");
        ToggleButton(false);
    }

    private void ToggleButton(bool status)
    {
        button.interactable = status;
        buttonImage.enabled = status;
    }
}
