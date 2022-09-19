using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinButton : MonoBehaviour
{
    public SkinSpecs skinSpecs;
    public RectTransform rect;
    public void AssignValues()
    {
        skinSpecs = transform.GetChild(0).GetComponent<SkinSpecs>();
        rect = gameObject.GetComponent<RectTransform>();
    }
}
