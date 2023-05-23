using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Architecture.Managers;

public class SkinSpawner : MonoBehaviour
{
    public CosmeticsData cosData;
    [Header("All Skin Prefabs")]
    public List<GameObject> AllPremiumPrefabs = new List<GameObject>();
    
    private GameObject playerRef;
    public void SpawnPremium(string skin_name)
    {
        print("SPAWNING PREMIUM: " + skin_name);

        int index = 0;
        foreach(GameObject gameObject in AllPremiumPrefabs)
        {
            if (gameObject.name == skin_name)
            {
                GameObject spawnedSkin = Instantiate(gameObject, null);
                spawnedSkin.transform.position = new Vector3(0f, 1f, 0f);
                InGamePremCon gamePremCon = GameObject.Find("GameExclusivePremiumController").GetComponent<InGamePremCon>();
                gamePremCon.spawnedSkin = spawnedSkin;
                gamePremCon.ApplySkin();
                //gamePremCon.skinIndex = index;
            }
            index++;
        }

        //int skinIndex = UserGameData.Instance.allPremiums.IndexOf(skin_name);
        //GameObject spawnedSkin = Instantiate(AllPremiumPrefabs[skinIndex], null);
        //spawnedSkin.transform.position = new Vector3(0f, 1f, 0f);
        //InGamePremCon gamePremCon = GameObject.Find("GameExclusivePremiumController").GetComponent<InGamePremCon>();
        //gamePremCon.spawnedSkin = spawnedSkin;
        //gamePremCon.ApplySkin();
        //gamePremCon.skinIndex = skinIndex;
    }
}
