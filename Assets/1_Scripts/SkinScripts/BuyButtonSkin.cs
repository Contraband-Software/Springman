﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using PlatformIntegrations;
using Architecture.Managers;

namespace Architecture.Customization
{
    public class BuyButtonSkin : MonoBehaviour
    {
        public Canvas canvasToOpen;
        public Canvas brokeBoyCanvas;
        public Canvas allBoughtCanvas;
        public FilterFade brokeFilter;
        private CosmeticsMenuController cosMenuCon;
        [Header("Purchase Details")]
        public float cost;
        public enum PurchaseType { Silver, Gold, Ads };
        public PurchaseType purchaseType;
        public ConfirmBuySkin confirmBuySkin;
        public SkinsController skinsCon;

        [Header("Skin Selector")]
        public SkinSelector skinSelector;

        [Header("SilverChanges")]
        public int silverCost;
        public Material silverColour;
        public Image CostType;

        [Header("GoldChanges")]
        public int goldCost;
        public Material goldColour;
        public Image CostTypeGold;

        [Header("ReferencesToChange")]
        public TextMeshProUGUI costText;
        public Image purchaseTypeImage;

        [Header("Alt Confirmer: Ads")]
        public BuyAdsSkinCon adsBuycon;

        [Header("Price on Button")]
        public TextMeshProUGUI priceOnButton;

        private AdvertisementsManager adManager;
        public Button btn;

        UserGameData menuData;

        private void Start()
        {
            menuData = UserGameData.Instance;
            canvasToOpen.enabled = false;
            brokeBoyCanvas.enabled = false;
            allBoughtCanvas.enabled = false;
            cosMenuCon = skinSelector.cosMenuCon;
            adManager = IntegrationsManager.Instance.advertisementsManager;

            if (purchaseType == PurchaseType.Ads)
            {
                priceOnButton.text = (10 - menuData.ads).ToString();
                adManager.GetShowCompleteEvent("AdSkins").AddListener((bool status) =>
                {
                    if (status)
                    {
                        menuData.ads++;
                        menuData.SaveGameData();
                    }
                    else
                    {
                        //ad failed, either because the user exited early (skipped), has no wifi or one couldnt be fetched for some reason
                        //Show a failiure dialogue
                    }

                    //probably best to update the price on the button regardless of the outcome to avoid bugs
                    priceOnButton.text = (10 - menuData.ads).ToString();
                });
                adManager.GetLoadCompleteEvent("AdSkins").AddListener(() =>
                {
                    //btn.interactable = true;
                    Debug.Log("Loaded ad skin ad");
                });
                btn.interactable = adManager.GetLoadedStatus("AdSkins");
            }
        }

        private void Update()
        {
            if (purchaseType == PurchaseType.Ads)
            {
                btn.interactable = IntegrationsManager.Instance.advertisementsManager.GetLoadedStatus("AdSkins");
            }
        }

