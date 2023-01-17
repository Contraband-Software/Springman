using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

using Architecture.Managers;

public class SkinsController : MonoBehaviour
{
    public SkinSelector silverSelector;
    public SkinSelector goldSelector;
    public SkinSelector adsSelector;
    public SkinSelector_Premium premiumSelector;

    public CosmeticsMenuController cosMenuCon;

    public GameObject blackTint;

    [Header("Current Skin Selected")]
    public Image prevSelectedImage;

    public SkinSpecs currentSkin;
    public string currentSkinID;
    public Vector3 currentObject;

    [Header("Skin Buying Tab Shit")]
    public bool defaultsSetSILVER = false;
    public bool defaultsSetGOLD = false;
    public bool defaultsSetADS = false;

    [Header("Premium")]
    public string selectedPremium;

    public void OpenNewTab(string name)
    {
        if(name.ToLower() == "silvers")
        {
            silverSelector.CollectSkins();
            LockedSkins(silverSelector);
            if (silverSelector.tabSkinCodes.Contains(currentSkinID))
            {
                if (prevSelectedImage != null)
                {
                    prevSelectedImage.color = Color.white;
                }

                prevSelectedImage = silverSelector.skinButtonScripts[silverSelector.tabSkinCodes.IndexOf(currentSkinID)].gameObject.transform.GetChild(0).GetComponent<Image>();
                prevSelectedImage.color = silverSelector.onSelectColour;
            }

            //make the currently active skin backgroun pink by checking if the current skin is in the tabskincodes
        }
        if (name.ToLower() == "golds")
        {
            goldSelector.CollectSkins();
            LockedSkins(goldSelector);
            if (goldSelector.tabSkinCodes.Contains(currentSkinID))
            {
                if (prevSelectedImage != null)
                {
                    prevSelectedImage.color = Color.white;
                }

                prevSelectedImage = goldSelector.skinButtonScripts[goldSelector.tabSkinCodes.IndexOf(currentSkinID)].gameObject.transform.GetChild(0).GetComponent<Image>();
                prevSelectedImage.color = goldSelector.onSelectColour;
            }

            //make the currently active skin backgroun pink by checking if the current skin is in the tabskincodes
        }
        if(name.ToLower() == "ads")
        {
            adsSelector.CollectSkins();
            LockedSkins(adsSelector);
            if (adsSelector.tabSkinCodes.Contains(currentSkinID))
            {
                if (prevSelectedImage != null)
                {
                    prevSelectedImage.color = Color.white;
                }

                prevSelectedImage = adsSelector.skinButtonScripts[adsSelector.tabSkinCodes.IndexOf(currentSkinID)].gameObject.transform.GetChild(0).GetComponent<Image>();
                prevSelectedImage.color = adsSelector.onSelectColour;
            }
        }
        if(name.ToLower() == "premium")
        {
            if (premiumSelector.premTabSkinCodes.Contains(currentSkinID))
            {
                if (prevSelectedImage != null)
                {
                    prevSelectedImage.color = Color.white;
                }

                prevSelectedImage = premiumSelector.premSkinIcons[premiumSelector.premTabSkinCodes.IndexOf(currentSkinID)].gameObject.GetComponent<Image>();
                prevSelectedImage.color = premiumSelector.onSelectColour;
            }
        }
    }

    public void LockedSkins(SkinSelector selector)
    {
        int index;
        foreach(string skin in selector.tabSkinCodes)
        {
            if (UserGameData.Instance.unlockedSkins.Contains(skin))
            {
                index = selector.tabSkinCodes.IndexOf(skin);
                selector.skinButtonScripts[index].gameObject.SetActive(true);
                if (selector.skinButtonScripts[index].gameObject.transform.childCount > 3)
                {
                    Destroy(selector.skinButtonScripts[index].gameObject.transform.GetChild(3).gameObject);
                }
            }
        }

    }

    public void LoadCurrentSkin()
    {
        FetchData();
        OpenNewTab(cosMenuCon.canvases[0].name);
        silverSelector.CollectSkins();
        goldSelector.CollectSkins();
        adsSelector.CollectSkins();
        premiumSelector.CollectSkins();
        if (!UserGameData.Instance.currentSkinPremium)
        {
            currentSkin = UserGameData.Instance.allSkinSpecs[UserGameData.Instance.allSkinsCodes.IndexOf(currentSkinID)].ConvertToSolid(currentSkin, UserGameData.Instance.allSkinSpecs[UserGameData.Instance.allSkinsCodes.IndexOf(currentSkinID)]);
        }
        else
        {
            currentSkin = null;
            currentSkinID = UserGameData.Instance.currentSkin;
        }
        
    }


    public void LoadAll()
    {
        silverSelector.CollectSkins();
        goldSelector.CollectSkins();
        adsSelector.CollectSkins();
        premiumSelector.CollectSkins();
    }

    public void SendData()
    {
        UserGameData.Instance.currentSkin = currentSkinID;
        UserGameData.Instance.activePremiumSkinName = selectedPremium;
        UserGameData.Instance.HardPassSkinID(currentSkinID);
    }

    public void FetchData()
    {
        currentSkinID = UserGameData.Instance.currentSkin;
    }
}
