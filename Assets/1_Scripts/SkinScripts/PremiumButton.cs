using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PremiumButton : MonoBehaviour
{
    [SerializeField] CosmeticsData cosData;
    [SerializeField] TextMeshProUGUI text;
    private bool currentSkinPrem_stored = false;


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
    }

    private void ChangeToSaySelect()
    {
        currentSkinPrem_stored = false;
        text.text = "Select A Skin";
    }
}
