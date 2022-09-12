using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReviveController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Button button;
    [Header("Settings")]
    [SerializeField] int MaxRevives = 1;

    int ReviveCount = 0;

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
        AdvertisementsManager.instance.PlayAd(AdvertisementsManager.AdType.REWARDED, (bool success) =>
        {
            if (success)
            {
                ReviveIncrement();
                Debug.Log("Successful revive");
                //DeathScreenScript.instance.DeathScreenHide();

                //player.revive();
            }
        });
    }
}
