using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Architecture.Managers;
using UnityEngine.Purchasing;
using Unity.Services.Analytics;

public class PremiumDemoContoller : MonoBehaviour
{
    public PremSkinDetailsDemo activePremiumSkin;
    public GameObject AllPremiumDemos;

    // names to sex
    public Dictionary<string, GameObject> DemoObjects { private set; get; } = new();
    public Transform skinIconParent; 

    public void Start()
    {
        for(int d = 0; d < AllPremiumDemos.transform.childCount; d++)
        {
            GameObject g = AllPremiumDemos.transform.GetChild(d).gameObject;
            DemoObjects.Add(g.name, g);
            AllPremiumDemos.transform.GetChild(d).gameObject.SetActive(false);
        }
    }

    public void ShowActivePremiumSkin()
    {
        HidePremiumSkin();

        ProductCatalog pc = ProductCatalog.LoadDefaultCatalog();
        foreach (ProductCatalogItem item in pc.allProducts)
        {
            if (UserGameData.Instance.currentSkin == item.id)
            {
                GameObject res = null;
                if (DemoObjects.TryGetValue(item.defaultDescription.Title, out res))
                {
                    res.SetActive(true);
                    activePremiumSkin = res.GetComponent<PremSkinDetailsDemo>();
                    activePremiumSkin.UpdateSkin();

                    return;
                }

                throw new InvalidOperationException("Premium GameObject with name (" + item.defaultDescription.Title + ") not found.");
            }
        }

        throw new InvalidOperationException("Premium ID not found: " + UserGameData.Instance.currentSkin);
    }

    public void HidePremiumSkin()
    {
        foreach(GameObject premSkin in DemoObjects.Values)
        {
            if (premSkin.activeInHierarchy)
            {
                premSkin.SetActive(false);
                activePremiumSkin = null;
            }
        }
    }
}
