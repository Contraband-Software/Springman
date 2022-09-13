using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using System;

public class AdvertisementsManager : MonoBehaviour, IUnityAdsInitializationListener//, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [Serializable]
    public class AdUnit : IUnityAdsLoadListener, IUnityAdsShowListener
    {
        public string Android;
        public string IOS;
        public bool Banner;

        private string AdID;

        private bool Loaded;
        public bool IsLoaded()
        {
            return Loaded;
        }

        public void Init()
        {
#if UNITY_IOS
            AdID = IOS;
#elif UNITY_ANDROID
            AdID = Android;
#endif
            Loaded = false;
            Advertisement.Load(AdID, this);

            LoadedCallback = () => {
                Debug.Log(AdID + " has been loaded");
            };

            CompleteCallback = (bool status) => {
                if (status)
                {
                    Debug.Log(AdID + " has been successful");
                } else
                {
                    Debug.Log(AdID + " has been unsuccessful");
                }
            };
        }

        private bool isPlaying;
        public void SetPlaying(bool status)
        {
            isPlaying = status;
        }
        public bool GetPlaying()
        {
            return isPlaying;
        }

        private Action<bool> CompleteCallback;
        public void SetCompleteCallback(Action<bool> action)
        {
            CompleteCallback = action;
        }
        public Action<bool> GetCompleteCallback()
        {
            return CompleteCallback;
        }
        private Action LoadedCallback;
        public void SetLoadedCallback(Action action)
        {
            LoadedCallback = action;
        }
        public Action GetAction()
        {
            return LoadedCallback;
        }

        public void ShowAd()
        {
            Advertisement.Show(AdID, this);

            Loaded = false;
        }

        public void OnUnityAdsAdLoaded(string adUnitId)
        {
            Debug.Log("Ad Loaded: " + adUnitId);

            if (adUnitId.Equals(AdID))
            {
                LoadedCallback();
                Debug.Log(adUnitId + " IS LOADED");
                Loaded = true;
            }
        }

        public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
        {
            if (adUnitId.Equals(AdID))
            {
                if (showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
                {
                    Debug.Log("Unity Ads Rewarded Ad Completed");
                    // Grant a reward.
                    CompleteCallback(true);
                } else
                {
                    CompleteCallback(false);
                }

                // Load another ad:
                Advertisement.Load(AdID, this);
            }
        }

        // Implement Load and Show Listener error callbacks:
        public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
        {
            Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
            Advertisement.Load(AdID, this);
        }

        public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
        {
            Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
            // Use the error details to determine whether to try to load another ad.
            CompleteCallback(false);
            Advertisement.Load(AdID, this);
        }

        public void OnUnityAdsShowStart(string adUnitId) { }
        public void OnUnityAdsShowClick(string adUnitId) { }
    }

    [Header("Unity Ads game IDs")]
    [SerializeField] string AppleGameID;
    [SerializeField] string GooglePlayGameID;

    [Header("Ad Units")]
    [SerializeField] List<AdUnit> AdUnits;

    [Header("Settings")]
    [SerializeField] BannerPosition BannerAdPosition;
    [SerializeField, Tooltip("WARNING: THIS IS FOR FRAUD PREVENTION, IT MEANS NO REVENUE IS EARNED FROM ADS")] bool TestMode = false;


    private string GameID;

    public void PlayAd(int index)
    {
        if (index > 0 && index < AdUnits.Count)
        {
            AdUnits[index].ShowAd();
        }
    }
    public void RegisterLoadCallback(int index, Action callback)
    {
        if (index > 0 && index < AdUnits.Count)
        {
            AdUnits[index].SetLoadedCallback(callback);
        }
    }
    public void RegisterCompletionCallback(int index, Action<bool> completionCallback)
    {
        if (index > 0 && index < AdUnits.Count)
        {
            AdUnits[index].SetCompleteCallback(completionCallback);
        }
    }
    public bool GetLoadedStatus(int index)
    {
        if (index > 0 && index < AdUnits.Count)
        {
            return AdUnits[index].IsLoaded();
        }

        return false;
    }

    private void Awake()
    {
        InitializeManager();
        InitializeAdUnits();
        DontDestroyOnLoad(transform.gameObject);
    }

    public void InitializeManager()
    {
#if UNITY_IOS
        GameID = AppleGameID;
#elif UNITY_ANDROID
        GameID = GooglePlayGameID;
#endif
        Advertisement.Initialize(GameID, TestMode, this);

        Advertisement.Banner.SetPosition(BannerAdPosition);
    }
    private void InitializeAdUnits()
    {
        for (int i = 0; i < AdUnits.Count; i++)
        {
            AdUnits[i].Init();
        }
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
    }
    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }
}