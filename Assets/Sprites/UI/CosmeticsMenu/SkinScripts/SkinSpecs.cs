using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinSpecs : MonoBehaviour
{
    public string ID;
    public string skin_name;
    public Sprite demoSkin;

    [Header("Base")]
    public Sprite base_Top;
    public Sprite base_Bottom; 
    public Sprite eyes;
    public Sprite eyes_death;
    public bool alt_base;
    public Sprite alt_Base_Sprite;

    [Header("---")]
    public bool colour_changeable_top;  //alt base only uses top color
    public bool colour_changeable_bottom;
    public bool colour_changeable_eyes;  //eye colour = top color
    public bool colour_top_equal_to_bottom;

    [Header("Skin")]
    public Sprite skin_Top;
    public Sprite skin_Bottom; //replaces base_Bottom -> base_Bottom = skin_Bottom,   colour_changeable_bottom = false
    public bool skin_Top_Flippable;

    [Header("Spring")]
    public Sprite spring_sprite;
    public Sprite demo_spring_sprite;
    public string bounce_anim;

    [Header("Sound")]
    public string alt_BounceSound = null;
}
