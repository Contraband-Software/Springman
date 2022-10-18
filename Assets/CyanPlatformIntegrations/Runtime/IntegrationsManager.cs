using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformIntegrations
{
    public class IntegrationsManager : MonoBehaviour
    {
        [Header("Integration Controllers")]

        [SerializeField] AdvertisementsManager advertisementsManager;
        public AdvertisementsManager GetAdvertisements() { return advertisementsManager; }

#if UNITY_ANALYTICS
        [SerializeField] DataPrivacyHandler dataPrivacyHandler;
        public DataPrivacyHandler GetDataPrivacy() { return dataPrivacyHandler; }
#endif

        [Header("Platform Controllers")]

        [SerializeField] SocialManager SocialManager;
        public SocialManager GetGooglePlayGamesService() { return SocialManager; }


        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}