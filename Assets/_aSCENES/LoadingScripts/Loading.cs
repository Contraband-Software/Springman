using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{

    public Slider slider;
    private void Awake()
    {

    }

    void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        Debug.Log(currentScene.name);
        if(currentScene.name != "Main Menu")
        {
            LoadMainMenu();
        }
    }

    public void LoadMainMenu()
    {
        StartCoroutine(LoadAsynchMM((int)SceneIndexes.MAINMENU));
    }

    IEnumerator LoadAsynchMM(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            slider.value = progress;
            yield return null;
        }

        Scene sceneToLoad = SceneManager.GetSceneAt(sceneIndex);
        if (sceneToLoad.IsValid())
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneAt(sceneIndex));
        }
    }
}

public enum SceneIndexes
{
    LOADING = 0,
    MAINMENU = 1,
    GAME = 2
}
