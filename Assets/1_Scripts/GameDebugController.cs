using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDebugController : Arch.AbstractSingleton<GameDebugController>
{
#if UNITY_EDITOR
    //public static GameDebugController Instance { get; private set; }

    [Header("References")]
    [SerializeField] GameObject flyingEnemyPrefab;

    [Header("Tweakables")]
    [SerializeField] bool spawnFlyingEnemyOnStart = false;
    [SerializeField] bool waterDisabled = false;
    [SerializeField] bool spawnSEOnAllPlatforms = false;
    //[SerializeField] bool disableFlyingEnemies = false;
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
    //public bool GetDisableFlyingEnemies()
    //{
    //    return disableFlyingEnemies;
    //}
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
        //DontDestroyOnLoad(gameObject);

        //if (Instance == null)
        //{
        //    Instance = this;

        //    Initialize();
        //}
        //else
        //{
        //    Destroy(gameObject);
        //}

        MakeSingleton();
        #endregion
    }

    private void OnLevelWasLoaded(int level)
    {
        if (level == 2)
        {
            if (spawnFlyingEnemyOnStart)
            {
                GameObject fe = Instantiate(flyingEnemyPrefab);
                fe.transform.position = new Vector3(-1.5f, 3.1f, 0f);
            }
        }
    }

    private void Initialize()
    {
        
    }

#endif
}
