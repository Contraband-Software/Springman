using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReplayButton : MonoBehaviour
{
    public GameData gameData;

    private AdvertisementsManager adManager;

    private void Start()
    {
        adManager = GameObject.FindGameObjectWithTag("Integrations")
            .GetComponent<IntegrationsManager>().GetAdvertisements();
    }

    public void OnClick()
    {
        gameData.SaveGameData();
        adManager.HideBannerAd();
        SceneManager.LoadScene("Game");
        Time.timeScale = 1;
    }
}
