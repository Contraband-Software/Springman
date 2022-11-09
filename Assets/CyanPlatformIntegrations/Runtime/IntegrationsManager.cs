using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformIntegrations
{
    public class IntegrationsManager : MonoBehaviour
    {
        //Ensure this is the only instance
        private static Tuple<bool, int> instanceID;

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


        void Awake()
        {
            DontDestroyOnLoad(gameObject);

            if (instanceID == null)
            {
                instanceID = new Tuple<bool, int>(true, gameObject.GetInstanceID());
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public static IntegrationsManager GetObject() {
            return GameObject.FindGameObjectWithTag("Integrations")
                .GetComponent<IntegrationsManager>();
        }
    }
}