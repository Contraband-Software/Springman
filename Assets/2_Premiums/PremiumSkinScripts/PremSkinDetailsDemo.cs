using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Architecture.Managers;
using Backend;

public class PremSkinDetailsDemo : MonoBehaviour
{
    public int skinIndex;
    [Header("Important References")]

    [Header("Base")]
    public List<Image> TopPieces = new List<Image>();
    public List<Image> BottomPieces = new List<Image>();
    public List<Image> SpringPieces = new List<Image>();

    [Header("Glow")]
    public Color targetColor;
    public bool hasSpecialColourMode = false;
    public bool colourShift = false;
    public List<Color> colorChoices = new List<Color>();
    public bool HardColourChange = false;
    public List<Image> ObjectsToColourChange = new List<Image>();

    public List<Sprite> Element1_Variants = new List<Sprite>();
    public List<Sprite> Element2_Variants = new List<Sprite>();
    public List<Sprite> Element3_Variants = new List<Sprite>();
    public List<Sprite> Element4_Variants = new List<Sprite>();
    public List<Sprite> Element5_Variants = new List<Sprite>();
    public List<Sprite> Element6_Variants = new List<Sprite>();
    public List<Sprite> Element7_Variants = new List<Sprite>();
    private List<List<Sprite>> element_variants = new List<List<Sprite>>();

    [Header("FOR IN GAME OBJECTS")]
    [Header("Base")]
    public bool forGame = false;
    public List<SpriteRenderer> TopPieces_ = new List<SpriteRenderer>();
    public List<SpriteRenderer> BottomPieces_ = new List<SpriteRenderer>();
    public List<SpriteRenderer> SpringPieces_ = new List<SpriteRenderer>();
    public List<SpriteRenderer> ObjectsToColourChange_ = new List<SpriteRenderer>();

    public void Start()
    {
        element_variants.Add(Element1_Variants);
        element_variants.Add(Element2_Variants);
        element_variants.Add(Element3_Variants);
        element_variants.Add(Element4_Variants);
        element_variants.Add(Element5_Variants);
        element_variants.Add(Element6_Variants);
        element_variants.Add(Element7_Variants);
    }

    public void UpdateSkin()
    {
        skinIndex = UserGameData.Instance.allPremiums.IndexOf(UserGameData.Instance.activePremiumSkinName);
        targetColor = Utilities.StringToColor(UserGameData.Instance.glowColours[skinIndex]);

        hasSpecialColourMode = UserGameData.Instance.hasSpecialColour[skinIndex];
        if (hasSpecialColourMode)
        {
            colourShift = UserGameData.Instance.specialColourModes[skinIndex];
        }

        if (!forGame)
        {
            ChangeBaseColour();
            ChangeGlowColour();
        }
        else
        {
            ChangeBaseColour_Game();
            ChangeGlowColour_Game();
        }
        
    }

    public void UpdateGlow_Soft()
    {
        foreach (SpriteRenderer element in ObjectsToColourChange_)
        {
            element.color = targetColor;
        }
    }

    public void AllGlowsTransparent_Soft()
    {
        foreach (SpriteRenderer element in ObjectsToColourChange_)
        {
            Color alphaCol = element.color;
            alphaCol.a = 0;
            element.color = alphaCol;
        }
    }

    public void ChangeBaseColour()
    {
        foreach(Image element in TopPieces)
        {
            element.color = UserGameData.Instance.topColor;
        }
        foreach (Image element in BottomPieces)
        {
            element.color = UserGameData.Instance.bottomColor;
        }
        foreach (Image element in SpringPieces)
        {
            element.color = UserGameData.Instance.springColor;
        }
    }

    public void ChangeGlowColour()
    {
        element_variants.Clear();

        element_variants.Add(Element1_Variants);
        element_variants.Add(Element2_Variants);
        element_variants.Add(Element3_Variants);
        element_variants.Add(Element4_Variants);
        element_variants.Add(Element5_Variants);
        element_variants.Add(Element6_Variants);
        element_variants.Add(Element7_Variants);


        string targetColString = Utilities.ColorToString(targetColor);
        List<string> colourChoicesString = new List<string>();

        foreach(Color col in colorChoices)
        {
            colourChoicesString.Add(Utilities.ColorToString(col));
        }

        int colourIndex = colourChoicesString.IndexOf(targetColString);

        if (HardColourChange)
        {
            for(int i = 0; i < ObjectsToColourChange.Count; i++)
            {
                ObjectsToColourChange[i].sprite = element_variants[i][colourIndex];
            }
        }
        else
        {
            foreach (Image element in ObjectsToColourChange)
            {
                element.color = targetColor;
            }
        }
    }

    public void ChangeBaseColour_Game()
    {
        foreach (SpriteRenderer element in TopPieces_)
        {
            element.color = UserGameData.Instance.topColor;
        }
        foreach (SpriteRenderer element in BottomPieces_)
        {
            element.color = UserGameData.Instance.bottomColor;
        }
        foreach (SpriteRenderer element in SpringPieces_)
        {
            element.color = UserGameData.Instance.springColor;
        }
    }
    public void ChangeGlowColour_Game()
    {
        element_variants.Clear();

        element_variants.Add(Element1_Variants);
        element_variants.Add(Element2_Variants);
        element_variants.Add(Element3_Variants);
        element_variants.Add(Element4_Variants);
        element_variants.Add(Element5_Variants);
        element_variants.Add(Element6_Variants);
        element_variants.Add(Element7_Variants);


        string targetColString = Utilities.ColorToString(targetColor);
        List<string> colourChoicesString = new List<string>();

        foreach (Color col in colorChoices)
        {
            colourChoicesString.Add(Utilities.ColorToString(col));
        }

        int colourIndex = colourChoicesString.IndexOf(targetColString);

        if (HardColourChange)
        {
            for (int i = 0; i < ObjectsToColourChange_.Count; i++)
            {
                ObjectsToColourChange_[i].sprite = element_variants[i][colourIndex];
            }
        }
        else
        {
            foreach (SpriteRenderer element in ObjectsToColourChange_)
            {
                element.color = targetColor;
            }
        }
    }
}
