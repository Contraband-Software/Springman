using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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

    void Awake()
    {
        StartCoroutine(LoadScene((int)Scene));
    }

    IEnumerator LoadScene(int sceneIndex)
    {
        yield return null;

        UnityEngine.AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            yield return StartCoroutine(UpdateLoadingBar(operation.progress));

            if (slider.value >= 0.88f)
            {
                operation.allowSceneActivation = true;
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
}