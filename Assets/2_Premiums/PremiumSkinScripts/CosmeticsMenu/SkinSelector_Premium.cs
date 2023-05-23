using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlatformIntegrations;

using Architecture.Managers;
using Backend;

public class SkinSelector_Premium : MonoBehaviour
{
    [Header("Important References")]
    public SkinsController skinsCon;
    public CosmeticsMenuController cosMenuCon;
    PremiumSelectLogic premSelectLogic;

    [Header("For Controlling Selection")]
    public Color onSelectColour;
    public RectTransform masterCanvasRect;
    public RectTransform maskRect;
    public ScrollRect scroller;
    public bool scrolled;
    public bool tabSkinsCollected = false;

    public List<PremiumSkinIcon> premSkinIcons = new List<PremiumSkinIcon>();

    public PremiumSkinIcon selectedSkin;
    public Vector3 selectedObject;

    public GameObject premiumDemosParent;

    private void Start()
    {
        //hand UserGameData info on Glow colours and special modes
        UserGameData.Instance.RequestColourData.AddListener(CollectGlowColours);
        tabSkinsCollected = false;

        CollectSkins();

        UserGameData.Instance.CheckIfLoadedSkinPremium();
        premSelectLogic = PremiumSelectLogic.GetReference();
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
            }
            tabSkinsCollected = true;

            RemoveLockIconOnOwnedSkins();
        }
    }

    /// <summary>
    /// This will remove the icon on any skins that are owned
    /// </summary>
    public void RemoveLockIconOnOwnedSkins()
    {
        foreach (PremiumSkinIcon premSI in premSkinIcons)
        {
            if (UserGameData.Instance.allPremiumCodes.Contains(premSI.ID)){
                premSI.gameObject.transform.parent.transform.GetChild(3).gameObject.GetComponent<Image>().enabled = false;
            }
        }
    }


    /// <summary>
    /// Collects the data from the each premium skin on what colour they had, if they have any special modes etc
    /// 
    /// Gives data to UserGameData to store
    /// 
    /// Say you set the colour of a premium glow to be purple, UserGameData stores purple in its index for that skin.
    /// </summary>
    public void CollectGlowColours()
    {
        print("COLLECTING GLOW COLOURS");

        List<string> glowColoursGathered = new List<string>();
        List<bool> hasSpecialColourGathered = new List<bool>();
        List<bool> sCMgathered = new List<bool>();
        for (int child = 0; child < premiumDemosParent.transform.childCount; child++)
        {
            PremSkinDetailsDemo premDemo = premiumDemosParent.transform.GetChild(child).gameObject.GetComponent<PremSkinDetailsDemo>();
            glowColoursGathered.Add(Utilities.ColorToString(premDemo.targetColor));
            //print(premDemo.name + ": "+ UserGameData.Instance.ColorToString(premDemo.targetColor));

            hasSpecialColourGathered.Add(premDemo.hasSpecialColourMode);
            sCMgathered.Add(premDemo.colourShift);
        }
        UserGameData.Instance.glowColours = glowColoursGathered;
        UserGameData.Instance.hasSpecialColour = hasSpecialColourGathered;
        UserGameData.Instance.specialColourModes = sCMgathered;
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

    /// <summary>
    /// Sets the premium skins to have the attributes that were saved in the file.
    /// </summary>
    public void SetAllGlowColours()
    {
        print("SetAllGlowColours()");
        for (int child = 0; child < premiumDemosParent.transform.childCount; child++)
        {
            PremSkinDetailsDemo premDemo = premiumDemosParent.transform.GetChild(child).gameObject.GetComponent<PremSkinDetailsDemo>();
            if(UserGameData.Instance.glowColours.Count > child)
            {
                premDemo.targetColor = Utilities.StringToColor(UserGameData.Instance.glowColours[child]);
            }
            else
            {
                UserGameData.Instance.glowColours.Add(Utilities.ColorToString(premDemo.targetColor));
            }

            if (UserGameData.Instance.hasSpecialColour.Count > child)
            {
                premDemo.hasSpecialColourMode = UserGameData.Instance.hasSpecialColour[child];
            }
            else
            {
                UserGameData.Instance.hasSpecialColour.Add(premDemo.hasSpecialColourMode);
            }

            if (UserGameData.Instance.specialColourModes.Count > child)
            {
                premDemo.colourShift = UserGameData.Instance.specialColourModes[child];
            }
            else
            {
                UserGameData.Instance.specialColourModes.Add(premDemo.colourShift);
            }
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
        if (IntegrationsManager.Instance.iapHandler.purchasedProducts.Contains(selectedSkin.ID))
        {
            UserGameData.Instance.currentSkinPremium = true;

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
        else
        {
            //NOT OWNED
            premSelectLogic.SelectedUnOwnedSkin(selectedSkin.skin_name, selectedSkin.ID);
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
