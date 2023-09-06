using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{

    //GAME DATA
    public int highscore;
    public bool musicOn = true;
    public bool soundsOn = true;
    public string language = "english";
    public int langIndex = 0;
    public int gold;
    public int silver;
    public bool eulaAccepted = false;
    public bool tutorialComplete = false;
    public int ads;
    public int ageLevel;

    //COSMETICS DATA
    public string themeColour;

    public string topColor;
    public string bottomColor;
    public string springColor;

    public V3Ser topObject;
    public V3Ser bottomObject;
    public V3Ser springObject;

    public int cosType;

    public List<string> unlockedColours;
    public List<string> unlockedSkins;

    public string currentSkin;
    public bool currentSkinPremium;

    public Dictionary<string, string> glowColours;

    public Dictionary<string, bool> hasSpecialColour;

    public Dictionary<string, bool> specColModes;

    public SaveData(int allTimeHighscore,
        bool musicOn,
        bool soundsOn,
        string language,
        int langIndex,
        int gold,
        int silver,
        bool eulaAccepted,
        bool tutorialComplete, 
        int ads,
        int ageLevel,
        string themeColour,
        string tcol, 
        string bcol, 
        string scol, 
        Vector3 tobj, 
        Vector3 bobj, 
        Vector3 sobj, 
        int cosType, 
        List<string> unlockedCols, 
        List<string> unlockedSkins, 
        string currentSkin,
        bool currentSkinPremium, 
        Dictionary<string, string> glowColours, 
        Dictionary<string, bool> hasSpecialColour, 
        Dictionary<string, bool> specColModes)
    {
        //GAME
        highscore = allTimeHighscore;
        this.musicOn = musicOn;
        this.soundsOn = soundsOn;
        this.language = language;
        this.langIndex = langIndex;
        this.gold = gold;
        this.silver = silver;
        this.eulaAccepted = eulaAccepted;
        this.tutorialComplete = tutorialComplete;
        this.ads = ads;
        this.ageLevel = ageLevel;

        //COSMETICS
        this.themeColour = themeColour;

        topColor = tcol;
        bottomColor = bcol;
        springColor = scol;

        topObject = new V3Ser(tobj);
        bottomObject = new V3Ser(bobj);
        springObject = new V3Ser(sobj);

        this.cosType = cosType;

        unlockedColours = unlockedCols;
        this.unlockedSkins = unlockedSkins;

        this.currentSkin = currentSkin;
        this.currentSkinPremium = currentSkinPremium;

        this.glowColours = glowColours;

        this.hasSpecialColour = hasSpecialColour;

        this.specColModes = specColModes;
    }
}

[System.Serializable]
public struct V3Ser
{
    public float x;
    public float y;
    public float z;

    public V3Ser(Vector3 vector3)
    {
        this.x = vector3.x;
        this.y = vector3.y;
        this.z = vector3.z;
    }
    public Vector3 V3
    { get { return new Vector3(x, y, z); } }

}
