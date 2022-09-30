using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinSelector_Premium : MonoBehaviour
{
    [Header("Important References")]
    public CosmeticsData cosmeticsData;
    public SkinsController skinsCon;
    public CosmeticsMenuController cosMenuCon;

    [Header("For Controlling Selection")]
    public Color onSelectColour;
    public RectTransform masterCanvasRect;
    public RectTransform maskRect;
    public ScrollRect scroller;
    public bool scrolled;
    public bool tabSkinsCollected = false;

    public List<PremiumSkinIcon> premSkinIcons = new List<PremiumSkinIcon>();
    public List<string> premTabSkinCodes = new List<string>();
    private List<string> premTabSkinNames = new List<string>();

    public PremiumSkinIcon selectedSkin;
    public Vector3 selectedObject;

    public GameObject premiumDemosParent;

    private void Start()
    {
        tabSkinsCollected = false;

        CollectSkins();
    }

    public void Update()
    {
        if (gameObject.activeSelf == true)
        {
            CheckForClick();
        }
    }

    //do this on the first time the page is opened
    public void CollectSkins()
    {
        if (tabSkinsCollected == false)
        {
            for (int child = 1; child < transform.childCount; child++)
            {
                PremiumSkinIcon premSI = transform.GetChild(child).GetChild(0).GetComponent<PremiumSkinIcon>();
                premSkinIcons.Add(premSI);
                premTabSkinCodes.Add(premSI.ID);
                premTabSkinNames.Add(premSI.skin_name);
            }
            cosmeticsData.allPremiums = premTabSkinNames;
            cosmeticsData.allPremiumCodes = premTabSkinCodes;
            tabSkinsCollected = true;
        }
    }

    public void CollectGlowColours()
    {
        print("COLLECTING GLOW COLOURS");

        List<string> glowColoursGathered = new List<string>();
        List<bool> hasSpecialColourGathered = new List<bool>();
        List<bool> sCMgathered = new List<bool>();
        for (int child = 0; child < premiumDemosParent.transform.childCount; child++)
        {
            PremSkinDetailsDemo premDemo = premiumDemosParent.transform.GetChild(child).gameObject.GetComponent<PremSkinDetailsDemo>();
            glowColoursGathered.Add(cosmeticsData.ColorToString(premDemo.targetColor));
            //print(premDemo.name + ": "+ cosmeticsData.ColorToString(premDemo.targetColor));

            hasSpecialColourGathered.Add(premDemo.hasSpecialColourMode);
            sCMgathered.Add(premDemo.colourShift);
        }
        cosmeticsData.glowColours = glowColoursGathered;
        cosmeticsData.hasSpecialColour = hasSpecialColourGathered;
        cosmeticsData.specialColourModes = sCMgathered;
        //ALSO GATHERS SPECIAL COLOUR EFFECTS
    }
    public void CollectSpecialColourSettings()
    {
        List<bool> hasSpecialColourGathered = new List<bool>();
        for (int child = 0; child < premiumDemosParent.transform.childCount; child++)
        {
            PremSkinDetailsDemo premDemo = premiumDemosParent.transform.GetChild(child).gameObject.GetComponent<PremSkinDetailsDemo>();
            hasSpecialColourGathered.Add(premDemo.hasSpecialColourMode);
        }
    }

    public void SetAllGlowColours()
    {
        for (int child = 0; child < premiumDemosParent.transform.childCount; child++)
        {
            PremSkinDetailsDemo premDemo = premiumDemosParent.transform.GetChild(child).gameObject.GetComponent<PremSkinDetailsDemo>();
            premDemo.targetColor = cosmeticsData.StringToColor(cosmeticsData.glowColours[child]);
            premDemo.hasSpecialColourMode = cosmeticsData.hasSpecialColour[child];
            premDemo.colourShift = cosmeticsData.specialColourModes[child];
        }
    }

    private void CheckForClick()
    {
        Vector3 thisClick;
        Touch touch;

        if (Input.GetMouseButtonDown(0))
        {
            thisClick = Input.mousePosition;

            if (InMaskBounds(thisClick))
            {
                scroller.inertia = false;
                scroller.inertia = true;

                WhichSkinClicked(thisClick);
            }
            scrolled = false;
        }
        if (Input.GetMouseButtonUp(0))
        {
            thisClick = Input.mousePosition;

            PremiumSkinIcon prevSelSkin = selectedSkin;
            Vector3 prevSelObj = selectedObject;

            if (scrolled)
            {
                selectedSkin = prevSelSkin;
                selectedObject = prevSelObj;

            }
            else
            {
                if (InMaskBounds(thisClick))
                {
                    WhichSkinClicked(thisClick);

                    if (prevSelSkin == selectedSkin && prevSelObj == selectedObject)
                    {
                        SendSkinDetailsToController();
                        
                    }

                    scrolled = false;
                }
            }
        }

        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                if (InMaskBounds(touch.position))
                {
                    scroller.inertia = false;
                    scroller.inertia = true;

                    WhichSkinClicked(touch.position);
                }
                scrolled = false;
            }

            if (touch.phase == TouchPhase.Ended)
            {

                PremiumSkinIcon prevSelSkin = selectedSkin;
                Vector3 prevSelObj = selectedObject;

                if (scrolled)
                {
                    selectedSkin = prevSelSkin;
                    selectedObject = prevSelObj;

                }
                else
                {
                    if (InMaskBounds(touch.position))
                    {

                        WhichSkinClicked(touch.position);
                        
                        if (prevSelSkin == selectedSkin && prevSelObj == selectedObject)
                        {
                            SendSkinDetailsToController();
                        }

                        scrolled = false;
                    }
                }
            }
        }
    }

    private bool InMaskBounds(Vector3 clickPos)
    {
        bool withinX = false;
        bool withinY = false;

        if (clickPos.y >= maskRect.position.y - ((maskRect.sizeDelta.y * masterCanvasRect.localScale.y) / 2f) &&
            clickPos.y <= maskRect.position.y + ((maskRect.sizeDelta.y * masterCanvasRect.localScale.y) / 2f))
        {
            withinY = true;
        }
        if (clickPos.x >= maskRect.position.x - ((maskRect.sizeDelta.x * masterCanvasRect.localScale.x) / 2f) &&
            clickPos.x <= maskRect.position.x + ((maskRect.sizeDelta.x * masterCanvasRect.localScale.x) / 2f))
        {
            withinX = true;
        }

        if (withinX && withinY && !cosMenuCon.buyCanvasOn && !cosMenuCon.allBoughtCanvasOn && !cosMenuCon.brokeCanvasOn)
        {
            return true;
        }
        else
        {
            return false;
        }

        //Add this in once fully able to select
        /*
        if (withinX && withinY && !buyCanvas.gameObject.activeSelf && !allBoughtCanvas.gameObject.activeSelf && !brokeCanvas.activeSelf)
        {
            return true;
        }
        else
        {
            return false;
        }
        */
    }

    private void WhichSkinClicked(Vector3 clickPos)
    {
        if (premSkinIcons.Count > 0)
        {
            foreach (PremiumSkinIcon sb in premSkinIcons)
            {
                if (clickPos.y >= sb.rect.position.y - ((sb.rect.sizeDelta.y * masterCanvasRect.localScale.y * sb.rect.localScale.y) / 2f) &&
                    clickPos.y <= sb.rect.position.y + ((sb.rect.sizeDelta.y * masterCanvasRect.localScale.y * sb.rect.localScale.y) / 2f))
                {
                    if (clickPos.x >= sb.rect.position.x - ((sb.rect.sizeDelta.x * masterCanvasRect.localScale.x * sb.rect.localScale.x) / 2f) &&
                        clickPos.x <= sb.rect.position.x + ((sb.rect.sizeDelta.x * masterCanvasRect.localScale.x * sb.rect.localScale.x) / 2f))
                    {
                        selectedSkin = sb;
                        selectedObject = sb.rect.anchoredPosition;

                        //Debug.Log("SELECTED: " + selectedSkin.skin_name);
                    }
                }
            }
        }
    }

    
    public void SendSkinDetailsToController()
    {
        if (cosmeticsData.unlockedPremiums.Contains(selectedSkin.ID))
        {
            cosmeticsData.currentSkinPremium = true;

            skinsCon.currentObject = selectedObject;
            skinsCon.currentSkinID = selectedSkin.ID;
            skinsCon.selectedPremium = selectedSkin.skin_name;

            if (skinsCon.prevSelectedImage != null)
            {
                skinsCon.prevSelectedImage.color = Color.white;
            }
            skinsCon.prevSelectedImage = selectedSkin.gameObject.GetComponent<Image>();
            skinsCon.prevSelectedImage.color = onSelectColour;

            skinsCon.SendData();
            cosMenuCon.UpdateDemo();
        }
    }
    

    public void InterruptSelection()
    {
        scrolled = true;
    }

    private bool InMaskBoundsPadded(Vector3 pos)
    {
        if (pos.y >= maskRect.position.y - ((maskRect.sizeDelta.y * 1.2f * masterCanvasRect.localScale.y))
            && pos.y <= maskRect.position.y + ((maskRect.sizeDelta.y * 1.2f * masterCanvasRect.localScale.y)))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void DisableMasked()
    {
        foreach (PremiumSkinIcon button in premSkinIcons)
        {
            if (InMaskBoundsPadded(button.transform.position))
            {
                button.transform.parent.gameObject.SetActive(true);
            }
            else
            {
                button.transform.parent.gameObject.SetActive(false);
            }
        }
    }
}
