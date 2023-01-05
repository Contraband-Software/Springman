using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using SimpleDiskUtils;
using PlatformIntegrations;

public class MenuData : MonoBehaviour
{
    public int allTimeHighscore;
    public bool musicOn;
    public bool soundsOn;
    public string currentLanguage; //Localisation
    public int langIndex;
    public int gold;
    public int silver;
    public int ads;

    public GameObject menuAudio;

    //[Header("Legal")]

    [Header("Language Fonts")]
    public TMP_FontAsset appropriateFont;
    public TMP_FontAsset Latin_Cyrillic;
    public TMP_FontAsset Latin;
    public TMP_FontAsset Vietnamese;
    public TMP_FontAsset Chinese;
    public TMP_FontAsset Hindi;
    public TMP_FontAsset Arabic;

    public CanvasGroup curtainCG;
    [Header("Tutorial Status")]
    public bool tutorialComplete = false;

    [Header("Error Stuff")]
    public bool errorOpened = false;

    string path;
    bool EULA_Accepted = false;
    PlatformIntegrations.SocialManager sm;

    public void SetEulaAccepted()
    {
        EULA_Accepted = true;
    }

    void Awake()
    {
        Application.targetFrameRate = 60;

        path = Path.Combine(Application.persistentDataPath, "gamedatafile.gd");

        LeanTween.cancelAll();

        InitialiseAppropriateFonts();

        //social manager will have loaded data cached
        //CreateFirstDataFile();
        //LoadGameData();
        Debug.Log("WE LOADED IN TO THE MAIN MENU POG POG POG");
        Debug.Log("MENU DATA - ATTEMPTING TO GET SAVED DATA FROM CACHE");
        Debug.Log(IntegrationsManager.Instance.socialManager.GetCachedSaveGame());
        Debug.Log((SaveData)IntegrationsManager.Instance.socialManager.GetCachedSaveGame());
    }

    private void Start()
    {
        errorOpened = false;

        curtainCG.alpha = 1f;
        LeanTween.alphaCanvas(curtainCG, 0f, 0.4f).setIgnoreTimeScale(true);

        sm = IntegrationsManager.Instance.socialManager;
        sm.SaveDataWriteCallback.AddListener((bool status) => {
            if (!status)
            {
                Debug.Log("MenuData: Save data cloud write unsuccessful");
            } else
            {
                Debug.Log("MenuData: Save data written to cloud successfully");
            }
        });

#if !UNITY_EDITOR
        SaveData loadedSaveData = (SaveData)IntegrationsManager.instance.socialManager.GetCachedSaveGame();
        Debug.Log(IntegrationsManager.instance.socialManager.GetCachedSaveGame());
        Debug.Log(loadedSaveData);


        ReLocalizeTexts();
        //load the vloud
#else
        //create default save data to create dummy file
        //create first data file()
        CreateFirstDataFile();
        LoadGameData();
        ReLocalizeTexts();
#endif

    }

