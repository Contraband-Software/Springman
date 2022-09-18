using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FrameController : MonoBehaviour
{
    public string currentFrame;
    public Canvas frameCanvas;
    public ColourSelector colourSelector;
    public ColoursController coloursController;

    [Header("Frames")]
    public GameObject topFrame;
    public GameObject bottomFrame;
    public GameObject springFrame;

    RectTransform tfButton;
    RectTransform bfButton;
    RectTransform sfButton;

    RectTransform[] buttons;
    public void Start()
    {
        tfButton = topFrame.transform.GetChild(0).GetComponent<RectTransform>();
        bfButton = bottomFrame.transform.GetChild(0).GetComponent<RectTransform>();
        sfButton = springFrame.transform.GetChild(0).GetComponent<RectTransform>();

        buttons = new RectTransform[] { tfButton, bfButton, sfButton };
    }

    public void SetFrameAsTop(GameObject frame)
    {
        RectTransform frameRect = frame.GetComponent<RectTransform>();
        frameRect.SetAsLastSibling();

        foreach(RectTransform frameButton in buttons)
        {
            frameButton.Find("ButtonText").SetAsLastSibling();
        }
        frameRect.GetChild(0).Find("ButtonTextWH").SetAsLastSibling();

        currentFrame = frame.name;
        colourSelector.currentFrame = currentFrame;

        coloursController.PlaceTick();
    }

    public void OnColourPageOpen()
    {
        //topFrame.GetComponent<RectTransform>().SetAsLastSibling();
        //tfButton.Find("ButtonTextWH").SetAsLastSibling();
        if(currentFrame == "")
        {
            topFrame.GetComponent<RectTransform>().SetAsLastSibling();
            tfButton.Find("ButtonTextWH").SetAsLastSibling();
        }

        //this resizes the texts so they are all the same size
        List<float> fontSizes = new List<float>();
        foreach(RectTransform frameButton in buttons)
        {
            fontSizes.Add(frameButton.GetChild(0).GetComponent<TextMeshProUGUI>().fontSize);
        }
        float smallestFont = fontSizes.Min();

        foreach(RectTransform frameButton in buttons)
        {
            frameButton.GetChild(0).GetComponent<TextMeshProUGUI>().fontSizeMax = smallestFont - (smallestFont * 0.05f);
            frameButton.GetChild(1).GetComponent<TextMeshProUGUI>().fontSizeMax = smallestFont - (smallestFont * 0.05f);
        }

        currentFrame = frameCanvas.transform.GetChild(frameCanvas.transform.childCount - 1).name;
        colourSelector.currentFrame = currentFrame;

        //coloursController.PlaceTick();
    }
}
