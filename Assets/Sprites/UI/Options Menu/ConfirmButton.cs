using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class ConfirmButton : MonoBehaviour
{
    public LanguageOrganiser langOrganiser;

    public GameData gameData;
    public MenuData menuData;

    Scene currentScene;

    public void OnClick()
    {
        if(langOrganiser.selectedLanguage != "")
        {
            if (currentScene.name == "Main Menu")
            {
                menuData.currentLanguage = langOrganiser.selectedLanguage;
                menuData.langIndex = Array.IndexOf(langOrganiser.languages, langOrganiser.selectedLanguage);

                LocalizationSystem.language = (LocalizationSystem.Language)menuData.langIndex;
                menuData.SaveGameData();
                menuData.ReLocalizeTexts();
            }



            if (currentScene.name == "Game")
            {
                gameData.currentLanguage = langOrganiser.selectedLanguage;
                gameData.langIndex = Array.IndexOf(langOrganiser.languages, langOrganiser.selectedLanguage);

                LocalizationSystem.language = (LocalizationSystem.Language)gameData.langIndex;
                gameData.SaveGameData();
                gameData.ReLocalizeTexts();
            }
        }


    }

    private void Start()
    {
        currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "Main Menu")
        {
            menuData = GameObject.Find("MenuController").GetComponent<MenuData>();
        }
        if (currentScene.name == "Game")
        {
            gameData = GameObject.Find("GameController").GetComponent<GameData>();
        }
    }
}