    public void CreateFirstDataFile()
    {
        float availableSpace = SimpleDiskUtils.DiskUtils.CheckAvailableSpace();
        if (availableSpace > 10)
        {
            if (!File.Exists(path))
            {
                EULA_Accepted = false;
                musicOn = true;
                soundsOn = true;
                gold = 60;
                silver = 300;
                tutorialComplete = false;
                ads = 0;

                //Localisation
                List<string> langsTemp = new List<string>{ "english", "french", "spanish", "russian", "german", "portugese", "malay",
                "polish", "italian", "chinese", "turkish", "vietnamese", "ukrainian", "hindi", "indonesian", "arabic"};

                //Localisation
                switch (Application.systemLanguage)
                {
                    case SystemLanguage.Arabic:
                        currentLanguage = "arabic";
                        break;
                    case SystemLanguage.Chinese:
                        currentLanguage = "chinese";
                        break;
                    case SystemLanguage.French:
                        currentLanguage = "french";
                        break;
                    case SystemLanguage.German:
                        currentLanguage = "german";
                        break;
                    case SystemLanguage.Indonesian:
                        currentLanguage = "indonesian";
                        break;
                    case SystemLanguage.Italian:
                        currentLanguage = "italian";
                        break;
                    case SystemLanguage.Polish:
                        currentLanguage = "polish";
                        break;
                    case SystemLanguage.Portuguese:
                        currentLanguage = "portugese";
                        break;
                    case SystemLanguage.Russian:
                        currentLanguage = "russian";
                        break;
                    case SystemLanguage.Spanish:
                        currentLanguage = "spanish";
                        break;
                    case SystemLanguage.Turkish:
                        currentLanguage = "turkish";
                        break;
                    case SystemLanguage.Ukrainian:
                        currentLanguage = "ukrainian";
                        break;
                    case SystemLanguage.Vietnamese:
                        currentLanguage = "vietnamese";
                        break;

                    //DEFAULT TO ENGLISH
                    default:
                        currentLanguage = "english";
                        break;
                }
                //Localisation
                langIndex = langsTemp.IndexOf(currentLanguage);


                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.OpenOrCreate);

                SaveData data = new SaveData(this.allTimeHighscore, this.musicOn, this.soundsOn, this.currentLanguage, this.langIndex, this.gold, this.silver,
                    tutorialComplete, this.ads);

#if !UNITY_EDITOR
                File.SetAttributes(path, FileAttributes.ReadOnly);
#endif

                formatter.Serialize(stream, data);
                stream.Close();

                Debug.Log("CREATED FIRST GAME DATA FILE");

                

                eula.Show();
                //if (sm.isAvailable()) { sm.ShowSaveGameSelectUI(); }
            } else
            {
                //if (sm.isAvailable()) { sm.LoadSaveGame(); }
            }
        }
        else
        {
            DisplayErrorScreen();
        }
    }

    public void LoadGameData()
    {
        File.SetAttributes(path, FileAttributes.Normal);
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Open);

        SaveData data = formatter.Deserialize(stream) as SaveData;
        stream.Close();

        allTimeHighscore = data.highscore;
        musicOn = data.musicOn;
        soundsOn = data.soundsOn;
        currentLanguage = data.language;

        langIndex = data.langIndex;
        gold = data.gold;
        silver = data.silver;
        tutorialComplete = data.tutorialComplete;
        ads = data.ads;

        EULA_Accepted = true;

        //Debug.Log("LOADED GAMEDATA FILE");
        
        
    }

    public void SaveGameData()
    {
        SaveData data = new SaveData(
            this.allTimeHighscore, 
            this.musicOn, 
            this.soundsOn, 
            this.currentLanguage, 
            this.langIndex, 
            this.gold, this.silver,
            tutorialComplete, 
            ads
        );

#if UNITY_EDITOR
        //SAVE TO LOCAL STORAGE ALWAYS ANYWAY
        SaveGameData_LocalFallback(data);
#else
        if (sm.IsAvailable() && sm.SaveGameLoaded())
        {
            sm.SaveGame(data);
        }
        else
        {
            Debug.Log("integration not availible");
        }
#endif

    }

    private void SaveGameData_LocalFallback(SaveData data)
    {
        float availableSpace = SimpleDiskUtils.DiskUtils.CheckAvailableSpace();
        if (availableSpace > 10)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            File.SetAttributes(path, FileAttributes.Normal);

            FileStream stream = new FileStream(path, FileMode.Create);

            File.SetAttributes(path, FileAttributes.ReadOnly);

            formatter.Serialize(stream, data);
            stream.Close();

            Debug.Log("MenuData: Save data written to local fallback");
        }
        else
        {
            DisplayErrorScreen();
        }
    }

    public void DisplayErrorScreen()
    {
        errorOpened = true;
    }

    //Localisation
    public void ReLocalizeTexts()
    {
        print("TRANSLATING TEXTS");
        Architecture.Localisation.LocalizationSystem.language = (Architecture.Localisation.LocalizationSystem.Language)langIndex;
        print("TO: " + Architecture.Localisation.LocalizationSystem.language.ToString());

        FindAppropriateFont();

        //can make this more efficient: after finding all these objects once, cache them.
        TextLocaliserUI[] textItems = FindObjectsOfType(typeof(TextLocaliserUI)) as TextLocaliserUI[];
        foreach(TextLocaliserUI text in textItems)
        {
            if (text.key == "localLang")
            {
                text.menuData = this;
                text.language = currentLanguage;
                text.ApplyFontForLangMenu();
            }
            else
            {
                text.menuData = this;
                text.language = currentLanguage;
                text.Localize();
            }
        }
    }

    //Localisation
    Dictionary<string, TMP_FontAsset> appropriateFonts = new Dictionary<string, TMP_FontAsset>();

    //Localisation
    void InitialiseAppropriateFonts()
    {
        appropriateFonts.Add("english", Latin);
        appropriateFonts.Add("french", Latin_Cyrillic);
        appropriateFonts.Add("spanish", Latin_Cyrillic);
        appropriateFonts.Add("russian", Latin_Cyrillic);
        appropriateFonts.Add("german", Latin_Cyrillic);
        appropriateFonts.Add("portugese", Latin_Cyrillic);
        appropriateFonts.Add("malay", Latin_Cyrillic);
        appropriateFonts.Add("polish", Latin_Cyrillic);
        appropriateFonts.Add("italian", Latin_Cyrillic);
        appropriateFonts.Add("chinese", Chinese);
        appropriateFonts.Add("turkish", Latin_Cyrillic);
        appropriateFonts.Add("vietnamese", Vietnamese);
        appropriateFonts.Add("ukrainian", Latin_Cyrillic);
        appropriateFonts.Add("hindi", Hindi);
        appropriateFonts.Add("indonesian", Latin_Cyrillic);
        appropriateFonts.Add("arabic", Arabic);
    }

    //Localisation
    void FindAppropriateFont()
    {
        TMP_FontAsset value;
        appropriateFonts.TryGetValue(currentLanguage, out value);

        appropriateFont = value;
    }

    //Localisation
    public TMP_FontAsset GiveAppropriateFont(string language)
    {
        TMP_FontAsset value;
        appropriateFonts.TryGetValue(language, out value);

        return value;
    }

    private void OnApplicationQuit()
    {
        if (EULA_Accepted)
        {
            if (!errorOpened)
            {
                Debug.Log("Saving on exit");
                SaveGameData();
            }
        } else
        {
            //shut game, delete all gamedata, hard factory reset
            DirectoryInfo dataDir = new DirectoryInfo(Application.persistentDataPath);
            dataDir.Delete(true);
        }
    }
}
