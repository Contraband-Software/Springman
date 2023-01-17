using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Architecture.Managers;

public class WaterSelector : MonoBehaviour
{
    [Header("Refs")]
    public RectTransform masterCanvasRect;
    public CosmeticsMenuController cosMenuCon;

    [Header("Buttons Ref")]
    public List<WaterColourBtn> colourButtonScripts = new List<WaterColourBtn>();

    [Header("Scrolling")]
    public RectTransform maskRect;
    public ScrollRect waterScroll;
    public bool scrolled = false;

    //to do wit clicking
    Color selectedColor;
    Vector3 selectedObject;

    private void Start()
    {
        CollectColours();
    }

    private void Update()
    {
        if (gameObject.activeSelf == true)
        {
            CheckForClick();
        }
    }

    //do this on the first time the page is opened
    public void CollectColours()
    {
        for (int child = 1; child < transform.childCount; child++)
        {
            colourButtonScripts.Add(transform.GetChild(child).GetComponent<WaterColourBtn>());
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
                waterScroll.inertia = false;
                waterScroll.inertia = true;

                WhichColourClicked(thisClick);
            }
            scrolled = false;
        }
        if (Input.GetMouseButtonUp(0))
        {
            thisClick = Input.mousePosition;

            Color prevSelCol = selectedColor;
            Vector3 prevSelObj = selectedObject;

            if (scrolled)
            {
                selectedColor = prevSelCol;
                selectedObject = prevSelObj;

            }
            else
            {
                if (InMaskBounds(thisClick))
                {
                    WhichColourClicked(thisClick);

                    if (prevSelCol == selectedColor && prevSelObj == selectedObject)
                    {
                        SendColorToUserGameData();
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
                    waterScroll.inertia = false;
                    waterScroll.inertia = true;

                    WhichColourClicked(touch.position);
                }
                scrolled = false;
            }

            if (touch.phase == TouchPhase.Ended)
            {

                Color prevSelCol = selectedColor;
                Vector3 prevSelObj = selectedObject;

                if (scrolled)
                {
                    selectedColor = prevSelCol;
                    selectedObject = prevSelObj;

                }
                else
                {
                    if (InMaskBounds(touch.position))
                    {
                        WhichColourClicked(touch.position);

                        if (prevSelCol == selectedColor && prevSelObj == selectedObject)
                        {
                            SendColorToUserGameData();
                        }

                        scrolled = false;
                    }
                }
            }
        }
    }

    private void SendColorToUserGameData()
    {
        UserGameData.Instance.themeColour = selectedColor;
        PanoramaBackground.GetReference().UpdateBackgroundColours();
    }


    public void InterruptSelection()
    {
        scrolled = true;
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
        foreach (WaterColourBtn button in colourButtonScripts)
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

    private void WhichColourClicked(Vector3 clickPos)
    {
        if (colourButtonScripts.Count > 0)
        {
            foreach (WaterColourBtn cb in colourButtonScripts)
            {
                if (clickPos.y >= cb.rect.position.y - ((cb.rect.sizeDelta.y * masterCanvasRect.localScale.y * cb.rect.localScale.y) / 2f) &&
                    clickPos.y <= cb.rect.position.y + ((cb.rect.sizeDelta.y * masterCanvasRect.localScale.y * cb.rect.localScale.y) / 2f))
                {
                    if (clickPos.x >= cb.rect.position.x - ((cb.rect.sizeDelta.x * masterCanvasRect.localScale.x * cb.rect.localScale.x) / 2f) &&
                        clickPos.x <= cb.rect.position.x + ((cb.rect.sizeDelta.x * masterCanvasRect.localScale.x * cb.rect.localScale.x) / 2f))
                    {
                        selectedColor = cb.storedColor;
                        selectedObject = cb.rect.anchoredPosition;
                    }
                }
            }
        }
    }
}
