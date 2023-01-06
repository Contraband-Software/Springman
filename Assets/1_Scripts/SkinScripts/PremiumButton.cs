using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

using Architecture.Managers;

public class PremiumButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Button thisButton;
    [SerializeField] ButtonMouseDown btnDownScript;
    [SerializeField] Canvas premBuyCanvas;
    [SerializeField] ScaleTween scaleTween;
    [SerializeField] FilterFade ff;
    [SerializeField] CosmeticsMenuController cosMenuCon;
    private bool currentSkinPrem_stored = false;

    private void Start()
    {
        premBuyCanvas.enabled = false;
    }

    private void Update()
    {
        if(currentSkinPrem_stored == false && UserGameData.Instance.currentSkinPremium)
        {
            ChangeToSayCustomise();
        }
        if(currentSkinPrem_stored == true && UserGameData.Instance.currentSkinPremium == false)
        {
            ChangeToSaySelect();
        }
    }


    private void ChangeToSayCustomise()
    {
        currentSkinPrem_stored = true;
        text.text = "Customise";
        thisButton.interactable = true;
        btnDownScript.disabledButton = false;
    }

    private void ChangeToSaySelect()
    {
        currentSkinPrem_stored = false;
        text.text = "Select A Skin";
        thisButton.interactable = false;
        btnDownScript.disabledButton = true;
    }


    public void Clicked()
    {
        if (UserGameData.Instance.currentSkinPremium)
        {
            premBuyCanvas.enabled = true;
            scaleTween.OnOpen();
            ff.FadeToBlack();
            cosMenuCon.buyCanvasOn = true;
        }
    }
}
