using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using System;

public class AdvertisementsManager : MonoBehaviour, IUnityAdsInitializationListener
{
    [Serializable]
    public class AdUnit : IUnityAdsLoadListener, IUnityAdsShowListener
    {
        public string Name;

        public string Android;
        public string IOS;
        public bool Banner;
        private BannerLoadOptions bannerLoadOptions;
        private BannerOptions bannerShowOptions;

        private string AdID;

        private bool Loaded;
        public bool IsLoaded()
        {
            return Loaded;
        }
        private void Load()
        {
            if (Banner)
            {
                Advertisement.Banner.Load(AdID, bannerLoadOptions);
            } else
            {
                Advertisement.Load(AdID, this);
            }
        }

        public void Init()
        {
#if UNITY_IOS
            AdID = IOS;
#elif UNITY_ANDROID
            AdID = Android;
#endif

            if (Banner)
            {
                bannerLoadOptions = new BannerLoadOptions
                {
                    loadCallback = () =>
                    {
                        Loaded = true;
                    },
                    errorCallback = (string err) =>
                    {
                        Debug.Log(err);
                        Advertisement.Banner.Load(AdID);
                    }
                };

                bannerShowOptions = new BannerOptions
                {
                    clickCallback = () =>
                    {

                    },
                    hideCallback = () =>
                    {
                        AdvertisementsManager.HideBannerAd();
                    },
                    showCallback = () =>
                    {

                    }
                };
            }

            Loaded = false;
            Load();

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

            isPlaying = false;
        }

        private bool isPlaying;
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
            if (Banner)
            {
                Advertisement.Banner.Show(AdID, bannerShowOptions);
            }
            else
            {
                Advertisement.Show(AdID, this);
            }

            Loaded = false;
        }

        public void OnUnityAdsAdLoaded(string adUnitId)
        {
            if (adUnitId.Equals(AdID))
            {
                LoadedCallback();
                Debug.Log(adUnitId + " IS LOADED");
                Loaded = true;
            }
        }

        public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
        {
            if (adUnitId.Equals(AdID) && isPlaying)
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

                isPlaying = false;

                // Load another ad:
                Load();
            }
        }

        // Implement Load and Show Listener error callbacks:
        public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
        {
            if (adUnitId.Equals(AdID))
            {
                Loaded = false;
                Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
                Load();
            }
        }

        public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
        {
            if (adUnitId.Equals(AdID))
            {
                Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
                // Use the error details to determine whether to try to load another ad.
                CompleteCallback(false);
                Load();

                isPlaying = false;
            }
        }

        public void OnUnityAdsShowStart(string adUnitId)
        {
            isPlaying = true;
        }
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

    private AdUnit GetAdUnitByName(string name)
    {
        for (int i = 0; i < AdUnits.Count; i++)
        {
            if (AdUnits[i].Name == name)
            {
                return AdUnits[i];
            }
        }

        return null;
    }

    public void PlayAd(string name)
    {
        //if (index > 0 && index < AdUnits.Count)
        //{
        GetAdUnitByName(name).ShowAd();
        //}
    }
    public static void HideBannerAd()
    {
        Advertisement.Banner.Hide();
    }
    public void RegisterLoadCallback(string name, Action callback)
    {
        //if (index > 0 && index < AdUnits.Count)
        //{
            GetAdUnitByName(name).SetLoadedCallback(callback);
        //}
    }
    public void RegisterCompletionCallback(string name, Action<bool> completionCallback)
    {
        //if (index > 0 && index < AdUnits.Count)
        //{
        GetAdUnitByName(name).SetCompleteCallback(completionCallback);
        //}
    }
    public bool GetLoadedStatus(string name)
    {
        //if (index > 0 && index < AdUnits.Count)
        //{
            return GetAdUnitByName(name).IsLoaded();
        //}

        //return false;
    }

    private void Awake()
    {
        InitializeManager();

        Advertisement.Banner.SetPosition(BannerAdPosition);

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
        InitializeAdUnits();
    }
    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }
}