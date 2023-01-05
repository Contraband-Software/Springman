using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Architecture;

namespace Architecture
{
    public class ErrorScreenScript : MonoBehaviour
    {
        public Canvas errorCanvas;

        Scene scene;

        private void Start()
        {
            UserGameData.Instance.ErrorEvent.AddListener(() =>
            {
                DisplayError();
            });

            errorCanvas.enabled = false;

            scene = SceneManager.GetActiveScene();

            switch (scene.name)
            {
                case "Main Menu":
                    menuData = GameObject.Find("MenuController").GetComponent<MenuData>();
                    break;
                case "Game":
                    gameData = GameObject.Find("GameController").GetComponent<GameData>();
                    break;
            }
        }

        void DisplayError()
        {
            errorCanvas.enabled = true;
            gameObject.transform.GetChild(0).GetComponent<FilterFade>().FadeToBlack();
            gameObject.transform.GetChild(1).GetComponent<ScaleTween>().OnOpen();

            if (scene.name == "Game")
            {
                Time.timeScale = 0f;
            }
        }
    }
}