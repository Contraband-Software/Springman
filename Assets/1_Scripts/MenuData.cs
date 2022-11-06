using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using SimpleDiskUtils;

public class MenuData : MonoBehaviour
{
    public int allTimeHighscore;
    public bool musicOn;
    public bool soundsOn;
    public string currentLanguage;
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

    [Header("First load systems")]
    [SerializeField] EULADialogue eula;
    [SerializeField] PlatformIntegrations.IntegrationsManager integrations;

    string path;
    bool EULA_Accepted = false;

    public void SetEULA_Accepted()
    {
        EULA_Accepted = true;
    }

    void Awake()
    {
        Application.targetFrameRate = 60;

        path = Path.Combine(Application.persistentDataPath, "gamedatafile.gd");

        LeanTween.cancelAll();

        InitialiseAppropriateFonts();
        CreateFirstDataFile();
        LoadGameData();
    }

    private void Start()
    {
        errorOpened = false;

        curtainCG.alpha = 1f;
        LeanTween.alphaCanvas(curtainCG, 0f, 0.4f).setIgnoreTimeScale(true);
    }

    public void CreateFirstDataFile()
    {
        PlatformIntegrations.SocialManager sm = integrations.GetSocialManager();

        float availableSpace = SimpleDiskUtils.DiskUtils.CheckAvailableSpace();
        if (availableSpace > 10)
        {
//#if UNITY_EDITOR
//            if (GameObject.FindGameObjectWithTag("DebugController").GetComponent<GameDebugController>().GetAlwaysFirstRun())
//            {
//                File.Delete(Path.Combine(Application.persistentDataPath, "cosmeticsData.cos"));
//                File.Delete(Path.Combine(Application.persistentDataPath, "gamedatafile.gd"));
//            }
//#endif
            if (!File.Exists(path))
            {
                EULA_Accepted = false;
                musicOn = true;
                soundsOn = true;
                gold = 60;
                silver = 300;
                tutorialComplete = false;
                ads = 0;

                List<string> langsTemp = new List<string>{ "english", "french", "spanish", "russian", "german", "portugese", "malay",
                "polish", "italian", "chinese", "turkish", "vietnamese", "ukrainian", "hindi", "indonesian", "arabic"};

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

                ReLocalizeTexts();

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
        
        ReLocalizeTexts();
    }

    public void SaveGameData()
    {
        float availableSpace = SimpleDiskUtils.DiskUtils.CheckAvailableSpace();
        if (availableSpace > 10)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            File.SetAttributes(path, FileAttributes.Normal);

            FileStream stream = new FileStream(path, FileMode.Create);

            SaveData data = new SaveData(this.allTimeHighscore, this.musicOn, this.soundsOn, this.currentLanguage, this.langIndex, this.gold, this.silver, 
                tutorialComplete, ads);

            File.SetAttributes(path, FileAttributes.ReadOnly);

            formatter.Serialize(stream, data);
            stream.Close();
        }
        else
        {
            DisplayErrorScreen();
        }
    }

    public void DisplayErrorScreen()
    {
        if (errorOpened == false)
        {
            errorOpened = true;    
        }
    }

    public void ReLocalizeTexts()
    {
        print("TRANSLATING TEXTS");
        LocalizationSystem.language = (LocalizationSystem.Language)langIndex;
        print("TO: " + LocalizationSystem.language.ToString());

        FindAppropriateFont();

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

    Dictionary<string, TMP_FontAsset> appropriateFonts = new Dictionary<string, TMP_FontAsset>();

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

    void FindAppropriateFont()
    {
        TMP_FontAsset value;
        appropriateFonts.TryGetValue(currentLanguage, out value);

        appropriateFont = value;
    }

    public TMP_FontAsset GiveAppropriateFont(string language)
    {
        TMP_FontAsset value;
        appropriateFonts.TryGetValue(language, out value);

        return value;
    }

    private void OnApplicationQuit()
    {
        if(errorOpened == false && EULA_Accepted)
        {
            Debug.Log("Saving on exit");
            SaveGameData();
        }

        if (!EULA_Accepted)
        {
            //shut game, delete all gamedata, hard factory reset
            DirectoryInfo dataDir = new DirectoryInfo(Application.persistentDataPath);
            dataDir.Delete(true);
        }
    }
}