        public void OnClick()
        {
            if (purchaseType == PurchaseType.Silver && menuData.silver >= cost)
            {
                confirmBuySkin.cost = silverCost;
                confirmBuySkin.purchaseType = ConfirmBuySkin.PurchaseType.Silver;
                costText.text = silverCost.ToString();
                costText.fontMaterial = silverColour;
                purchaseTypeImage.sprite = CostType.sprite;

                confirmBuySkin.PopulatePossiblePurchases(skinSelector);
                if (confirmBuySkin.possiblePurchases.Count > 0)
                {
                    canvasToOpen.enabled = true;
                    cosMenuCon.buyCanvasOn = true;
                    if (skinsCon.defaultsSetSILVER || skinsCon.defaultsSetGOLD)
                    {
                        confirmBuySkin.ResetToDefaults();
                        confirmBuySkin.StopLerping();
                        confirmBuySkin.stopTweens();
                        canvasToOpen.gameObject.transform.GetChild(1).GetComponent<ScaleTween>().OnOpen();
                        //defaultsSet = false;
                    }
                    else
                    {
                        confirmBuySkin.SetOriginals();
                        skinsCon.defaultsSetSILVER = true;
                        canvasToOpen.gameObject.transform.GetChild(1).GetComponent<ScaleTween>().OnOpen();
                    }
                }
                else
                {
                    allBoughtCanvas.enabled = true;
                    cosMenuCon.allBoughtCanvasOn = true;
                    allBoughtCanvas.transform.GetChild(0).GetComponent<FilterFade>().FadeToBlack();
                    allBoughtCanvas.gameObject.transform.GetChild(1).GetComponent<ScaleTween>().OnOpen();
                    //tell that everything bought
                }
            }
            else if (purchaseType == PurchaseType.Gold && menuData.gold >= cost)
            {
                confirmBuySkin.cost = goldCost;
                confirmBuySkin.purchaseType = ConfirmBuySkin.PurchaseType.Gold;
                costText.text = goldCost.ToString();
                costText.fontMaterial = goldColour;
                purchaseTypeImage.sprite = CostTypeGold.sprite;

                confirmBuySkin.PopulatePossiblePurchases(skinSelector);
                if (confirmBuySkin.possiblePurchases.Count > 0)
                {
                    canvasToOpen.enabled = true;
                    cosMenuCon.buyCanvasOn = true;
                    if (skinsCon.defaultsSetGOLD || skinsCon.defaultsSetSILVER)
                    {
                        confirmBuySkin.ResetToDefaults();
                        confirmBuySkin.StopLerping();
                        confirmBuySkin.stopTweens();
                        canvasToOpen.gameObject.transform.GetChild(1).GetComponent<ScaleTween>().OnOpen();
                    }
                    else
                    {
                        confirmBuySkin.SetOriginals();
                        skinsCon.defaultsSetGOLD = true;
                        canvasToOpen.gameObject.transform.GetChild(1).GetComponent<ScaleTween>().OnOpen();
                    }
                }
                else
                {
                    allBoughtCanvas.enabled = true;
                    cosMenuCon.allBoughtCanvasOn = true;
                    allBoughtCanvas.transform.GetChild(0).GetComponent<FilterFade>().FadeToBlack();
                    allBoughtCanvas.gameObject.transform.GetChild(1).GetComponent<ScaleTween>().OnOpen();
                    //tell that everything bought
                }
            }
            else if (purchaseType == PurchaseType.Ads)
            {
                //confirmBuySkin.cost = goldCost;
                //confirmBuySkin.purchaseType = ConfirmBuySkin.PurchaseType.Gold;
                //costText.text = goldCost.ToString();
                //costText.fontMaterial = goldColour;
                //purchaseTypeImage.sprite = CostTypeGold.sprite;

                adsBuycon.PopulatePossiblePurchases(skinSelector);
                if (adsBuycon.possiblePurchases.Count > 0)
                {
                    if (menuData.ads < cost)
                    {
                        Debug.LogWarning("AD SKIN BUTTON PRESS");
                        adManager.PlayAd("AdSkins");
                    }
                    else
                    {
                        if (skinsCon.defaultsSetADS)
                        {
                            adsBuycon.ResetToDefaults();
                            adsBuycon.stopTweens();
                            adsBuycon.StopLerping();
                        }
                        else
                        {
                            adsBuycon.SetDefaults();
                            skinsCon.defaultsSetADS = true;
                        }


                        menuData.ads = 0;

                        canvasToOpen.enabled = true;
                        cosMenuCon.buyCanvasOn = true;
                        adsBuycon.pressedOnce = false;

                        canvasToOpen.gameObject.transform.GetChild(1).GetComponent<ScaleTween>().OnOpen();

                        adsBuycon.UnlockAdSkin();

                    }

                    priceOnButton.text = (10 - menuData.ads).ToString();
                }
                else
                {
                    allBoughtCanvas.enabled = true;
                    cosMenuCon.allBoughtCanvasOn = true;
                    allBoughtCanvas.transform.GetChild(0).GetComponent<FilterFade>().FadeToBlack();
                    allBoughtCanvas.gameObject.transform.GetChild(1).GetComponent<ScaleTween>().OnOpen();
                    //tell that everything bought
                }

                /*
                //confirmBuySkin.PopulatePossiblePurchases(skinSelector);
                if (confirmBuySkin.possiblePurchases.Count > 0)
                {
                    canvasToOpen.gameObject.SetActive(true);
                    canvasToOpen.enabled = true;
                    if (skinsCon.defaultsSetGOLD || skinsCon.defaultsSetSILVER)
                    {
                        confirmBuySkin.ResetToDefaults();
                        confirmBuySkin.StopLerping();
                        confirmBuySkin.stopTweens();
                        canvasToOpen.gameObject.transform.GetChild(1).GetComponent<ScaleTween>().OnOpen();
                    }
                    else
                    {
                        confirmBuySkin.SetOriginals();
                        skinsCon.defaultsSetGOLD = true;
                        canvasToOpen.gameObject.transform.GetChild(1).GetComponent<ScaleTween>().OnOpen();
                    }
                }
                else
                {
                    allBoughtCanvas.gameObject.SetActive(true);
                    allBoughtCanvas.enabled = true;
                    allBoughtCanvas.gameObject.transform.GetChild(1).GetComponent<ScaleTween>().OnOpen();
                    //tell that everything bought
                }
                */
            }
            else
            {
                brokeBoyCanvas.enabled = true;
                cosMenuCon.brokeCanvasOn = true;
                brokeFilter.FadeToBlack();
                brokeBoyCanvas.gameObject.transform.GetChild(1).GetComponent<ScaleTween>().OnOpen();
            }
        }
    }
}