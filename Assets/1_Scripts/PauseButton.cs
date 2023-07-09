using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour
{
    public Canvas PauseMenu;
    public CanvasGroup gameCanvasGroup;
    public Button pausebutton;
    public PlayerController pController;

    public event PauseEvent OnPause;
    public delegate void PauseEvent();

    private AudioSource[] audios;

    public void OnClick()
    {
        PauseMenu.gameObject.SetActive(true);
        PauseMenu.enabled = true;
        Time.timeScale = 0;
        Architecture.Managers.GamePlay.GetReference().Paused = true;

        PauseMenu.gameObject.transform.GetChild(1).GetComponent<ScaleTween>().OnOpen();
        Architecture.Managers.UserGameData.Instance.SaveGameData();

        LeanTween.alphaCanvas(gameCanvasGroup, 0f, 0.2f).setIgnoreTimeScale(true);

        OnPause?.Invoke();


        GatherAudioSources();
        for(int i = 0; i < audios.Length; i++)
        {
            if (audios[i].isPlaying)
            {
                //audios[i].Pause();
            }
        }
    }
    private void Awake()
    {
        pController = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    private void Start()
    {
        PauseMenu.enabled = false;

        GatherAudioSources();
        pController.revive_Reassign += ReassignPCon;
    }

    private void ReassignPCon(PlayerController pCon)
    {
        pController = pCon;
    }

    public void OnOpen()
    {
        LeanTween.alphaCanvas(gameCanvasGroup, 1f, 0.2f).setIgnoreTimeScale(true);
    }

    private void OnApplicationPause(bool pause)
    {
        if(pController.state == PlayerController.State.Alive && PauseMenu.enabled == false)
        {
            pausebutton.onClick.Invoke();
        }
    }

    private void GatherAudioSources()
    {
        audios = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
    }

    public void ResumeAudios()
    {
        GatherAudioSources();
        for (int i = 0; i < audios.Length; i++)
        {
            if (Architecture.Managers.UserGameData.Instance.soundsOn)
            {
                //audios[i].UnPause();
            }
        }
    }
}
