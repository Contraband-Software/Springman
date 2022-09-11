using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCanvas : MonoBehaviour
{
    public CosmeticsMenuController cosMenuCon;
    public SkinsController skinsCon;

    public Transform cosmeticsCanvas;
    public RectTransform allCanvases;

    public Canvas activeCanvas;

    public bool decrement = false;
    public bool increment = false;

    float targetX;

    public LeanTweenType slide;

    //THINGS DONE: Gold tab (last tab) will move to the back of the queue (-800)
    //This must then lerp to the startCanvas position, and the start canvas position to x + 800. the other tabs should be premoved to x + 800 also but without lerp

    //FOCUS ON MOVING ALL THE TABS RIGHT PROPERLY

    private void Start()
    {
        targetX = allCanvases.position.x;
    }

    public void OnClick()
    {
        GameObject startCanvas = cosMenuCon.canvases[cosMenuCon.currentCanvas];
        GameObject targetCanvas = startCanvas; //to default
        if (decrement)
        {
            ToggleRectMasks_Decrement();

            if (allCanvases.localPosition.x % cosMenuCon.canvasWidth == 0)
            {
                if (cosMenuCon.currentCanvasIndicative - 1 == -1)
                {
                    cosMenuCon.currentCanvasIndicative = cosMenuCon.canvases.Length - 1;
                }
                else
                {
                    cosMenuCon.currentCanvasIndicative--;
                }
            }


            if(cosMenuCon.currentCanvas - 1 == -1)
            {
                if(allCanvases.localPosition.x % cosMenuCon.canvasWidth == 0)
                {
                    //changing active canvas to the one currently centred
                    activeCanvas = cosMenuCon.canvases[0].gameObject.GetComponent<Canvas>();

                    //visually reordering the canvases PREP
                    cosMenuCon.currentCanvas = cosMenuCon.canvases.Length - 1;
                    targetCanvas = cosMenuCon.canvases[cosMenuCon.currentCanvas];
                    cosMenuCon.currentCanvas = 0;
                    RectTransform tcRect = targetCanvas.GetComponent<RectTransform>();

                    tcRect.localPosition = new Vector3(((allCanvases.localPosition.x / cosMenuCon.canvasWidth) + 1) * -1 * cosMenuCon.canvasWidth, tcRect.localPosition.y);

                    //re ordering the canvas in the canvases array
                    GameObject tempMax = cosMenuCon.canvases[cosMenuCon.canvases.Length - 1];
                    GameObject tempMin = cosMenuCon.canvases[0];

                    for (int i = cosMenuCon.canvases.Length - 2; i >= 1; i--)
                    {
                        cosMenuCon.canvases[i + 1] = cosMenuCon.canvases[i];
                    }
                    cosMenuCon.canvases[1] = tempMin;
                    cosMenuCon.canvases[0] = tempMax;

                    skinsCon.OpenNewTab(cosMenuCon.canvases[0].name);
   
                    //reseting allCanvases position if it has moved too far.
                    if(allCanvases.localPosition.x == cosMenuCon.canvasWidth * (cosMenuCon.canvases.Length - 1))
                    {
                        allCanvases.transform.localPosition = Vector3.zero;
                        for (int c = 0; c < cosMenuCon.canvases.Length; c++)
                        {
                            cosMenuCon.canvasesRects[c].localPosition += new Vector3((cosMenuCon.canvases.Length - 1) * cosMenuCon.canvasWidth, 0f, 0f);
                        }
                    }
                    //moving the canvases
                    LeanTween.moveLocalX(allCanvases.gameObject, allCanvases.localPosition.x + cosMenuCon.canvasWidth, 0.2f).setEase(slide).setOnComplete(disableCanvas);
                }
            }
            else
            {
                if(allCanvases.localPosition.x % cosMenuCon.canvasWidth == 0)
                {
                    //changing active canvas to the one currently centred
                    activeCanvas = cosMenuCon.canvases[0].gameObject.GetComponent<Canvas>();

                    cosMenuCon.currentCanvas--;

                    //re ordering the canvas in the canvases array
                    GameObject tempMax = cosMenuCon.canvases[cosMenuCon.canvases.Length - 1];

                    for (int i = cosMenuCon.canvases.Length - 1; i > 0; i--)
                    {
                        cosMenuCon.canvases[i] = cosMenuCon.canvases[i - 1];
                    }
                    cosMenuCon.canvases[0] = tempMax;

                    skinsCon.OpenNewTab(cosMenuCon.canvases[0].name);

                    //moving the canvases
                    LeanTween.moveLocalX(allCanvases.gameObject, allCanvases.localPosition.x + cosMenuCon.canvasWidth, 0.2f).setEase(slide).setOnComplete(disableCanvas);
                }
            }
        }
        if (increment)
        {
            ToggleRectMasks_Increment();

            if (allCanvases.localPosition.x % cosMenuCon.canvasWidth == 0)
            {
                if (cosMenuCon.currentCanvasIndicative + 1 == cosMenuCon.canvases.Length)
                {
                    cosMenuCon.currentCanvasIndicative = 0;
                }
                else
                {
                    cosMenuCon.currentCanvasIndicative++;
                }
            }

            if (cosMenuCon.currentCanvas + 1 == cosMenuCon.canvases.Length)
            {
                if(allCanvases.localPosition.x % cosMenuCon.canvasWidth == 0)
                {
                    //unhiding canvas to the direct right
                    cosMenuCon.canvases[1].gameObject.GetComponent<Canvas>().enabled = true;
                    //changing active canvas to the one currently centred
                    activeCanvas = cosMenuCon.canvases[0].gameObject.GetComponent<Canvas>();

                    targetCanvas = cosMenuCon.canvases[1];
                    cosMenuCon.currentCanvas = cosMenuCon.canvases.Length - 1;
                    RectTransform tcRect = targetCanvas.GetComponent<RectTransform>();

                    //re ordering the canvas in the canvases array
                    GameObject tempMin = cosMenuCon.canvases[0];

                    for (int i = 0; i < cosMenuCon.canvases.Length - 1; i++)
                    {
                        cosMenuCon.canvases[i] = cosMenuCon.canvases[i + 1];
                    }
                    cosMenuCon.canvases[cosMenuCon.canvases.Length - 1] = tempMin;

                    tcRect.localPosition = new Vector3(((allCanvases.localPosition.x / cosMenuCon.canvasWidth) - 1) * -1 * cosMenuCon.canvasWidth, tcRect.localPosition.y);

                    skinsCon.OpenNewTab(cosMenuCon.canvases[0].name);

                    //reset if allCanvas too far to the left
                    if (allCanvases.localPosition.x == (cosMenuCon.canvases.Length - 1) * -1 * cosMenuCon.canvasWidth)
                    {
                        
                        allCanvases.transform.localPosition = Vector3.zero;
                        for(int c = 0; c < cosMenuCon.canvases.Length; c++)
                        {
                            cosMenuCon.canvasesRects[c].localPosition -= new Vector3((cosMenuCon.canvases.Length-1)*cosMenuCon.canvasWidth, 0f, 0f);
                        }
                    }
                    //moving the canvases
                    LeanTween.moveLocalX(allCanvases.gameObject, allCanvases.localPosition.x - cosMenuCon.canvasWidth, 0.2f).setEase(slide).setOnComplete(disableCanvas);
                }
            }
            else
            {
                if (allCanvases.localPosition.x % cosMenuCon.canvasWidth == 0)
                {
                    //changing active canvas to the one currently centred
                    activeCanvas = cosMenuCon.canvases[0].gameObject.GetComponent<Canvas>();

                    cosMenuCon.currentCanvas++;

                    //re ordering the canvas in the canvases array
                    GameObject tempMin = cosMenuCon.canvases[0];

                    for (int i = 0; i < cosMenuCon.canvases.Length-1; i++)
                    {
                        cosMenuCon.canvases[i] = cosMenuCon.canvases[i + 1];
                    }
                    cosMenuCon.canvases[cosMenuCon.canvases.Length - 1] = tempMin;

                    skinsCon.OpenNewTab(cosMenuCon.canvases[0].name);

                    //moving the canvases
                    LeanTween.moveLocalX(allCanvases.gameObject, allCanvases.localPosition.x - cosMenuCon.canvasWidth, 0.2f).setEase(slide).setOnComplete(disableCanvas);
                    
                }
            }
        }
        cosMenuCon.ChangePageNum();
    }
    public void disableCanvas()
    {
        //activeCanvas.enabled = false;
        cosMenuCon.ToggleCorrectRectMask();
    }

    private void ToggleRectMasks_Decrement()
    {
        if(cosMenuCon.currentCanvasIndicative - 1 == -1)
        {
            cosMenuCon.rectMasks[cosMenuCon.rectMasks.Length - 1].enabled = true;
        }
        else
        {
            cosMenuCon.rectMasks[cosMenuCon.currentCanvasIndicative - 1].enabled = true;
        }
    }

    private void ToggleRectMasks_Increment()
    {
        if (cosMenuCon.currentCanvasIndicative + 1 == cosMenuCon.canvases.Length)
        {
            cosMenuCon.rectMasks[0].enabled = true;
        }
        else
        {
            cosMenuCon.rectMasks[cosMenuCon.currentCanvasIndicative + 1].enabled = true;
        }
    }
}
