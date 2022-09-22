using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntegrationsManager : MonoBehaviour
{
    [Header("Integration Controllers")]
    [SerializeField] AdvertisementsManager advertisementsManager;
    [SerializeField] DataPrivacyHandler dataPrivacyHandler;

    public AdvertisementsManager GetAdvertisements() { return advertisementsManager; }
    public DataPrivacyHandler GetDataPrivacy() { return dataPrivacyHandler; }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
