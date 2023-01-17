using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PlatformIntegrations;

public class HomeButton : MonoBehaviour
{
    public CanvasGroup curtainCG;

    private AdvertisementsManager adManager;

    private void Start()
    {
        adManager = IntegrationsManager.Instance.advertisementsManager;
    }

    public void OnClick()
    {
        Architecture.Managers.UserGameData.Instance.SaveGameData();

        //AdvertisementsManager.HideBannerAd();
        adManager.HideBannerAd();

        LeanTween.alphaCanvas(curtainCG, 1f, 0.2f).setIgnoreTimeScale(true);
        StartCoroutine(LoadMainMenu());
    }

    IEnumerator LoadMainMenu()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        Time.timeScale = 1;
        SceneManager.LoadScene("Main Menu");
    }
}





