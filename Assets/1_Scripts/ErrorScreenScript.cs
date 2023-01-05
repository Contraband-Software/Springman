using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Architecture
{
    public class ErrorScreenScript : MonoBehaviour
    {
        public Canvas errorCanvas { get; set; }

        private void Start()
        {
            UserGameData.Instance.ErrorEvent.AddListener(() =>
            {
                DisplayError();
            });

            errorCanvas.enabled = false;
        }

        void DisplayError()
        {
            errorCanvas.enabled = true;

            gameObject.transform.GetChild(0).GetComponent<FilterFade>().FadeToBlack();
            gameObject.transform.GetChild(1).GetComponent<ScaleTween>().OnOpen();

            if (SceneManager.GetActiveScene().name == "Game")
            {
                Time.timeScale = 0f;
            }
        }
    }
}