using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GlowColourSelector : MonoBehaviour
{
    [SerializeField] private List<GameObject> coloursButtons = new List<GameObject>();


    [SerializeField] private PremiumDemoContoller premDemoCon;
    [SerializeField] private CosmeticsData cosData;

    [SerializeField] private GameObject colourChoiceParent;
    private int currentOptionsCount;

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

        int skinColourChoicesCount = premDemoCon.activePremiumSkin.colorChoices.Count;
        bool hasSpecialColourMode = false;

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
