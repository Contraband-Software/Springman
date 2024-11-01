﻿using GooglePlayGames;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using static PlatformIntegrations.AdvertisementsManager;

namespace PlatformIntegrations
{
    internal static class Config
    {
        public const string globalLogDecorator = "PLATFORM_INTEGRATIONS::";
    }

    public sealed class IntegrationsManager : MonoBehaviour
    {
        public static IntegrationsManager Instance { get; private set; } = null;

        public SocialManager socialManager { get; private set; } = null;
        public InAppPurchases iapHandler { get; private set; } = null;
        public AdvertisementsManager advertisementsManager { get; private set; } = null;
        public DataPrivacyHandler dataPrivacyHandler { get; private set; } = null;

        [Header("Enable Integrations")]
        [SerializeField, Tooltip("Enable Google Play Games / iOS services for cloud save games and leaderboards")] bool Social;
        [SerializeField] bool InAppPurchases;
        [SerializeField] bool Advertising;
        
        [Header("Advertisement Setup")]
        [SerializeField] string AppleGameID;
        [SerializeField] string GooglePlayGameID;
        [SerializeField] List<AdUnit> AdUnits;
        [SerializeField] BannerPosition BannerAdPosition;
        [SerializeField, Tooltip("WARNING: THIS IS FOR FRAUD PREVENTION, IT MEANS NO REVENUE IS EARNED FROM ADS")] bool TestMode = false;

        void Awake()
        {
#region PREVENT_DUPLICATES
            DontDestroyOnLoad(gameObject);

            if (Instance == null)
            {
                Instance = this;

                Initialize();
            }
            else
            {
                Destroy(gameObject);
            }
#endregion
        }

        void Initialize()
        {
            Debug.Log(Config.globalLogDecorator + "Main: Initializing Integrations");

            if (Social) { socialManager = new SocialManager(); }
            if (InAppPurchases) { iapHandler = new InAppPurchases(); }
            if (Advertising) { advertisementsManager = new AdvertisementsManager(AppleGameID, GooglePlayGameID, AdUnits, BannerAdPosition, TestMode); }
            if (Advertising || InAppPurchases) { dataPrivacyHandler = new DataPrivacyHandler(); }
        }

        private void OnApplicationFocus(bool focus)
        {
            if (dataPrivacyHandler != null)
            {
                dataPrivacyHandler.OnApplicationFocus(focus);
            }
        }
    }
}