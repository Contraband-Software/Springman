using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using PlatformIntegrations;

namespace Architecture
{
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

                if (slider.value >= 0.88f && !operation.allowSceneActivation)
                {
                    //start coroutine to check if save data has been loaded
                    Debug.Log("LOADED ASSETS");
                    operation.allowSceneActivation = true;
                    StartCoroutine(WaitForSaveDataToLoad());
                    yield return null;
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
#if !UNITY_EDITOR
        while (!IntegrationsManager.instance.socialManager.HasLoadedFromCloud())
        {
            Debug.Log("Waiting to load data from GPGS...");
            Debug.Log("Data loaded: " + IntegrationsManager.instance.socialManager.HasLoadedFromCloud());
            

            Debug.Log(IntegrationsManager.instance.socialManager.GetCachedSaveGame());
            yield return new WaitForSeconds(0.1f);
        }

        if(IntegrationsManager.instance.socialManager.GetCachedSaveGame() == null){
            Debug.Log("LOADED FROM GPGS, FILE SEEMS EMPTY...");
                
        }
#endif
            Debug.Log("LOADING SCRIPT:  RECIEVED DATA FROM LOAD");
            Debug.Log("ALLOWING SCENE ACTIVATION");
            Debug.Log("Scene Activation Allowed?: " + operation.allowSceneActivation);
            operation.allowSceneActivation = true;
            yield break;
        }
    }
}