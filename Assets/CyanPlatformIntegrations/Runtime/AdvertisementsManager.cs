using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

namespace PlatformIntegrations
{
    using UnityEngine.Advertisements;

    public class AdvertisementsManager : IUnityAdsInitializationListener
    {
        #region CONFIG
        public const string logDecorator = Config.globalLogDecorator + "Advertisements: ";

        [Serializable]
        public class AdUnit : IUnityAdsLoadListener, IUnityAdsShowListener
        {
            [SerializeField] string Name;

            [SerializeField] string Android;
            [SerializeField] string IOS;
            [SerializeField] bool Banner;

            private BannerLoadOptions bannerLoadOptions;
            private BannerOptions bannerShowOptions;

            private string AdID;

            private bool Loaded;

            public string GetName()
            {
                return Name;
            }

            public bool IsLoaded()
            {
                return Loaded;
            }
            private void Load()
            {
                if (Banner)
                {
                    Advertisement.Banner.Load(AdID, bannerLoadOptions);
                }
                else
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
                            OnLoadComplete.Invoke();
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
                            //HideBannerAd();
                        },
                        showCallback = () =>
                        {

                        }
                    };
                }

                Loaded = false;
                Load();

                isPlaying = false;
            }

            private bool isPlaying;
            public bool GetPlaying()
            {
                return isPlaying;
            }

            public class OnShowCompleteEvent : UnityEvent<bool> { };
            [HideInInspector] public OnShowCompleteEvent OnShowComplete = new OnShowCompleteEvent();
            [HideInInspector] public UnityEvent OnLoadComplete = new UnityEvent();

            public void ShowAd()
            {
                if (Loaded)
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
            }

            public void OnUnityAdsAdLoaded(string adUnitId)
            {
                if (adUnitId.Equals(AdID))
                {
                    //LoadedCallback();
                    OnLoadComplete.Invoke();
                    //Debug.Log(adUnitId + " IS LOADED");
                    Loaded = true;
                }
            }

            public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
            {
                //Debug.LogWarning("OnUnityAdsShowComplete");
                //Debug.LogWarning(isPlaying);
                if (adUnitId.Equals(AdID) && isPlaying)
                {
                    //Debug.LogWarning("adUnitId.Equals(AdID) && isPlaying");
                    if (showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
                    {
                        //Debug.LogWarning("Unity Ads Rewarded Ad Completed");
                        // Grant a reward.
                        OnShowComplete.Invoke(true);
                    }
                    else
                    {
                        OnShowComplete.Invoke(false);
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
                    Debug.LogWarning(logDecorator + $"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
                    Load();
                }
            }

            public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
            {
                if (adUnitId.Equals(AdID))
                {
                    Debug.Log(logDecorator + $"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
                    // Use the error details to determine whether to try to load another ad.
                    OnShowComplete.Invoke(false);
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

        string AppleGameID;
        string GooglePlayGameID;
        List<AdUnit> AdUnits;
        BannerPosition BannerAdPosition;
        bool TestMode = false;
        #endregion

        #region STATE
        private string GameID;
        #endregion

        private AdUnit GetAdUnitByName(string name)
        {
            for (int i = 0; i < AdUnits.Count; i++)
            {
                if (AdUnits[i].GetName() == name)
                {
                    return AdUnits[i];
                }
            }

            return null;
        }

        public void PlayAd(string name)
        {
            if (Advertisement.isInitialized)
            {
                GetAdUnitByName(name).ShowAd();
            } else
            {
                Debug.Log(logDecorator + "AD NOT INITIALIZED: " + name);
            }
        }
        public void HideBannerAd()
        {
            Advertisement.Banner.Hide();
        }

        public AdUnit.OnShowCompleteEvent GetShowCompleteEvent(string name)
        {
            return GetAdUnitByName(name).OnShowComplete;
        }
        public UnityEvent GetLoadCompleteEvent(string name)
        {
            return GetAdUnitByName(name).OnLoadComplete;
        }
        public bool GetLoadedStatus(string name)
        {
            return GetAdUnitByName(name).IsLoaded();
        }

        public AdvertisementsManager(
            string appleGameID, string gpgsID, 
            List<AdUnit> AdUnits, 
            BannerPosition BannerAdPosition,
            bool testMode
        ) {
            this.AppleGameID = appleGameID;
            this.GooglePlayGameID = gpgsID;
            this.AdUnits = AdUnits;
            this.BannerAdPosition = BannerAdPosition;
            this.TestMode = testMode;

            InitializeManager();
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
            Debug.Log(logDecorator + "Unity Ads initialization complete.");

            Advertisement.Banner.SetPosition(BannerAdPosition);

            InitializeAdUnits();
        }
        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            Debug.Log(logDecorator + $"Unity Ads Initialization Failed: {error.ToString()} - {message}");

            Advertisement.Initialize(GameID, TestMode, this);
        }
    }
}