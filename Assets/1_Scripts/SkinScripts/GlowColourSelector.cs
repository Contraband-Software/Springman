using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Architecture.Managers;

public class GlowColourSelector : MonoBehaviour
{
    [SerializeField] private List<GameObject> coloursButtons = new List<GameObject>();
    [SerializeField] private List<Image> colourBtnImages = new List<Image>();


    [SerializeField] private PremiumDemoContoller premDemoCon;

    [SerializeField] private GameObject colourChoiceParent;
    [SerializeField] private Sprite colourShiftImage;
    [SerializeField] private Sprite basicBtnImage;
    private int currentOptionsCount;
    private string lastPremiumName = null;


    //TEST
    public Color colorChosen;

    private void Start()
    {
        currentOptionsCount = colourChoiceParent.transform.childCount;
    }


    //grabs premDetails and sets all colour options to match premiums colour variants
    public void UpdateColourOptions()
    {
        if (!UserGameData.Instance.currentSkinPremium)
        {
            return;
        }

        //if different skin to last time, change potentially colour shift button sprite back to normal
        if (lastPremiumName != null && lastPremiumName != UserGameData.Instance.activePremiumSkinName)
        {
            colourBtnImages[currentOptionsCount - 1].sprite = basicBtnImage;
            colourBtnImages[currentOptionsCount - 1].color = Color.white;
        }
        lastPremiumName = UserGameData.Instance.activePremiumSkinName;

        int skinColourChoicesCount = premDemoCon.activePremiumSkin.colorChoices.Count;
        bool hasSpecialColourMode = false;
        lastPremiumName = UserGameData.Instance.activePremiumSkinName;

        if (premDemoCon.activePremiumSkin.hasSpecialColourMode)
        {
            hasSpecialColourMode = true;
            skinColourChoicesCount++;
        }


        //Current skin has less colour options than previous
        if(skinColourChoicesCount < currentOptionsCount)
        {
            //disables all gameobjects above the max option
            for(int i = skinColourChoicesCount; i <= currentOptionsCount-1; i++)
            {
                colourChoiceParent.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        //previous skin has more colour options than now selected skin
        if(skinColourChoicesCount > currentOptionsCount)
        {
            //enables all gameobjects from the current max to the new skin max
            for (int i = currentOptionsCount; i <= skinColourChoicesCount - 1; i++)
            {
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



    public void ColourClicked(ColorBtnPremium colBtn)
    {
        //get color of button
        //convert to a string to swap into glow colours array
        //feed into the premdetails and update the demo

        int indexOfPremium = UserGameData.Instance.allPremiums.IndexOf(UserGameData.Instance.activePremiumSkinName);
        if (!colBtn.isColorShiftButton())
        {

            if (premDemoCon.activePremiumSkin.hasSpecialColourMode)
            {
                UserGameData.Instance.specialColourModes[indexOfPremium] = false;
                premDemoCon.activePremiumSkin.colourShift = false;
            }


            colorChosen = colBtn.getButtonColour();
            UserGameData.Instance.glowColours[indexOfPremium] = UserGameData.Instance.ColorToString(colorChosen); 

            premDemoCon.activePremiumSkin.UpdateSkin();
        }
        else
        {
            UserGameData.Instance.specialColourModes[indexOfPremium] = true;
            premDemoCon.activePremiumSkin.colourShift = true;
        }
    }
}
