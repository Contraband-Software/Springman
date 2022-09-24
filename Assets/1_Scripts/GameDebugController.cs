using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDebugController : MonoBehaviour
{
#if UNITY_EDITOR
    [Header("References")]
    [SerializeField] GameObject flyingEnemyPrefab;

    [Header("Tweakables")]
    [SerializeField] bool spawnFlyingEnemyOnStart = false;
    [SerializeField] bool waterDisabled = false;
    [SerializeField] bool spawnSEOnAllPlatforms = false;
    [SerializeField] bool disableAds = false;
    [SerializeField] bool alwaysFirstRun = false;

    public bool GetWaterDisabled()
    {
        return waterDisabled;
    }
    public bool GetSEOnAllPlatforms()
    {
        return spawnSEOnAllPlatforms;
    }
    public bool GetAdsDisabled()
    {
        return disableAds;
    }
    public bool GetAlwaysFirstRun()
    {
        return alwaysFirstRun;
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        if (spawnFlyingEnemyOnStart)
        {
            GameObject fe = Instantiate(flyingEnemyPrefab);
            fe.transform.position = new Vector3(-1.5f, 3.1f, 0f);
        }
    }

#endif
}
