using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GlowColourSelector : MonoBehaviour
{
    [SerializeField] private List<GameObject> coloursButtons = new List<GameObject>();
    [SerializeField] private List<Image> colourBtnImages = new List<Image>();


    [SerializeField] private PremiumDemoContoller premDemoCon;
    [SerializeField] private CosmeticsData cosData;

    [SerializeField] private GameObject colourChoiceParent;
    [SerializeField] private Sprite colourShiftImage;
    [SerializeField] private Sprite basicBtnImage;
    private int currentOptionsCount;
    private string lastPremiumName = null;

    private void Start()
    {
        currentOptionsCount = colourChoiceParent.transform.childCount;
    }


    //grabs premDetails and sets all colour options to match premiums colour variants
    public void UpdateColourOptions()
    {
        if (!cosData.currentSkinPremium)
        {
            return;
        }

        //if different skin to last time, change potentially colour shift button sprite back to normal
        if (lastPremiumName != null && lastPremiumName != cosData.activePremiumSkinName)
        {
            colourBtnImages[currentOptionsCount - 1].sprite = basicBtnImage;
            colourBtnImages[currentOptionsCount - 1].color = Color.white;
        }
        lastPremiumName = cosData.activePremiumSkinName;

        int skinColourChoicesCount = premDemoCon.activePremiumSkin.colorChoices.Count;
        bool hasSpecialColourMode = false;
        lastPremiumName = cosData.activePremiumSkinName;

        if (premDemoCon.activePremiumSkin.hasSpecialColourMode)
        {
            hasSpecialColourMode = true;
            skinColourChoicesCount++;
        }

        print(skinColourChoicesCount);
        print(currentOptionsCount);


        //Current skin has less colour options than previous
        if(skinColourChoicesCount < currentOptionsCount)
        {
            //disables all gameobjects above the max option
            for(int i = skinColourChoicesCount; i <= currentOptionsCount-1; i++)
            {
                print(i);
                colourChoiceParent.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        //previous skin has more colour options than now selected skin
        if(skinColourChoicesCount > currentOptionsCount)
        {
            //enables all gameobjects from the current max to the new skin max
            for (int i = currentOptionsCount; i <= skinColourChoicesCount - 1; i++)
            {
                print(i);
                colourChoiceParent.transform.GetChild(i).gameObject.SetActive(true);
            }
        }

        //Updates the colours to show the options for the skin
        for (int i = 0; i < premDemoCon.activePremiumSkin.colorChoices.Count; i++)
        {
            colourBtnImages[i].color = premDemoCon.activePremiumSkin.colorChoices[i];
        }
        if (hasSpecialColourMode)
        {
            print("HAS SPECIAL MODE");
            colourBtnImages[skinColourChoicesCount - 1].color = Color.white;
            colourBtnImages[skinColourChoicesCount - 1].sprite = colourShiftImage;
        }


        //finally sets the currentOptionsCount to whatever it is now to use in next click
        currentOptionsCount = skinColourChoicesCount;
    }



    public void ColourClicked()
    {
        //get color of button
        //convert to a string to swap into glow colours array
        //feed into the premdetails and update the demo

    }
}
