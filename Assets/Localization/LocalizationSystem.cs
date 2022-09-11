using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationSystem
{
    public enum Language
    {
        English,
        French,
        Spanish,
        Russian,
        German,
        Portugese,
        Malay,
        Polish,
        Italian,
        Chinese,
        Turkish,
        Vietnamese,
        Ukrainian,
        Hindi,
        Indonesian,
        Arabic
    }

    public static Language language = Language.English;

    private static Dictionary<string, string> localisedEN;
    private static Dictionary<string, string> localisedFR;
    private static Dictionary<string, string> localisedES;
    private static Dictionary<string, string> localisedRU;
    private static Dictionary<string, string> localisedGE;
    private static Dictionary<string, string> localisedPO;
    private static Dictionary<string, string> localisedMA;
    private static Dictionary<string, string> localisedPL;
    private static Dictionary<string, string> localisedIT;
    private static Dictionary<string, string> localisedCH;
    private static Dictionary<string, string> localisedTU;
    private static Dictionary<string, string> localisedVI;
    private static Dictionary<string, string> localisedUK;
    private static Dictionary<string, string> localisedHI;
    private static Dictionary<string, string> localisedIN;
    private static Dictionary<string, string> localisedAR;

    public static bool isInit;

    public static CSVLoader csvLoader;

    public static void Init()
    {
        csvLoader = new CSVLoader();
        csvLoader.LoadCSV();

        localisedEN = csvLoader.GetDictionaryValues("en");
        localisedFR = csvLoader.GetDictionaryValues("fr");
        localisedES = csvLoader.GetDictionaryValues("es");
        localisedRU = csvLoader.GetDictionaryValues("ru");
        localisedGE = csvLoader.GetDictionaryValues("ge");
        localisedPO = csvLoader.GetDictionaryValues("po");
        localisedMA = csvLoader.GetDictionaryValues("ma");
        localisedPL = csvLoader.GetDictionaryValues("pl");
        localisedIT = csvLoader.GetDictionaryValues("it");
        localisedCH = csvLoader.GetDictionaryValues("ch");
        localisedTU = csvLoader.GetDictionaryValues("tu");
        localisedVI = csvLoader.GetDictionaryValues("vi");
        localisedUK = csvLoader.GetDictionaryValues("uk");
        localisedHI = csvLoader.GetDictionaryValues("hi");
        localisedIN = csvLoader.GetDictionaryValues("in");
        localisedAR = csvLoader.GetDictionaryValues("ar");

        isInit = true;

    }

    public static string GetLocalisedValue(string key)
    {
        if (!isInit) { Init(); }

        string value = key;

        switch (language)
        {
            case Language.English:
                localisedEN.TryGetValue(key, out value);
                break;
            case Language.French:
                localisedFR.TryGetValue(key, out value);
                break;
            case Language.Spanish:
                localisedES.TryGetValue(key, out value);
                break;
            case Language.Russian:
                localisedRU.TryGetValue(key, out value);
                break;
            case Language.German:
                localisedGE.TryGetValue(key, out value);
                break;
            case Language.Portugese:
                localisedPO.TryGetValue(key, out value);
                break;
            case Language.Malay:
                localisedMA.TryGetValue(key, out value);
                break;
            case Language.Polish:
                localisedPL.TryGetValue(key, out value);
                break;
            case Language.Italian:
                localisedIT.TryGetValue(key, out value);
                break;
            case Language.Chinese:
                localisedCH.TryGetValue(key, out value);
                break;
            case Language.Turkish:
                localisedTU.TryGetValue(key, out value);
                break;
            case Language.Vietnamese:
                localisedVI.TryGetValue(key, out value);
                break;
            case Language.Ukrainian:
                localisedUK.TryGetValue(key, out value);
                break;
            case Language.Hindi:
                localisedHI.TryGetValue(key, out value);
                break;
            case Language.Indonesian:
                localisedIN.TryGetValue(key, out value);
                break;
            case Language.Arabic:
                localisedAR.TryGetValue(key, out value);
                break;
        }

        return value;
    }
}
