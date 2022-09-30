using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PremiumButton : MonoBehaviour
{
    [SerializeField] CosmeticsData cosData;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Button thisButton;
    [SerializeField] ButtonMouseDown btnDownScript;
    [SerializeField] Canvas premBuyCanvas;
    [SerializeField] ScaleTween scaleTween;
    private bool currentSkinPrem_stored = false;

    private void Start()
    {
        premBuyCanvas.enabled = false;
    }

    private void Update()
    {
        if(currentSkinPrem_stored == false && cosData.currentSkinPremium)
        {
            ChangeToSayCustomise();
        }
        if(currentSkinPrem_stored == true && cosData.currentSkinPremium == false)
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
        if (cosData.currentSkinPremium)
        {
            premBuyCanvas.enabled = true;
            scaleTween.OnOpen();
        }
    }
}
