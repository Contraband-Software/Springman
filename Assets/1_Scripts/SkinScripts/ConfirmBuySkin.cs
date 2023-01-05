using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using Architecture.Localisation;
using Architecture;

public class ConfirmBuySkin : MonoBehaviour
{
    [Header("Purchase Details")]
    public int cost;
    public List<string> possiblePurchases;
    public enum PurchaseType { Silver, Gold, Ads };
    public PurchaseType purchaseType;

    [Header("Misc References")]
    public CosmeticsData cosmeticsData;
    public SkinsController skinsController;
    public RectTransform masterCanvasRect;
    public CosmeticsMenuController cosMenuCon;

    [Header("Skin Change Sprites")]
    public Image top;
    public Image bottom;
    public Image unlockedSkinDemo;

    [Header("YesButtonText")]
    public TextMeshProUGUI yesButtonText;
    public TextMeshProUGUI okText;

    [Header("QMark")]
    public RectTransform qMarkRect;
    public Image qMarkImage;
    public LeanTweenType easeOutQMark;

    [Header("Buttons")]
    public GameObject yesButton;
    public Image yesButtonImage;
    public RectTransform yesButtonRect;

    public GameObject noButton;
    public Image noButtonImage;
    public RectTransform noButtonRect;

    public LeanTweenType easeInMove;
    public LeanTweenType easeInFade;
    public LeanTweenType easeOutFade;

    [Header("Cost")]
    public TextMeshProUGUI costText;
    public GameObject costTypeObj;
    public Image CostType;
    public LeanTweenType fadeOutCost;
    public UpdateValue silverUpdateVal;
    public UpdateValue goldUpdateVal;

    [Header("Text")]
    public GameObject buyText;
    public TextMeshProUGUI buyTextText;
    public TextMeshProUGUI noText;
    public LeanTweenType fadeInBuyText;
    public LeanTweenType easeInBuyText;

    [Header("Unlocking")]
    public string unlockedSkin;

    [Header("ButtonStatus")]
    public bool pressedOnce = false;
    public ScaleTween buyTabScaleTween;
    public FilterFade buyTabFilterFade;

    [Header("Purchase Sound")]
    public AudioSource purchase_sound;

    public void Start()
    {
        purchase_sound = GameObject.Find("MenuAudio/Purchase").gameObject.GetComponent<AudioSource>();
    }


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

    public void UnlockSkin()
    {
        if (!pressedOnce)
        {
            purchase_sound.Play();

            pressedOnce = true;

            System.Random rnd = new System.Random();
            int purchaseIndex = rnd.Next(0, possiblePurchases.Count);
            unlockedSkin = possiblePurchases[purchaseIndex];

            cosmeticsData.unlockedSkins.Add(unlockedSkin);

            cosmeticsData.SaveCosData();
            skinsController.OpenNewTab(cosMenuCon.canvases[0].name);

            ChangeCurrencyValues();
            cosmeticsData.menuData.SaveGameData();

            FadeButtonOut();
            MoveToCentre();
        }
        else
        {
            skinsController.OpenNewTab(cosMenuCon.canvases[0].name);
            buyTabFilterFade.FadeToClear();
            buyTabScaleTween.OnClose();
            cosMenuCon.buyCanvasOn = false;
        }
    }

    public void ChangeCurrencyValues()
    {
        if(purchaseType == PurchaseType.Silver)
        {
            cosmeticsData.menuData.silver -= cost;
            silverUpdateVal.CurrencyChangeDetails(UpdateValue.ValueType.Silver);
        }
        if(purchaseType == PurchaseType.Gold)
        {
            cosmeticsData.menuData.gold -= cost;
            goldUpdateVal.CurrencyChangeDetails(UpdateValue.ValueType.Gold);
        }
    }

    public void FadeButtonOut()
    {
        var color = noButtonImage.color;
        var fadeOutColor = color;
        fadeOutColor.a = 0f;

        LeanTween.value(noButton, updateColorValueNoButton, color, fadeOutColor, 0.1f).setIgnoreTimeScale(true);
    }

    public void MoveToCentre()
    {
        LeanTween.value(yesButton, MoveToCentreCallback, yesButtonRect.anchoredPosition.x, 0f, 0.3f).setIgnoreTimeScale(true).setEase(easeInMove);
        TransitionYesButtonText();
        DisappearQMark();
    }

    public void TransitionYesButtonText()
    {
        var color = yesButtonText.color;
        var fadeOutColor = color;
        fadeOutColor.a = 0f;
        LeanTween.value(yesButtonText.gameObject, UpdateColorValueYesText, color, fadeOutColor, 0.15f).setIgnoreTimeScale(true).setEase(easeOutFade);

        var color2 = okText.color;
        var fadeInColor = color2;
        fadeInColor.a = 1f;

        LeanTween.value(okText.gameObject, UpdateColorValueOKText, color2, fadeInColor, 0.15f).setIgnoreTimeScale(true).setEase(easeInFade);
    }

