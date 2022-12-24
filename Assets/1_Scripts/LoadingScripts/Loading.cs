using System.Collections;
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

    private object data = null;
    private UnityEngine.AsyncOperation operation;

    void Awake()
    {
        StartCoroutine(LoadScene((int)Scene));

        //register listener for loading the user game data
        IntegrationsManager.instance.socialManager.SaveDataLoadCallback.AddListener((bool status, object data) => {
            this.data = data;
        });
        
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

                StartCoroutine(WaitForSaveDataToLoad());
            }

            yield return null;
        }
    }

    IEnumerator UpdateLoadingBar(float targetValue)
    {
        while (targetValue - slider.value > 0.005f)
        {
            slider.value += (targetValue - slider.value) * updateSpeed;

            yield return null;
        }
    }

    IEnumerator WaitForSaveDataToLoad()
    {


        while(data == null)
        {
            yield return new WaitForSeconds(0.1f);
        }

        operation.allowSceneActivation = true;
    }
}