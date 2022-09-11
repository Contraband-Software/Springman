using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CosmeticsMenuController : MonoBehaviour
{
    public Canvas cosmeticsCanvas;
    public Canvas mainMenuCanvas;
    public CosmeticsButton cosmeticsButton;
    public RectTransform cmRect;
    public Canvas transitionCanvas;
    public CosmeticsData cosData;

    public SkinsController skinsCon;

    List<GameObject> transitionObjects = new List<GameObject>();

    [Header("Tweens")]
    public LeanTweenType openEase;
    public LeanTweenType closeEase;

    public Vector2 originalCoords;
    public Vector2 hiddenCoords;

    [Header("Canvases")]
    public GameObject[] canvases = new GameObject[] { };
    public List<RectTransform> canvasesRects = new List<RectTransform>();
    public int currentCanvas = 0;
    public float canvasWidth = 0;
    public int currentCanvasIndicative = 0;

    [Header("RectMasks")]
    public RectMask2D[] rectMasks = new RectMask2D[] { }; 

    [Header("PageNum")]
    public List<GameObject> pageNums = new List<GameObject>();
    public Sprite dotOff;
    public Sprite dotOn;

    [Header("Premium")]
    public PremiumDemoContoller premDemoCon;

    [Header("Special Menu Status")]
    public bool buyCanvasOn = false;
    public bool allBoughtCanvasOn = false;
    public bool brokeCanvasOn = false;

    void Start()
    {
        TurnOffAllRectMasks();
        ChangePageNum();

        canvasWidth = canvases[1].GetComponent<RectTransform>().localPosition.x;

        cmRect = gameObject.GetComponent<RectTransform>();
        originalCoords = cmRect.transform.position;

        cmRect.transform.position = new Vector2(cmRect.transform.position.x, cmRect.transform.position.y - (cmRect.transform.position.y * 2f));
        hiddenCoords = cmRect.transform.position;

        unAltBasePos = demo_top.gameObject.GetComponent<RectTransform>().anchoredPosition;
        skinsCon.LoadAll();

        foreach(GameObject canvas in canvases)
        {
            canvasesRects.Add(canvas.GetComponent<RectTransform>());
        }
    }

    public void ChangePageNum()
    {
        foreach(GameObject dot in pageNums)
        {
            dot.GetComponent<Image>().sprite = dotOff;
        }
        pageNums[currentCanvasIndicative].GetComponent<Image>().sprite = dotOn;
    }

    public void OpenMenu()
    {
        LeanTween.value(cosmeticsCanvas.gameObject, MenuCallback, cmRect.transform.position.y, originalCoords.y, 0.4f).setIgnoreTimeScale(true).
            setOnComplete(FinishedOpening).setEase(openEase);
        UpdateDemo();
    }

    public void CloseMenu()
    {
        
        DepositIntoTransitionCanvas();

        LeanTween.value(cosmeticsCanvas.gameObject, MenuCallback, cmRect.transform.position.y, hiddenCoords.y, 0.4f).setIgnoreTimeScale(true).setEase(closeEase);
        LeanTween.delayedCall(0.15f, cosmeticsButton.SlideMainMenuDown);
    }

    void MenuCallback(float y)
    {
        cmRect.transform.position = new Vector2(cmRect.transform.position.x, y);
    }

    void FinishedOpening()
    {
        CollectFromTransitionCanvas();
    }

    void CollectFromTransitionCanvas()
    {
        for(int child = 0; child < transitionCanvas.transform.childCount; child++)
        {
            transitionObjects.Add(transitionCanvas.transform.GetChild(child).gameObject);
            transitionCanvas.transform.GetChild(child).transform.SetParent(gameObject.transform);
        }
    }

    void DepositIntoTransitionCanvas()
    {
        foreach(GameObject gameObj in transitionObjects)
        {
            gameObj.transform.SetParent(transitionCanvas.transform);
        }
    }

    [Header("Springman Demo Parts")]
    public GameObject skin_demo;
    public Image demo_spring;
    public Image demo_top;
    public Image demo_top_skin;
    public Image demo_bottom;
    public Image demo_eyes;


    public Vector3 unAltBasePos;
    public Vector3 altBasePos = Vector3.zero;

    [Header("Demo Part Rects")]
    public RectTransform skinRect;
    public RectTransform eyesRect;

    public void UpdateDemo()
    {
        if (!cosData.currentSkinPremium)
        {
            skin_demo.SetActive(true);

            premDemoCon.HidePremiumSkin();

            SkinSpecsSolid sSpecs = cosData.allSkinSpecs[cosData.allSkinsCodes.IndexOf(cosData.currentSkin)];

            BaseTop(sSpecs);
        }
        else
        {
            skin_demo.SetActive(false);
            
            premDemoCon.ShowActivePremiumSkin();
        }
        
    }

    public void BaseTop(SkinSpecsSolid sSpecs)
    {
        if (!sSpecs.alt_base)
        {
            demo_top.sprite = sSpecs.base_Top;
            demo_eyes.sprite = sSpecs.eyes;
            demo_top.gameObject.GetComponent<RectTransform>().anchoredPosition = unAltBasePos;
            demo_top_skin.gameObject.GetComponent<RectTransform>().anchoredPosition = unAltBasePos;
            demo_eyes.gameObject.GetComponent<RectTransform>().anchoredPosition = unAltBasePos;

            demo_bottom.sprite = sSpecs.base_Bottom;
            demo_spring.sprite = sSpecs.demo_spring_sprite;

            //set colours
            SetColours(sSpecs);

            //set skin
            demo_top_skin.sprite = sSpecs.skin_Top;
            if(sSpecs.skin_Bottom != null)
            {
                demo_bottom.sprite = sSpecs.skin_Bottom;
            }

            //hide missing
            HideMissing(sSpecs);
        }
        else
        {
            demo_top.sprite = sSpecs.alt_Base_Sprite;
            demo_eyes.sprite = sSpecs.eyes;
            demo_top.gameObject.GetComponent<RectTransform>().anchoredPosition = altBasePos;
            demo_top_skin.gameObject.GetComponent<RectTransform>().anchoredPosition = altBasePos;
            demo_eyes.gameObject.GetComponent<RectTransform>().anchoredPosition = altBasePos;

            //set colours
            SetColours(sSpecs);

            //set skin
            demo_top_skin.sprite = sSpecs.skin_Top;

            //hide missing

            var temp = demo_spring.color;
            temp.a = 0f;
            demo_spring.color = temp;
            temp = demo_bottom.color;
            temp.a = 0f;
            demo_bottom.color = temp;

            HideMissing(sSpecs);

            ResizeForFisherman(sSpecs);
        }
    }

    public void SetColours(SkinSpecsSolid sSpecs)
    {
        if (sSpecs.colour_changeable_top)
        {
            demo_top.color = cosData.topColor;
        }
        else
        {
            demo_top.color = Color.white;
        }

        if (sSpecs.colour_changeable_bottom)
        {
            demo_bottom.color = cosData.bottomColor;
        }
        else
        {
            demo_bottom.color = Color.white;
        }

        if (sSpecs.colour_changeable_eyes)
        {
            demo_eyes.color = cosData.topColor;
        }
        else
        {
            demo_eyes.color = Color.white;
        }

        if (sSpecs.colour_top_equal_to_bottom)
        {
            demo_bottom.color = cosData.topColor;
        }
        else
        {
            demo_bottom.color = demo_bottom.color;
        }

        demo_spring.color = cosData.springColor;
        demo_top_skin.color = Color.white;
    }
    public void HideMissing(SkinSpecsSolid sSpecs)
    {
        Color temp;
        if (sSpecs.demo_spring_sprite == null)
        {
            temp = demo_spring.color;
            temp.a = 0f;
            demo_spring.color = temp;
        }
        if (sSpecs.skin_Top == null)
        {
            temp = demo_top_skin.color;
            temp.a = 0f;
            demo_top_skin.color = temp;
        }
        if (demo_top.sprite == null)
        {
            temp = demo_top.color;
            temp.a = 0f;
            demo_top.color = temp;
        }
        if (demo_bottom.sprite == null)
        {
            temp = demo_bottom.color;
            temp.a = 0f;
            demo_bottom.color = temp;
        }
        if (sSpecs.eyes == null)
        {
            temp = demo_eyes.color;
            temp.a = 0f;
            demo_eyes.color = temp;
        }
    }

    public void ResizeForFisherman(SkinSpecsSolid sSpecs)
    {
        if(sSpecs.skin_name == "fisherman")
        {
            skinRect.sizeDelta = new Vector2(skinRect.sizeDelta.x * 1.25f, skinRect.sizeDelta.y);
        }
        else
        {
            skinRect.sizeDelta = new Vector2(eyesRect.sizeDelta.x, eyesRect.sizeDelta.y);
        }
    }



    public void TurnOffBuyCanvas()
    {
        buyCanvasOn = false;
    }
    public void TurnOffAllBoughtCanvas()
    {
        allBoughtCanvasOn = false;
    }
    public void TurnOffBrokeCanvas()
    {
        brokeCanvasOn = false;
    }


    public void ToggleCorrectRectMask()
    {
        //TurnOffAllRectMasks();
        //rectMasks[currentCanvasIndicative].enabled = true;
    }
    public void TurnOffAllRectMasks()
    {
        /*
        for (int i = 0; i < rectMasks.Length; i++)
        {
            if (rectMasks[i].enabled)
            {
                rectMasks[i].enabled = false;
            }
            
        }
        */
    }
}
