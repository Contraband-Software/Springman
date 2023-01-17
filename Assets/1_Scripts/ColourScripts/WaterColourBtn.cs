using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using Architecture.Managers;

public class WaterColourBtn : MonoBehaviour
{
    [HideInInspector] public Color storedColor;
    [HideInInspector] public RectTransform rect;

    private void Start()
    {
        storedColor = gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().color;
        rect = gameObject.GetComponent<RectTransform>();
    }
}
