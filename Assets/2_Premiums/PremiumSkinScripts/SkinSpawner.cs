using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinSpawner : MonoBehaviour
{
    public CosmeticsData cosData;
    [Header("All Skin Prefabs")]
    public List<GameObject> AllPremiumPrefabs = new List<GameObject>();
    
    private GameObject playerRef;
    public void SpawnPremium(string skin_name)
    {
        int skinIndex = cosData.allPremiums.IndexOf(skin_name);
        GameObject spawnedSkin = Instantiate(AllPremiumPrefabs[skinIndex], null);
        spawnedSkin.transform.position = new Vector3(0f, 1f, 0f);
        InGamePremCon gamePremCon = GameObject.Find("GameExclusivePremiumController").GetComponent<InGamePremCon>();
        gamePremCon.spawnedSkin = spawnedSkin;
        gamePremCon.ApplySkin();
        gamePremCon.skinIndex = skinIndex;
    }
}
