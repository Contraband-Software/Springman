using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
using UnityEditor;
using System;
using SimpleDiskUtils;

public class CosmeticsData : MonoBehaviour
{
    /*[Header("TEST")]
    public Text testText;

    [Header("menu data")]

    public SkinSelector_Premium skinSelectorPremium;

    [Header("Colour Data")]
    public Color topColor;
    public Color bottomColor;
    public Color springColor;

    public Vector3 topObject;
    public Vector3 bottomObject;
    public Vector3 springObject;

    public List<string> unlockedColours = new List<string>();
    public List<string> allColours = new List<string>();

    [Header("Skin Data")]

    public string currentSkin;

    public SkinSpecsSolid cSpecs = new SkinSpecsSolid();

    public List<string> unlockedSkins = new List<string>();
    public List<string> allSkins = new List<string>();

    public List<string> allSkinsCodes = new List<string>();
    public List<SkinSpecsSolid> allSkinSpecs = new List<SkinSpecsSolid>();

    public enum PlayerCosmeticType { None, Color };
    public PlayerCosmeticType playerCosmeticType = PlayerCosmeticType.None;


    private static GameObject objectInstance;

    [Header("Premium Skins")]
    public bool currentSkinPremium = false;
    public string activePremiumSkinName;
    public List<string> unlockedPremiums = new List<string>();
    public List<string> allPremiums = new List<string>();
    public List<string> allPremiumCodes = new List<string>();

    public List<string> glowColours = new List<string>();
    public List<bool> hasSpecialColour = new List<bool>();
    public List<bool> specialColourModes = new List<bool>();*/

