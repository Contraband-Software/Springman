using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Development
{
    public class GameDebugController : Backend.AbstractSingleton<GameDebugController>
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

        protected override void SingletonAwake()
        {

        }

#endif
    }
}