    public void DisappearQMark()
    {
        LeanTween.scale(qMarkRect, Vector3.zero, 0.3f).setEase(easeOutQMark).setIgnoreTimeScale(true);

        var color = qMarkImage.color;
        var fadeOutColor = color;
        fadeOutColor.a = 0f;
        LeanTween.value(qMarkRect.gameObject, UpdateColorValueQMark, color, fadeOutColor, 0.3f).setIgnoreTimeScale(true).setEase(easeOutFade).setOnComplete(ChangeToUnlockedSkin);
        LeanTween.delayedCall(1f, ReAppearText).setIgnoreTimeScale(true);
        DisappearText();
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
        LeanTween.value(top.gameObject, updateColorValueSkinBackground, topcolor, fadeOutColor, 0.3f).setIgnoreTimeScale(true);
        LeanTween.value(unlockedSkinDemo.gameObject, updateColorValueSkinDemo, botcolor, fadeInColor, 0.8f).setIgnoreTimeScale(true);
    }

    public void DisappearText()
    {
        var color = buyTextText.color;
        var fadeOutColor = color;
        fadeOutColor.a = 0f;
        LeanTween.value(buyText, UpdateColorValueText, color, fadeOutColor, 0.3f).setIgnoreTimeScale(true).setEase(fadeOutCost);
    }

    public void ReAppearText()
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

    //CALLBACK FUNCTIONS

    void updateColorValueSkinBackground(Color col)
    {
        top.color = col;
        bottom.color = col;
    }

    void updateColorValueSkinDemo(Color col)
    {
        unlockedSkinDemo.color = col;
    }

    void updateColorValueNoButton(Color col)
    {
        noButtonImage.color = col;
        noText.color = col;
    }

    void MoveToCentreCallback(float x)
    {
        yesButtonRect.anchoredPosition = new Vector2(x, yesButtonRect.anchoredPosition.y);
    }

    void UpdateColorValueOKText(Color col)
    {
        okText.color = col;
    }
    void UpdateColorValueYesText(Color col)
    {
        yesButtonText.color = col;
    }

    void UpdateColorValueQMark(Color col)
    {
        qMarkImage.color = col;
    }

    void UpdateColorValueText(Color col)
    {
        buyTextText.color = col;
        CostType.color = col;
        costText.color = col;
    }

    void UpdateColorValueBuyText(Color col)
    {
        buyTextText.color = col;
    }

    public void StopLerping()
    {
        StopAllCoroutines();
    }

    private Vector2 org_QMarkRectScale;
    private Vector3 org_yesButtonRectPos;
    private Color org_noButtonCol;
    private Color org_noButtonTextCol;
    private Color org_QMarkColor;
    private Color org_screwColor;
    private Color org_buyTextColor;
    private Color org_costTextColor;
    private Color org_BG;
    private Color org_skinDemo;
    private Color org_yesButtonTextCol;
    private Color org_okTextCol;

    private Vector3 org_buyTextScale;
    private Vector2 org_buyTextRectPos;
    private Vector2 org_BuyTextRectSize;

    public void SetOriginals()
    {

        org_QMarkRectScale = qMarkRect.localScale;
        org_yesButtonRectPos = yesButtonRect.anchoredPosition;
        org_noButtonCol = noButtonImage.color;
        org_noButtonTextCol = noText.color;
        org_QMarkColor = qMarkImage.color;
        org_screwColor = CostType.color;
        org_buyTextColor = buyTextText.color;
        org_costTextColor = costText.color;
        org_BG = top.color;
        org_skinDemo = unlockedSkinDemo.color;
        org_yesButtonTextCol = yesButtonText.color;
        org_okTextCol = okText.color;

        org_buyTextScale = buyTextText.rectTransform.localScale;
        org_buyTextRectPos = buyTextText.rectTransform.anchoredPosition;
        org_BuyTextRectSize = buyTextText.rectTransform.sizeDelta;
    }

    public void ResetToDefaults()
    {

        buyText.GetComponent<TextLocaliserUI>().key = "buyTabSkin_text";
        buyText.GetComponent<TextLocaliserUI>().Localize();
        //buyTextText.text = "Are you sure you want to buy a Random Color for:";
        pressedOnce = false;

        qMarkRect.localScale = org_QMarkRectScale;
        yesButtonRect.anchoredPosition = org_yesButtonRectPos;
        noButtonImage.color = org_noButtonCol;
        noText.color = org_noButtonTextCol;
        qMarkImage.color = org_QMarkColor;
        CostType.color = org_screwColor;
        buyTextText.color = org_buyTextColor;
        costText.color = org_costTextColor;
        top.color = org_BG;
        bottom.color = org_BG;
        unlockedSkinDemo.color = org_skinDemo;
        yesButtonText.color = org_yesButtonTextCol;
        okText.color = org_okTextCol;

        buyTextText.rectTransform.localScale = org_buyTextScale;
        buyTextText.rectTransform.anchoredPosition = org_buyTextRectPos;
        buyTextText.rectTransform.sizeDelta = org_BuyTextRectSize;
    }

    public void stopTweens()
    {
        LeanTween.cancelAll();
    }
}
