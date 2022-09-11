using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CosSaveData
{
    public string topColor;
    public string bottomColor;
    public string springColor;

    public V3Ser topObject;
    public V3Ser bottomObject;
    public V3Ser springObject;

    public int cosType;

    public List<string> unlockedColours;
    public List<string> unlockedSkins;
    public List<string> unlockedPremiums;

    public string currentSkin;
    public bool currentSkinPremium;

    public List<string> glowColours;

    public List<bool> hasSpecialColour;

    public List<bool> specColModes;

    public CosSaveData(string tcol, string bcol, string scol, Vector3 tobj, Vector3 bobj, Vector3 sobj, int cosType, List<string> unlockedCols, List<string> unlockedSkins, string currentSkin,
        List<string> unlockedPremiums, bool currentSkinPremium, List<string> glowColours, List<bool> hasSpecialColour, List<bool> specColModes)
    {
        topColor = tcol;
        bottomColor = bcol;
        springColor = scol;

        topObject = new V3Ser(tobj);
        bottomObject = new V3Ser(bobj);
        springObject = new V3Ser(sobj);

        this.cosType = cosType;

        unlockedColours = unlockedCols;
        this.unlockedSkins = unlockedSkins;
        this.unlockedPremiums = unlockedPremiums;

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
