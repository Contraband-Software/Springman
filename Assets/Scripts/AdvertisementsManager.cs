using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using System;

public class AdvertisementsManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public static AdvertisementsManager instance;

    [Serializable]
    public struct AdUnit
    {
        public string Android;
        public string IOS;
    }

    [Header("Unity Ads game IDs")]
    [SerializeField] string AppleGameID;
    [SerializeField] string GooglePlayGameID;

    [Header("Unity Ad Units")]
    [SerializeField] AdUnit InterstitialAd;
    [SerializeField] AdUnit RewardedAd;
    [SerializeField] AdUnit BannerAd;

    [Header("Settings")]
    [SerializeField] BannerPosition BannerAdPosition;
    [SerializeField, Tooltip("WARNING: THIS IS FOR FRAUD PREVENTION, IT MEANS NO REVENUE IS EARNED FROM ADS")] bool TestMode = false;

    string GameID;

    Action<bool> ActionCallback;

    public enum AdType
    {
        INTERSTITIAL,
        REWARDED,
        BANNER
    }

    BannerLoadOptions bannerOptions = new BannerLoadOptions
    {
        loadCallback = delegate()
        {
            Advertisement.Banner.Show("Banner_Android");
        },
        errorCallback = delegate(string err)
        {
            Debug.Log($"Banner Error: {err}");
        }
    };

    void Awake()
    {
#if UNITY_IOS
        GameID = AppleGameID;
#elif UNITY_ANDROID
        GameID = GooglePlayGameID;
#endif
        Advertisement.Initialize(GameID, TestMode, this);

        Advertisement.Banner.SetPosition(BannerAdPosition);

        DontDestroyOnLoad(transform.gameObject);

        if (!instance)
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //Initialization callbacks
    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads has initialized");

        //PlayAd(AdType.BANNER, delegate (bool status) {});
    }
    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }

    public void PlayAd(AdType type, Action<bool> callback)
    {
        if (!Advertisement.isInitialized)
        {
            callback(false);
            return;
        }

#if UNITY_IOS
        switch (type)
        {
            case AdType.INTERSTITIAL:
                Advertisement.Load(InterstitialAd.IOS, this);
                break;
            case AdType.REWARDED:
                Advertisement.Load(RewardedAd.IOS, this);
                break;
            case AdType.BANNER:
                Advertisement.Banner.Load(BannerAd.IOS, bannerOptions);
                break;
        }
#elif UNITY_ANDROID
        switch (type)
        {
            case AdType.INTERSTITIAL:
                Advertisement.Load(InterstitialAd.Android, this);
                break;
            case AdType.REWARDED:
                Advertisement.Load(RewardedAd.Android, this);
                break;
            case AdType.BANNER:
                Advertisement.Banner.Load(BannerAd.Android, bannerOptions);
                break;
        }
#endif

        ActionCallback = callback;
    }
    public void HideBannerAd()
    {
        Advertisement.Banner.Hide();
    }

    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("Ad Loaded: " + adUnitId);
        Advertisement.Show(adUnitId, this);
    }

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("Unity Ads Rewarded Ad Completed");

            ActionCallback(true);
        }
    }
    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
        ActionCallback(false);
    }

    // Implement Load and Show Listener error callbacks:
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
        ActionCallback(false);
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }

}
