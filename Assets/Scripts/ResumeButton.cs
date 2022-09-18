using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class ResumeButton : MonoBehaviour
{
    public Canvas uiToClose;
    public GameData gameData;
    public PauseButton pauseButton;

    public event ResumeEvent OnResume;
    public delegate void ResumeEvent();

    public void OnClick()
    {
        Time.timeScale = 1f;

        if(gameData.tutorialComplete == true)
        {
            Time.timeScale = 1f;
        }
        gameData.Paused = false;

        uiToClose.gameObject.transform.GetChild(1).GetComponent<ScaleTween>().OnClose();
        pauseButton.OnOpen();

        OnResume?.Invoke();
    }

    private void Start()
    {
        gameData = GameObject.Find("GameController").GetComponent<GameData>();
    }

}
