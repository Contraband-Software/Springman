using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReviveController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Button button;
    [SerializeField] PlayerController playerController;
    [Header("Settings")]
    [SerializeField] int MaxRevives = 1;

    private AdvertisementsManager adManager;

    int ReviveCount = 0;

    private void Start()
    {
        adManager = GameObject.FindGameObjectWithTag("AdvertisementsManager").GetComponent<AdvertisementsManager>();
        //adManager.RegisterCompletionCallback("ReviveAd", (bool status) => {
        //    if (status)
        //    {
        //        ReviveIncrement();
        //        playerController.Revive();
        //        DeathScreenScript.instance.DeathScreenHide();
        //    }
        //    else
        //    {
        //        Debug.LogWarning("revive fail");
        //    }
        //});
        //adManager.RegisterLoadCallback("ReviveAd", () =>
        //{
        //    if (ReviveCount < MaxRevives)
        //    {
        //        button.interactable = true;
        //    }
        //});
        //button.interactable = adManager.GetLoadedStatus("ReviveAd");
        adManager.GetShowCompleteEvent("ReviveAd").AddListener((bool status) =>
        {
            if (status)
            {
                ReviveIncrement();
                playerController.Revive();
                DeathScreenScript.instance.DeathScreenHide();
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
                button.interactable = true;
            }
        });
        button.interactable = adManager.GetLoadedStatus("ReviveAd");
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
            button.interactable = false;
        }
    }

    public void AdRevive()
    {
        //AdvertisementsManager.instance.PlayAd(AdvertisementsManager.AdType.REWARDED, (bool success) =>
        //{
        //    if (success)
        //    {
        //        ReviveIncrement();
        //        Debug.Log("Successful revive");
        //        //DeathScreenScript.instance.DeathScreenHide();

        //        //player.revive();
        //        playerController.Revive();
        //        DeathScreenScript.instance.DeathScreenHide();
        //    } else
        //    {
        //        Debug.Log("Unsuccessful revive");
        //    }
        //});

        Debug.Log("User tried to revive");

        adManager.PlayAd("ReviveAd");
        button.interactable = false;
    }
}