    /*public void Awake()
    {
        CreateFirstCosDataFile();
    }

    public void Start()
    {
        LoadCosData();
        menuData.errorOpened = false;
    }

    public void CreateFirstCosDataFile()
    {
        float availableSpace = SimpleDiskUtils.DiskUtils.CheckAvailableSpace();
        if (availableSpace > 10)
        {
            string path = Path.Combine(Application.persistentDataPath, "cosmeticsData.cos");

            if (!File.Exists(path))
            {
                Debug.Log("CREATING FIRST COSMETICS FILE");

                unlockedColours.Add("FFFFFF");
                unlockedColours.Add("373737");

                //////////////
                //unlockedPremiums.Add("lpqok951139");
                unlockedPremiums = new List<string>{ "lpqok951139", "bonvmm916571", "jkhqys871421", "xxclpu871531", "kljqye098901", "opiuqa9815211", "loiqyv904091", "gqulpo090861"
                , "oilpqu876019", "vbtqeq651064"};
                /////////////

                unlockedSkins.Add("109651fc");

                currentSkin = "109651fc";

                skinSelectorPremium.CollectGlowColours();

                playerCosmeticType = PlayerCosmeticType.Color;
                topColor = Color.white;
                bottomColor = Color.white;
                springColor = StringToColor("373737");

                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Create);

                CosSaveData data = new CosSaveData(ColorToString(topColor), ColorToString(bottomColor), ColorToString(springColor),
                    topObject, bottomObject, springObject, (int)playerCosmeticType, unlockedColours, unlockedSkins, currentSkin, unlockedPremiums, currentSkinPremium, glowColours, hasSpecialColour, specialColourModes);

                File.SetAttributes(path, FileAttributes.ReadOnly);

                formatter.Serialize(stream, data);
                stream.Close();
            }

        }
        else
        {
            DisplayErrorScreen();
        }
    }
    public void LoadCosData()
    {
        string path = Path.Combine(Application.persistentDataPath, "cosmeticsData.cos");
        if (File.Exists(path))
        {
            File.SetAttributes(path, FileAttributes.Normal);

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            CosSaveData data = formatter.Deserialize(stream) as CosSaveData;
            stream.Close();


            topColor = StringToColor(data.topColor);
            bottomColor = StringToColor(data.bottomColor);
            springColor = StringToColor(data.springColor);

            topObject = data.topObject.V3;
            bottomObject = data.bottomObject.V3;
            springObject = data.springObject.V3;


            playerCosmeticType = (PlayerCosmeticType)data.cosType;

            unlockedColours = data.unlockedColours;

            unlockedSkins = data.unlockedSkins;

            currentSkin = data.currentSkin;

            unlockedPremiums = data.unlockedPremiums;

            currentSkinPremium = data.currentSkinPremium;

            glowColours = data.glowColours;

            hasSpecialColour = data.hasSpecialColour;

            specialColourModes = data.specColModes;

            /////
            string fullText = "";
            try
            {
                //Debug.Log(unlockedPremiums.Count);
            }
            catch(NullReferenceException exc)
            {
                Debug.Log("REINSTALL ERROR, PROMPTING REBUILDING OF INITIAL FILES: " + exc.Message);
                DeleteExistingFilesAndRebuild();
            }

            testText.text = fullText;

            if (currentSkinPremium)
            {
                activePremiumSkinName = allPremiums[allPremiumCodes.IndexOf(currentSkin)];
            }
        }
    }
    public void SaveCosData()
    {
        float availableSpace = SimpleDiskUtils.DiskUtils.CheckAvailableSpace();
        if (availableSpace > 10)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            string path = Path.Combine(Application.persistentDataPath, "cosmeticsData.cos");
            File.SetAttributes(path, FileAttributes.Normal);

            FileStream stream = new FileStream(path, FileMode.Create);

            //print("SAVING COSMETICS DATA");
            //control changes to skins
            skinSelectorPremium.CollectSpecialColourSettings();

            CosSaveData data = new CosSaveData(ColorToString(topColor), ColorToString(bottomColor), ColorToString(springColor),
                topObject, bottomObject, springObject, (int)playerCosmeticType, unlockedColours, unlockedSkins, currentSkin, unlockedPremiums, currentSkinPremium, glowColours, hasSpecialColour, specialColourModes);

            File.SetAttributes(path, FileAttributes.ReadOnly);

            formatter.Serialize(stream, data);
            stream.Close();
        }
        else
        {
            DisplayErrorScreen();
        }
    }

    private void DeleteExistingFilesAndRebuild()
    {
        Debug.Log("CLEANING DIRECTORY");

        File.Delete(Path.Combine(Application.persistentDataPath, "cosmeticsData.cos"));
        File.Delete(Path.Combine(Application.persistentDataPath, "gamedatafile.gd"));

        Debug.Log("REBUILDING DIRECTORY");
        GameObject menuCon = GameObject.Find("MenuController");
        if(menuCon != null)
        {
            MenuData md = menuCon.GetComponent<MenuData>();
            md.CreateFirstDataFile();
            CreateFirstCosDataFile();
        }
    }

    public void HardPassSkinID(string ID)
    {
        currentSkin = ID;
    }

    public int HexToDec(string hex)
    {
        int dec = System.Convert.ToInt32(hex, 16);
        return dec;
    }

    public string DecToHex(int value)
    {
        return value.ToString("X2");
    }

    public string FloatToNormalizedToHex(float value)
    {
        return DecToHex(Mathf.RoundToInt(value * 255f));
    }

    public float HexToFloatNormalized(string hex)
    {
        return HexToDec(hex) / 255f;
    }

    public Color StringToColor(string hexString)
    {
        float red = HexToFloatNormalized(hexString.Substring(0, 2));
        float green = HexToFloatNormalized(hexString.Substring(2, 2));
        float blue = HexToFloatNormalized(hexString.Substring(4, 2));
        float alpha = 1f;
        if(hexString.Length >= 8)
        {
            alpha = HexToFloatNormalized(hexString.Substring(6, 2));
        }

        return new Color(red, green, blue, alpha);
    }

    public string ColorToString(Color color, bool useAlpha = false)
    {
        string red = FloatToNormalizedToHex(color.r);
        string green = FloatToNormalizedToHex(color.g);
        string blue = FloatToNormalizedToHex(color.b);
        if (!useAlpha)
        {
            return red + green + blue;
        }
        else
        {
            string alpha = FloatToNormalizedToHex(color.a);
            return red + green + blue + alpha;
        }
    }

    public void DisplayErrorScreen()
    {
        menuData.errorOpened = true;
    }*/
}

