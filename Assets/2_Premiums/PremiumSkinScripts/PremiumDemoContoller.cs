﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PremiumDemoContoller : MonoBehaviour
{
    public CosmeticsData cosData;
    public PremSkinDetailsDemo activePremiumSkin;
    public GameObject AllPremiumDemos;
    public List<GameObject> DemoObjects = new List<GameObject>();

    public void Start()
    {
        for(int d = 0; d < AllPremiumDemos.transform.childCount; d++)
        {
            DemoObjects.Add(AllPremiumDemos.transform.GetChild(d).gameObject);
            AllPremiumDemos.transform.GetChild(d).gameObject.SetActive(false);
        }
    }

    public void ShowActivePremiumSkin()
    {
        HidePremiumSkin();

        int indexOfPremium = cosData.allPremiums.IndexOf(cosData.activePremiumSkinName);
        DemoObjects[indexOfPremium].gameObject.SetActive(true);
        activePremiumSkin = DemoObjects[indexOfPremium].gameObject.GetComponent<PremSkinDetailsDemo>();
        activePremiumSkin.UpdateSkin();
    }

    public void HidePremiumSkin()
    {
        foreach(GameObject premSkin in DemoObjects)
        {
            if (premSkin.activeInHierarchy)
            {
                premSkin.SetActive(false);
                activePremiumSkin = null;
            }
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            activePremiumSkin.UpdateSkin();
        }
    }
}