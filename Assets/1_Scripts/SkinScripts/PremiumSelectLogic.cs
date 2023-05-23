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

    [Header("States (Mutables I know, cringe)")]
    [SerializeField] bool previousSkinWasPremium = false;
    private bool selectedUnownedPremium = false;
    private int previousPremiumSkinIndex;
    private int premiumChildIndexClicked;
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

        if (!ff.filterImage.raycastTarget)
        {
            DisplayBuyingTab(skinName);
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
            //save index of previous skin then disable it (hide)
            previousPremiumSkinIndex = UserGameData.Instance.allPremiums.IndexOf(UserGameData.Instance.activePremiumSkinName);
            previousPremiumSkinName = UserGameData.Instance.activePremiumSkinName;
            premDemoParent.transform.GetChild(previousPremiumSkinIndex).gameObject.SetActive(false);

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
        premiumChildIndexClicked = UserGameData.Instance.allPremiums.IndexOf(UserGameData.Instance.activePremiumSkinName);
        premDemoParent.transform.GetChild(premiumChildIndexClicked).gameObject.SetActive(true);
        premDemoCon.activePremiumSkin = premDemoParent.transform.GetChild(premiumChildIndexClicked).GetComponent<PremSkinDetailsDemo>();

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
            premDemoParent.transform.GetChild(premiumChildIndexClicked).gameObject.SetActive(false);

            if (previousSkinWasPremium)
            {
                premDemoParent.transform.GetChild(previousPremiumSkinIndex).gameObject.SetActive(true);
                UserGameData.Instance.activePremiumSkinName = previousPremiumSkinName;
                premDemoCon.activePremiumSkin = premDemoParent.transform.GetChild(previousPremiumSkinIndex).GetComponent<PremSkinDetailsDemo>();
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

    public void ClosingBuyingTab()
    {
        //moved back into place
        premDemoParent.transform.SetParent(cosMenuCon.gameObject.transform);
        premDemoParent.transform.SetSiblingIndex(1);
    }
}
