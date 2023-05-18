using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrialController : MonoBehaviour
{
    [SerializeField] private WaterRise waterRise;
    [SerializeField] private CreatePlatforms createPlatforms;
    [SerializeField] private SpawnFlyingEnemy spawnFE;
    // Start is called before the first frame update
    void Start()
    {
        waterRise.enabled = false;
        createPlatforms.enabled = false;
        spawnFE.nextSpawn = 5f;
    }

    public void ReplayTrialScene()
    {
        SceneManager.LoadScene("Trial");
        Time.timeScale = 1f;
    }
}
