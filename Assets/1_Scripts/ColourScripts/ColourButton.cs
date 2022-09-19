using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColourButton : MonoBehaviour
{
    public Color storedColour;
    public RectTransform rect;
    public void AssignValues()
    {
        storedColour = transform.GetChild(0).GetComponent<Image>().color;
        rect = gameObject.GetComponent<RectTransform>();
    }
}
