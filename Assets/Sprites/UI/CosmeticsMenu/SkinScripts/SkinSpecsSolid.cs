using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinSpecsSolid
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

    public SkinSpecs ConvertToSolid(SkinSpecs copiedSpecs, SkinSpecsSolid collectedSkin)
    {
        //deep copying the specs
        copiedSpecs.ID = collectedSkin.ID;
        copiedSpecs.skin_name = collectedSkin.skin_name;
        copiedSpecs.demoSkin = collectedSkin.demoSkin;
        copiedSpecs.base_Top = collectedSkin.base_Top;
        copiedSpecs.base_Bottom = collectedSkin.base_Bottom;
        copiedSpecs.eyes = collectedSkin.eyes;
        copiedSpecs.eyes_death = collectedSkin.eyes_death;
        copiedSpecs.alt_base = collectedSkin.alt_base;
        copiedSpecs.alt_Base_Sprite = collectedSkin.alt_Base_Sprite;
        copiedSpecs.colour_changeable_top = collectedSkin.colour_changeable_top;
        copiedSpecs.colour_changeable_bottom = collectedSkin.colour_changeable_bottom;
        copiedSpecs.colour_changeable_eyes = collectedSkin.colour_changeable_eyes;
        copiedSpecs.colour_top_equal_to_bottom = collectedSkin.colour_top_equal_to_bottom;
        copiedSpecs.skin_Top = collectedSkin.skin_Top;
        copiedSpecs.skin_Bottom = collectedSkin.skin_Bottom;
        copiedSpecs.skin_Top_Flippable = collectedSkin.skin_Top_Flippable;
        copiedSpecs.spring_sprite = collectedSkin.spring_sprite;
        copiedSpecs.demo_spring_sprite = collectedSkin.demo_spring_sprite;
        copiedSpecs.bounce_anim = collectedSkin.bounce_anim;
        copiedSpecs.alt_BounceSound = collectedSkin.alt_BounceSound;


        return copiedSpecs;
    }
}
