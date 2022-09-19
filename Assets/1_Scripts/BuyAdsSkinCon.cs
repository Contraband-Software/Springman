using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuyAdsSkinCon : MonoBehaviour
{
    [Header("Purchase Details")]
    public int cost;
    public List<string> possiblePurchases;

    [Header("Misc References")]
    public CosmeticsData cosmeticsData;
    public SkinsController skinsController;
    public CosmeticsMenuController cosMenuCon;

    [Header("Skin Change Sprites")]
    public Image top;
    public Image bottom;
    public Image unlockedSkinDemo;

    [Header("buy text")]
    public TextMeshProUGUI buyTextText;
    public GameObject buyText;

    public LeanTweenType easeInBuyText;
    public LeanTweenType fadeInBuyText;

    [Header("Unlocking")]
    public string unlockedSkin;

    public bool pressedOnce;

    public void PopulatePossiblePurchases(SkinSelector selector)
    {
        possiblePurchases = new List<string>();
        foreach (string skinID in selector.tabSkinCodes)
        {
            if (!cosmeticsData.unlockedSkins.Contains(skinID) && !possiblePurchases.Contains(skinID))
            {
                possiblePurchases.Add(skinID);
            }
        }
    }

    public void UnlockAdSkin()
    {
        System.Random rnd = new System.Random();
        int purchaseIndex = rnd.Next(0, possiblePurchases.Count);
        unlockedSkin = possiblePurchases[purchaseIndex];

        cosmeticsData.unlockedSkins.Add(unlockedSkin);

        cosmeticsData.SaveCosData();
        skinsController.OpenNewTab(cosMenuCon.canvases[0].name);

        cosmeticsData.menuData.SaveGameData();
        /*
        else
        {
            skinsController.OpenNewTab(cosMenuCon.canvases[0].name);
            buyTabFilterFade.FadeToClear();
            buyTabScaleTween.OnClose();
        }
        */

        ChangeToUnlockedSkin();
    }

    public void ChangeToUnlockedSkin()
    {
        unlockedSkinDemo.sprite = cosmeticsData.allSkinSpecs[cosmeticsData.allSkinsCodes.IndexOf(unlockedSkin)].demoSkin;
        var topcolor = top.color;
        var fadeOutColor = topcolor;
        fadeOutColor.a = 0f;

        var botcolor = unlockedSkinDemo.color;
        var fadeInColor = botcolor;
        fadeInColor.a = 1f;
        LeanTween.value(top.gameObject, updateColorValueSkinBackground, topcolor, fadeOutColor, 0.5f).setIgnoreTimeScale(true);
        LeanTween.value(unlockedSkinDemo.gameObject, updateColorValueSkinDemo, botcolor, fadeInColor, 1.2f).setIgnoreTimeScale(true);

        LeanTween.delayedCall(0.3f, AppearText).setIgnoreTimeScale(true);
    }

    public void AppearText()
    {
        var color = buyTextText.color;
        var fadeInColor = color;
        fadeInColor.a = 1f;
        buyTextText.rectTransform.anchoredPosition = new Vector2(buyTextText.rectTransform.anchoredPosition.x,
            (buyTextText.rectTransform.sizeDelta.y / 2f - buyTextText.rectTransform.anchoredPosition.y) * -1f);
        buyTextText.rectTransform.sizeDelta = new Vector2(buyTextText.rectTransform.sizeDelta.x, buyTextText.rectTransform.sizeDelta.y * 2f);

        buyText.GetComponent<TextLocaliserUI>().key = "buyTabSkin_newSkin";
        buyText.GetComponent<TextLocaliserUI>().Localize();
        //buyTextText.text = "Unlocked a New Colour!"; //here will have to be relocalized to secondary key

        buyTextText.rectTransform.localScale = Vector3.zero;
        LeanTween.scale(buyTextText.gameObject, new Vector3(1f, 1f, 1f), 0.3f).setEase(easeInBuyText).setIgnoreTimeScale(true);
        LeanTween.value(buyText, UpdateColorValueBuyText, color, fadeInColor, 0.3f).setIgnoreTimeScale(true).setEase(fadeInBuyText);
    }

    void updateColorValueSkinBackground(Color col)
    {
        top.color = col;
        bottom.color = col;
    }

    void updateColorValueSkinDemo(Color col)
    {
        unlockedSkinDemo.color = col;
    }

    void UpdateColorValueBuyText(Color col)
    {
        buyTextText.color = col;
    }

    private Color org_buyTextColor;
    private string org_buyTextKey;
    private Vector2 org_buyTextRectSize;
    private Vector2 org_buyTextRectPos;
    private Color org_BGColor;
    private Color org_demoSkinColor;

    public void SetDefaults()
    {
        org_buyTextColor = buyTextText.color;
        org_buyTextKey = buyText.GetComponent<TextLocaliserUI>().key;
        org_buyTextRectSize = buyTextText.rectTransform.sizeDelta;
        org_buyTextRectPos = buyTextText.rectTransform.anchoredPosition;
        org_BGColor = top.color;
        org_demoSkinColor = unlockedSkinDemo.color;
    }

    public void ResetToDefaults()
    {
        buyText.GetComponent<TextLocaliserUI>().key = org_buyTextKey;
        buyText.GetComponent<TextLocaliserUI>().Localize();

        buyTextText.color = org_buyTextColor;
        buyTextText.rectTransform.sizeDelta = org_buyTextRectSize;
        buyTextText.rectTransform.anchoredPosition = org_buyTextRectPos;
        top.color = org_BGColor;
        bottom.color = org_BGColor;
        unlockedSkinDemo.color = org_demoSkinColor;
    }

    public void stopTweens()
    {
        LeanTween.cancelAll();
    }

    public void StopLerping()
    {
        StopAllCoroutines();
    }
}
