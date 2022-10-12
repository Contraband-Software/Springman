using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] GooglePlayGamesManager googlePlayGamesManager;
    public GooglePlayGamesManager GetGooglePlayGamesService() { return googlePlayGamesManager; }


    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
