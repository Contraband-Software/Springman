using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDebugController : MonoBehaviour
{
#if UNITY_EDITOR
    public static GameDebugController instance;

    [Header("References")]
    [SerializeField] GameObject flyingEnemyPrefab;

    [Header("Tweakables")]
    [SerializeField] bool spawnFlyingEnemyOnStart = false;
    [SerializeField] bool waterDisabled = false;
    [SerializeField] bool spawnSEOnAllPlatforms = false;
    //[SerializeField] bool disableAds = false;
    //[SerializeField] bool alwaysFirstRun = false;

    public bool GetWaterDisabled()
    {
        return waterDisabled;
    }
    public bool GetSEOnAllPlatforms()
    {
        return spawnSEOnAllPlatforms;
    }
    //public bool GetAdsDisabled()
    //{
    //    return disableAds;
    //}
    //public bool GetAlwaysFirstRun()
    //{
    //    return alwaysFirstRun;
    //}

    void Start()
    {
        #region PREVENT_DUPLICATES
        DontDestroyOnLoad(gameObject);

        if (instance == null)
        {
            instance = this;

            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
        #endregion
    }

    private void Initialize()
    {
        if (spawnFlyingEnemyOnStart)
        {
            GameObject fe = Instantiate(flyingEnemyPrefab);
            fe.transform.position = new Vector3(-1.5f, 3.1f, 0f);
        }
    }

#endif
}
