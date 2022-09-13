﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeButton : MonoBehaviour
{
    public GameData gameData;
    public CanvasGroup curtainCG;

    public void OnClick()
    {
        gameData.SaveGameData();

        //AdvertisementsManager.instance.HideBannerAd();

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





