using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PlatformIntegrations;

public class ReplayButton : MonoBehaviour
{
    private AdvertisementsManager adManager;

    private void Start()
    {
        adManager = IntegrationsManager.Instance.advertisementsManager;
    }

    public void OnClick()
    {
        Architecture.Managers.UserGameData.Instance.SaveGameData();
        adManager.HideBannerAd();
        SceneManager.LoadScene("Game");
        Time.timeScale = 1;
    }
}
