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
        [SerializeField] Image loadingBar;

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

                if (loadingBar.fillAmount >= 0.88f && !operation.allowSceneActivation)
                {
                    //start coroutine to check if save data has been loaded
                    Debug.Log("LOADED ASSETS");

                    yield return StartCoroutine(WaitForSaveDataToLoad());

                    yield return StartCoroutine(UpdateLoadingBar(1f));

                    if(loadingBar.fillAmount >= 0.99f && !operation.allowSceneActivation)
                    {
                        operation.allowSceneActivation = true;
                    }

                    yield return null;
                }

                yield return null;
            }
        }

        IEnumerator UpdateLoadingBar(float targetValue)
        {
            while (targetValue - loadingBar.fillAmount > 0.005f)
            {
                loadingBar.fillAmount += (targetValue - loadingBar.fillAmount) * updateSpeed;

                yield return null;
            }
        }


        IEnumerator WaitForSaveDataToLoad()
        {
#if !UNITY_EDITOR
            IntegrationsManager.Instance.socialManager.TrySignIn();

            Debug.Log("LOADING: Waiting to load data from GPGS...");
            Debug.Log("LOADING: Data loaded: " + IntegrationsManager.Instance.socialManager.HasConnectedToCloud());

            yield return new WaitUntil(() => IntegrationsManager.Instance.socialManager.HasConnectedToCloud());

            Debug.Log("LOADING: Waiting to load data from GPGS...");
            Debug.Log("LOADING: Data loaded: " + IntegrationsManager.Instance.socialManager.HasConnectedToCloud());


            if(IntegrationsManager.Instance.socialManager.LoadedCloudSaveEmpty()){
                Debug.Log("LOADING: LOADED FROM GPGS, FILE SEEMS EMPTY...");
            }
#endif
            Debug.Log("LOADING: RECIEVED RESPONSE FROM LOAD");
            Debug.Log("LOADING: ALLOWING SCENE ACTIVATION");
            Debug.Log("LOADING: Scene Activation Allowed?: " + operation.allowSceneActivation);

            yield break;
        }
    }
}