using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Architecture.Managers;

public class PremiumSelectLogic : MonoBehaviour
{
    public static PremiumSelectLogic GetReference()
    {
        return GameObject.FindGameObjectWithTag("PremiumCanvas").GetComponent<PremiumSelectLogic>();
    }

    /// <summary>
    /// The point of this script is to organise the UI flow of the premium tab
    /// </summary>
    [Header("Buy Tab References")]
    [SerializeField] Canvas premBuyCanvas;
    [SerializeField] ScaleTween scaleTween;
    [SerializeField] FilterFade ff;
    Image filterImage;
    [Header("Other References")]
    [SerializeField] PremiumDemoContoller premDemoCon;
    [SerializeField] GameObject premDemoParent;
    [SerializeField] GameObject normalSkinParent;
    [SerializeField] CosmeticsMenuController cosMenuCon;
    [SerializeField] GlowColourSelector glowColourSelector;

    [Header("Buy UI Element References")]
    [SerializeField] GameObject tryButton;
    [SerializeField] GameObject purchaseButton;
    [SerializeField] TextMeshProUGUI premiumNameText;
    [SerializeField] TextMeshProUGUI priceText;

    [Header("States (Mutables I know, cringe)")]
    [SerializeField] bool previousSkinWasPremium = false;
    private bool selectedUnownedPremium = false;
    private GameObject previousDemoSkinObject;
    private GameObject currentDemoSkinObject;
    private string previousPremiumSkinName;

    private void Start()
    {
        filterImage = ff.GetComponent<Image>();
    }

    public void SelectedToCustomiseAnOwnedSkin()
    {
        selectedUnownedPremium = false;
        DisplayBuyingTab(UserGameData.Instance.activePremiumSkinName);

        tryButton.SetActive(false);
        purchaseButton.SetActive(false);
    }

    public void SelectedUnOwnedSkin(string skinName, string productid)
    {
        selectedUnownedPremium = true;

        print("click on unowned skin: " + skinName);

        // if button clicked, stops multiple
        if (!ff.filterImage.raycastTarget)
        {
            DisplayBuyingTab(skinName);
            SetProductPriceForBuyButton(productid);
            SetProductIDForBuyButton(productid);

            tryButton.SetActive(true);
            purchaseButton.SetActive(true);

            DisplayPremiumSkinDemoTemporarily(skinName);
        }
    }


    /// <summary>
    /// These functions are so that when the user closes the tab, the skin doesnt stay
    /// in the demo section
    /// </summary>
    public void DisplayPremiumSkinDemoTemporarily(string skinName)
    {
        //store whether previous skin was premium
        previousSkinWasPremium = UserGameData.Instance.currentSkinPremium;

        //if it was, store the index of the child, and disable the current active premium, enable selected premium
        if (previousSkinWasPremium)
        {
            if (!premDemoCon.DemoObjects.TryGetValue(UserGameData.Instance.activePremiumSkinName, out previousDemoSkinObject))
            {
                throw new System.InvalidOperationException("PremiumSelectLogic: Cannot find: " + UserGameData.Instance.activePremiumSkinName + " In DemoPremiums");
            }

            //save index of previous skin then disable it (hide)
            previousPremiumSkinName = UserGameData.Instance.activePremiumSkinName;
            previousDemoSkinObject.SetActive(false);

        }
        else
        {
            //otherwise, hide the demo of the normal skin
            normalSkinParent.SetActive(false);
        }

        //if it was NOT, hide the normal skin demo then later re-enable it. 

        //update demo
        //get index of the new selected skin and show it in the demo
        UserGameData.Instance.activePremiumSkinName = skinName;
        if (!premDemoCon.DemoObjects.TryGetValue(UserGameData.Instance.activePremiumSkinName, out currentDemoSkinObject))
        {
            throw new System.InvalidOperationException("Cannot find: " + UserGameData.Instance.activePremiumSkinName + " In DemoPremiums");
        }
        currentDemoSkinObject.SetActive(true);
        premDemoCon.activePremiumSkin = currentDemoSkinObject.GetComponent<PremSkinDetailsDemo>();

        premDemoCon.activePremiumSkin.UpdateSkin();

        //update color choices to match
        glowColourSelector.UpdateColourOptionsForTrialingSkin(skinName);
    }

    /// <summary>
    /// Reverts back to the settings and display as before the premium skin was clicked.
    /// </summary>
    public void RevertToPreviousNormalSkin()
    {
        if (selectedUnownedPremium)
        {
            currentDemoSkinObject.SetActive(false);

            if (previousSkinWasPremium)
            {
                previousDemoSkinObject.SetActive(true);
                UserGameData.Instance.activePremiumSkinName = previousPremiumSkinName;
                premDemoCon.activePremiumSkin = previousDemoSkinObject.GetComponent<PremSkinDetailsDemo>();
            }
            else
            {
                normalSkinParent.SetActive(true);
                UserGameData.Instance.activePremiumSkinName = string.Empty;
                premDemoCon.activePremiumSkin = null;
            }
            
        }
    }

    private void DisplayBuyingTab(string skinName)
    {
        premiumNameText.text = skinName;
        premBuyCanvas.enabled = true;
        scaleTween.OnOpen();
        ff.FadeToBlack();
        cosMenuCon.buyCanvasOn = true;


        //Move in front of black filter
        premDemoParent.transform.SetParent(premBuyCanvas.gameObject.transform);
        premDemoParent.transform.SetSiblingIndex(1);
    }
    /// <summary>
    /// Sets the productid for the buy button based off of what skin was clicked.
    /// </summary>
    private void SetProductIDForBuyButton(string productid)
    {
        purchaseButton.GetComponent<BuyPremiumButton>().productID = productid;
    }

    private void SetProductPriceForBuyButton(string productid)
    {
        float price = PlatformIntegrations.IntegrationsManager.Instance.iapHandler.GetProductPrice(productid);
        priceText.text = "$" + price.ToString();
    }

    public void ClosingBuyingTab()
    {
        //moved back into place
        premDemoParent.transform.SetParent(cosMenuCon.gameObject.transform);
        premDemoParent.transform.SetSiblingIndex(1);
    }
}
