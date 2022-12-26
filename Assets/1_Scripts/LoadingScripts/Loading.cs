﻿using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using PlatformIntegrations;
using static UpdateValue;

public class Loading : MonoBehaviour
{
    public enum SceneIndexes
    {
        LOADING = 0,
        MAINMENU = 1,
        GAME = 2
    }

    [Header("Important References")]
    [SerializeField] SceneIndexes Scene;
    [Header("Settings")]
    [SerializeField, Range(0, 1)] float updateSpeed = 0.3f;
    [SerializeField] Slider slider;

    private UnityEngine.AsyncOperation operation;

    void Awake()
    {
        StartCoroutine(LoadScene((int)Scene));

    }

    IEnumerator LoadScene(int sceneIndex)
    {
        yield return null;

        operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            yield return StartCoroutine(UpdateLoadingBar(operation.progress));

            if (slider.value >= 0.88f)
            {
                //start coroutine to check if save data has been loaded
                Debug.Log("LOADED ASSETS");
                StartCoroutine(WaitForSaveDataToLoad());
            }

            yield return null;
        }
    }

    IEnumerator UpdateLoadingBar(float targetValue)
    {
        while (targetValue - slider.value > 0.005f)
        {
            Debug.Log("Updating loading bar...");
            slider.value += (targetValue - slider.value) * updateSpeed;

            yield return null;
        }
    }


    IEnumerator WaitForSaveDataToLoad()
    {
#if !UNITY_EDITOR
        while (IntegrationsManager.instance.socialManager.GetCachedSaveGame() == null)
        {
            Debug.Log("Waiting to load data from GPGS...");
            yield return new WaitForSeconds(0.1f);
        }
#endif
        Debug.Log("LOADING SCRIPT:  RECIEVED DATA FROM LOAD");
        Debug.Log("ALLOWING SCENE ACTIVATION");
        operation.allowSceneActivation = true;
        yield return null;
    }
}