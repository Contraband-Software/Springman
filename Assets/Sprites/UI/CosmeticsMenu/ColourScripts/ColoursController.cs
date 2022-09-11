using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.UI;

public class ColoursController : MonoBehaviour
{
    public FrameController frameController;
    public ColourSelector colourSelector;
    public CosmeticsController cosmeticsController;
    public CosmeticsMenuController cosMenuCon;
    public CosmeticsData cosmeticsData;

    public Image tick;
    public GameObject lockIcon;
    [Header("Springman Parts")]
    public Image demoTop;
    public Image demoBottom;
    public Image demoSpring;

    [Header("Color Values")]
    public Color topColor = Color.white;
    public Color bottomColor = Color.white;
    public Color springColor = Color.white;

    [Header("Corresponding Gameobjects")]
    public Vector3 topObject;
    public Vector3 bottomObject;
    public Vector3 springObject;

    [Header("Values of current frame")]
    //this will switch based off of the current frame. when the frame switches, a different value is loaded into here
    public Color currentColour;
    public Vector3 currentObject;

    public void Start()
    {
        tick.gameObject.SetActive(false);
        lockIcon.gameObject.SetActive(false);
    }

    public void OnColourPageOpen()
    {
        cosmeticsData.LoadCosData();
        colourSelector.CollectColours();
        FetchData();
        if (cosMenuCon.canvases[0].name.ToLower() == "colours")
        {
            frameController.OnColourPageOpen();

            LockedColours();

            PlaceTick();
        }
    }

    public void PlaceTick()
    {
        ChangeToCorrectCurrent();
        //UpdateDemo();
        SendData();

        if (currentObject != Vector3.zero)
        {
            tick.GetComponent<RectTransform>().anchoredPosition = currentObject;
            tick.gameObject.SetActive(true);
        }
        else
        {
            tick.gameObject.SetActive(false);
        }
    }

    //will be made to work with all skin types
    public void ChangeToCorrectCurrent()
    {
        //checks which frame is currently open, and makes sure the current colour and object is corresponding
        //to that frame
        if(cosmeticsData.playerCosmeticType == CosmeticsData.PlayerCosmeticType.Color)
        {
            if (frameController.currentFrame == "TopFrame")
            {
                currentColour = topColor;
                currentObject = topObject;

                colourSelector.selectedColor = topColor;
                colourSelector.selectedObject = topObject;
            }
            if (frameController.currentFrame == "BottomFrame")
            {
                currentColour = bottomColor;
                currentObject = bottomObject;

                colourSelector.selectedColor = bottomColor;
                colourSelector.selectedObject = bottomObject;
            }
            if (frameController.currentFrame == "SpringFrame")
            {
                currentColour = springColor;
                currentObject = springObject;

                colourSelector.selectedColor = springColor;
                colourSelector.selectedObject = springObject;
            }
        }
        else
        {
            currentColour = Color.white;
            currentObject = Vector3.zero;
        }
    }

    public void LockedColours()
    {
        int index;

        foreach(string colour in colourSelector.colours)
        {
            if (cosmeticsData.unlockedColours.Contains(colour))
            {
                index = colourSelector.colours.IndexOf(colour);
                colourSelector.colourButtonScripts[index].gameObject.SetActive(true);
                if (colourSelector.colourButtonScripts[index].gameObject.transform.childCount > 2)
                {
                    Destroy(colourSelector.colourButtonScripts[index].gameObject.transform.GetChild(2).gameObject);
                }
            }
        }
    }

    public void UpdateDemo()
    {
        demoTop.color = topColor;
        demoBottom.color = bottomColor;
        demoSpring.color = springColor;

    }
    public void SendData()
    {
        cosmeticsData.topColor = topColor;
        cosmeticsData.bottomColor = bottomColor;
        cosmeticsData.springColor = springColor;

        cosmeticsData.topObject = topObject;
        cosmeticsData.bottomObject = bottomObject;
        cosmeticsData.springObject = springObject;
    }
    public void FetchData()
    {
        topColor = cosmeticsData.topColor;
        bottomColor = cosmeticsData.bottomColor;
        springColor = cosmeticsData.springColor;

        topObject = cosmeticsData.topObject;
        bottomObject = cosmeticsData.bottomObject;
        springObject = cosmeticsData.springObject;
    }
}
