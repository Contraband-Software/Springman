using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

using Architecture.Managers;

public class PremiumButton : MonoBehaviour
{
    [SerializeField] Button thisButton;
    [SerializeField] ButtonMouseDown btnDownScript;
    [SerializeField] ButtonMouseDown btnDownScript2;
    [SerializeField] Canvas premBuyCanvas;
    [SerializeField] ScaleTween scaleTween;
    [SerializeField] FilterFade ff;
    [SerializeField] CosmeticsMenuController cosMenuCon;
    private bool currentSkinPrem_stored = false;


    [Header("Text References")]
    [SerializeField] TextMeshProUGUI customiseText;
    [SerializeField] TextMeshProUGUI customiseTextPressed;
    [SerializeField] TextMeshProUGUI selectText;
    [SerializeField] TextMeshProUGUI selectTextPressed;

    enum TextState { UNSET, CUSTOMISE, SELECT}
    TextState currentState = TextState.UNSET;

    private void Start()
    {
        premBuyCanvas.enabled = false;

        if (!UserGameData.Instance.currentSkinPremium)
        {
            ChangeToSaySelect();
        }
    }

    private void Update()
    {
        CheckWhatToSwitchTo();
    }

    private void CheckWhatToSwitchTo()
    {
        if (currentSkinPrem_stored == false && UserGameData.Instance.currentSkinPremium)
        {
            ChangeToSayCustomise();
        }
        if (currentSkinPrem_stored == true && UserGameData.Instance.currentSkinPremium == false)
        {
            ChangeToSaySelect();
        }
    }


    private void ChangeToSayCustomise()
    {
        currentSkinPrem_stored = true;
        thisButton.interactable = true;
        btnDownScript.disabledButton = false;
        btnDownScript2.disabledButton = false;

        //hide select texts
        if (currentState != TextState.CUSTOMISE)
        {
            customiseText.enabled = true;
            customiseTextPressed.enabled = true;
            selectText.enabled = false;
            selectTextPressed.enabled = false;

            currentState = TextState.CUSTOMISE;
        }
    }

    private void ChangeToSaySelect()
    {
        currentSkinPrem_stored = false;
        thisButton.interactable = false;
        btnDownScript.disabledButton = true;
        btnDownScript2.disabledButton = true;

        //hide customise texts
        if (currentState != TextState.SELECT)
        {
            customiseText.enabled = false;
            customiseTextPressed.enabled = false;
            selectText.enabled = true;
            selectTextPressed.enabled = true;

            currentState = TextState.SELECT;
        }
    }


    public void Clicked()
    {
        if (UserGameData.Instance.currentSkinPremium)
        {
            premBuyCanvas.enabled = true;
            scaleTween.OnOpen();
            ff.FadeToBlack();
            cosMenuCon.buyCanvasOn = true;


            //Ive selected a skin which I have already bought. 
        }
    }
}
