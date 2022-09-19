using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DeathAd : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] bool Enabled = false;
    [SerializeField] string AdUnit = "";
    [SerializeField, Min(0), Tooltip("How many games before an unrewarded ad is shown when the player dies")] int InterstitialAdFrequency = 0;

    AdvertisementsManager adManager;
    int GamesSinceAd = 0;

    private void Awake()
    {
        adManager = GetComponent<AdvertisementsManager>();
    }
    private void Start()
    {
        adManager.GetShowCompleteEvent(AdUnit).AddListener((bool status) =>
        {
            if (status)
            {
                GamesSinceAd = 0;
            }
        });
        //adManager.GetLoadCompleteEvent("ReviveAd").AddListener(() =>
        //{

        //});
    }

    public void Tick()
    {
        if (Enabled)
        {
            if (GamesSinceAd >= InterstitialAdFrequency)
            {
                adManager.PlayAd(AdUnit);
            } else
            {
                GamesSinceAd++;
            }
        }
    }
}
