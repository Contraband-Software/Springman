﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformIntegrations
{
    public class IntegrationsManager : MonoBehaviour
    {
        [Header("Integration Controllers")]

        [SerializeField] AdvertisementsManager advertisementsManager;
        public AdvertisementsManager GetAdvertisements() { return advertisementsManager; }

//#if UNITY_ANALYTICS
        [SerializeField] DataPrivacyHandler dataPrivacyHandler;
        public DataPrivacyHandler GetDataPrivacy() { return dataPrivacyHandler; }
//#endif
        [SerializeField] InAppPurchases inAppPurchases;
        public InAppPurchases GetIAPManager() { return inAppPurchases; }

        [Header("Platform Controllers")]

        [SerializeField] SocialManager SocialManager;
        public SocialManager GetSocialManager() { return SocialManager; }


        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}