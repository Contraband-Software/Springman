using Architecture;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyButton : MonoBehaviour
{
    public Canvas canvasToOpen;
    public Canvas brokeBoyCanvas;
    public Canvas allBoughtCanvas;
    public FilterFade brokeFilter;
    public CosmeticsMenuController cosMenuCon;
    [Header("Purchase Details")]
    public float cost;
    public enum PurchaseType { Color, Silver, Gold};
    public PurchaseType purchaseType;
    public ConfirmBuyColour confirmBuyColor;
    //public MenuData menuData;
    bool defaultsSet = false;

    public void Start()
    {
        canvasToOpen.enabled = false;
        brokeBoyCanvas.enabled = false;
        allBoughtCanvas.enabled = false;
        
    }

    public void OnClick()
    {

        if (purchaseType == PurchaseType.Color && UserGameData.Instance.silver >= cost)
        {
            confirmBuyColor.PopulatePossiblePurchases();
            if(confirmBuyColor.possiblePurchases.Count > 0)
            {
                canvasToOpen.enabled = true;
                cosMenuCon.buyCanvasOn = true;
                if (defaultsSet)
                {
                    confirmBuyColor.ResetToDefaults();
                    confirmBuyColor.StopLerping();
                    confirmBuyColor.stopTweens();
                    canvasToOpen.gameObject.transform.GetChild(1).GetComponent<ScaleTween>().OnOpen();
                }
                else
                {
                    confirmBuyColor.SetOriginals();
                    defaultsSet = true;
                    canvasToOpen.gameObject.transform.GetChild(1).GetComponent<ScaleTween>().OnOpen();
                }
            }
            else
            {
                allBoughtCanvas.enabled = true;
                cosMenuCon.allBoughtCanvasOn = true;
                allBoughtCanvas.gameObject.transform.GetChild(1).GetComponent<ScaleTween>().OnOpen();
                //tell that everything bought
            }
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
