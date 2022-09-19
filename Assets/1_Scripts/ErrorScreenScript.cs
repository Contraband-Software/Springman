using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ErrorScreenScript : MonoBehaviour
{
    public Canvas errorCanvas;
    public GameData gameData;
    public MenuData menuData;
    public bool displayError = false;
    Scene scene;
    private void Start()
    {
        errorCanvas.enabled = false;
        displayError = false;

        scene = SceneManager.GetActiveScene();

        if(scene.name == "Main Menu")
        {
            menuData = GameObject.Find("MenuController").GetComponent<MenuData>();
        }
        if(scene.name == "Game")
        {
            gameData = GameObject.Find("GameController").GetComponent<GameData>();
        }

        DisplayError();
    }

    private void Update()
    {
        DisplayError();
    }

    void DisplayError()
    {
        if(scene.name == "Game" && gameData.errorOpened)
        {
            Time.timeScale = 0f;
            errorCanvas.enabled = true;
            gameObject.transform.GetChild(0).GetComponent<FilterFade>().FadeToBlack();
            gameObject.transform.GetChild(1).GetComponent<ScaleTween>().OnOpen();
        }
        if (scene.name == "Main Menu" && menuData.errorOpened)
        {
            errorCanvas.enabled = true;
            gameObject.transform.GetChild(0).GetComponent<FilterFade>().FadeToBlack();
            gameObject.transform.GetChild(1).GetComponent<ScaleTween>().OnOpen();
        }
    }
}
