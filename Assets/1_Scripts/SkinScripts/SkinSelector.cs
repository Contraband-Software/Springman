using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Architecture.Managers;

public class SkinSelector : MonoBehaviour
{
    public RectTransform masterCanvasRect;
    public RectTransform maskRect;
    public SkinsController skinsCon;
    public CosmeticsMenuController cosMenuCon;

    public GameObject buyCanvas;
    public GameObject allBoughtCanvas;
    public GameObject brokeCanvas;

    public Color onSelectColour;

    public SkinSpecs selectedSkin;
    public Vector3 selectedObject;

    public ScrollRect scroller;
    public bool scrolled;

    public List<SkinButton> skinButtonScripts = new List<SkinButton>();
    public List<string> tabSkins = new List<string>();
    public List<string> tabSkinCodes = new List<string>();
    public bool tabSkinsCollected = false;

    private void Start()
    {
        tabSkinsCollected = false;
    }

    private void Update()
    {

        if (gameObject.activeSelf == true)
        {
            CheckForClick();
        }
    }

    //do this on the first time the page is opened
    public void CollectSkins()
    {
        if (!tabSkinsCollected)
        {
            List<SkinSpecsSolid> tabSkinSpecs = new List<SkinSpecsSolid>();
            for (int child = 1; child < transform.childCount; child++)
            {
                skinButtonScripts.Add(transform.GetChild(child).GetComponent<SkinButton>());
                skinButtonScripts[child - 1].AssignValues();

                tabSkins.Add(skinButtonScripts[child - 1].skinSpecs.skin_name);
                tabSkinCodes.Add(skinButtonScripts[child - 1].skinSpecs.ID);
                SkinSpecs collectedSkin = skinButtonScripts[child - 1].skinSpecs;
                SkinSpecsSolid copiedSpecs = new SkinSpecsSolid();

                //deep copying the specs
                copiedSpecs.ID = collectedSkin.ID;
                copiedSpecs.skin_name = collectedSkin.skin_name;
                copiedSpecs.demoSkin = collectedSkin.demoSkin;
                copiedSpecs.base_Top = collectedSkin.base_Top;
                copiedSpecs.base_Bottom = collectedSkin.base_Bottom;
                copiedSpecs.eyes = collectedSkin.eyes;
                copiedSpecs.eyes_death = collectedSkin.eyes_death;
                copiedSpecs.alt_base = collectedSkin.alt_base;
                copiedSpecs.alt_Base_Sprite = collectedSkin.alt_Base_Sprite;
                copiedSpecs.colour_changeable_top = collectedSkin.colour_changeable_top;
                copiedSpecs.colour_changeable_bottom = collectedSkin.colour_changeable_bottom;
                copiedSpecs.colour_changeable_eyes = collectedSkin.colour_changeable_eyes;
                copiedSpecs.colour_top_equal_to_bottom = collectedSkin.colour_top_equal_to_bottom;
                copiedSpecs.skin_Top = collectedSkin.skin_Top;
                copiedSpecs.skin_Bottom = collectedSkin.skin_Bottom;
                copiedSpecs.skin_Top_Flippable = collectedSkin.skin_Top_Flippable;
                copiedSpecs.spring_sprite = collectedSkin.spring_sprite;
                copiedSpecs.demo_spring_sprite = collectedSkin.demo_spring_sprite;
                copiedSpecs.bounce_anim = collectedSkin.bounce_anim;
                copiedSpecs.alt_BounceSound = collectedSkin.alt_BounceSound;

                //
                tabSkinSpecs.Add(copiedSpecs);
            }
            UserGameData.Instance.allSkins.AddRange(tabSkins);
            UserGameData.Instance.allSkinSpecs.AddRange(tabSkinSpecs);
            UserGameData.Instance.allSkinsCodes.AddRange(tabSkinCodes);
            tabSkinsCollected = true;
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

            SkinSpecs prevSelSkin = selectedSkin;
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
                    //if a skin is actualy clicked
                    if (WhichSkinClicked(thisClick))
                    {
                        if (prevSelSkin == selectedSkin && prevSelObj == selectedObject)
                        {
                            SendSkinDetailsToController();
                        }

                        scrolled = false;
                    }
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

                SkinSpecs prevSelSkin = selectedSkin;
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
                        //if actually clicked on a skin
                        if (WhichSkinClicked(touch.position)){

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
    }

    private bool WhichSkinClicked(Vector3 clickPos)
    {
        if (skinButtonScripts.Count > 0)
        {
            foreach (SkinButton sb in skinButtonScripts)
            {
                if (clickPos.y >= sb.rect.position.y - ((sb.rect.sizeDelta.y * masterCanvasRect.localScale.y * sb.rect.localScale.y) / 2f) &&
                    clickPos.y <= sb.rect.position.y + ((sb.rect.sizeDelta.y * masterCanvasRect.localScale.y * sb.rect.localScale.y) / 2f))
                {
                    if (clickPos.x >= sb.rect.position.x - ((sb.rect.sizeDelta.x * masterCanvasRect.localScale.x * sb.rect.localScale.x) / 2f) &&
                        clickPos.x <= sb.rect.position.x + ((sb.rect.sizeDelta.x * masterCanvasRect.localScale.x * sb.rect.localScale.x) / 2f))
                    {
                        selectedSkin = sb.skinSpecs;
                        //print(selectedSkin.altBounceSound);
                        selectedObject = sb.rect.anchoredPosition;

                        return true;
                    }
                }
            }
            return false;
        }
        return false;
    }

    public void SendSkinDetailsToController()
    {
        if (UserGameData.Instance.unlockedSkins.Contains(selectedSkin.ID))
        {
            UserGameData.Instance.currentSkinPremium = false;


            skinsCon.currentSkin = selectedSkin;
            //print(skinsCon.currentSkin.altBounceSound);
            skinsCon.currentObject = selectedObject;
            skinsCon.currentSkinID = selectedSkin.ID;

            if (skinsCon.prevSelectedImage != null)
            {
                skinsCon.prevSelectedImage.color = Color.white;
            }
            skinsCon.prevSelectedImage = selectedSkin.gameObject.GetComponent<Image>();
            skinsCon.prevSelectedImage.color = onSelectColour;

            skinsCon.SendData();
            UserGameData.Instance.activePremiumSkinName = null;
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
        foreach (SkinButton button in skinButtonScripts)
        {
            if (InMaskBoundsPadded(button.transform.position))
            {
                button.gameObject.SetActive(true);
            }
            else
            {
                button.gameObject.SetActive(false);
            }
        }
    }
}
