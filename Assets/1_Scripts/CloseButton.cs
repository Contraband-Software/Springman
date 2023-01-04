using Architecture;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CloseButton : MonoBehaviour
{
    public Canvas uiToClose;

    private Scene currentScene;

    public event OptionsExitEvent OnOptionsExit;
    public delegate void OptionsExitEvent();

    public void OnClick()
    {
        currentScene = SceneManager.GetActiveScene();

        uiToClose.gameObject.transform.GetChild(1).GetComponent<ScaleTween>().OnClose();

        UserGameData.Instance.SaveGameData();

        if (currentScene.name == "Game")
        {
            if (uiToClose.transform.root.gameObject.name == "Options Menu")
            {
                OnOptionsExit?.Invoke();
            }
        }

    }
}
