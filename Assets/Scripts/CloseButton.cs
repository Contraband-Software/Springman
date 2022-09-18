using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CloseButton : MonoBehaviour
{
    public Canvas uiToClose;
    public MenuData menuData;
    public GameData gameData;

    private Scene currentScene;

    public event OptionsExitEvent OnOptionsExit;
    public delegate void OptionsExitEvent();

    public void OnClick()
    {
        currentScene = SceneManager.GetActiveScene();

        uiToClose.gameObject.transform.GetChild(1).GetComponent<ScaleTween>().OnClose();

        if (currentScene.name == "Main Menu")
        {
            menuData = GameObject.Find("MenuController").GetComponent<MenuData>();
            menuData.SaveGameData();
        }
        if (currentScene.name == "Game")
        {
            gameData = GameObject.Find("GameController").GetComponent<GameData>();
            gameData.SaveGameData();

            if (uiToClose.transform.root.gameObject.name == "Options Menu")
            {
                OnOptionsExit?.Invoke();
            }
        }

    }
}
