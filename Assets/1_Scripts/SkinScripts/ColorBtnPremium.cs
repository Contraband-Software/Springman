using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorBtnPremium : MonoBehaviour
{
    [SerializeField] private Image image;

    public Color getButtonColour()
    {
        return image.color;
    }

    public bool isColorShiftButton()
    {
        if(image.sprite.name == "prem_glow_col_bg_SHIFT")
        {
            return true;
        }
        return false;
    }
}
