using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using SimpleDiskUtils;

using Architecture.Localisation;

/*
 * 
 * 
 * 
 * 
 * DONT FIX ANYTHING HERE, THE IDEA IS TO REMOVE THIS FROM THE PROJECT
 * 
 * 
 * 
 * 
 */
public class GameData : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highscore;
    public CanvasGroup curtainCG;

    [Header("TutorialStatus")]
    public bool tutorialComplete = false;
    public bool allowSlideMove = true;

    [Header("Language Fonts")]
    public TMP_FontAsset appropriateFont;
    public TMP_FontAsset Latin_Cyrillic;
    public TMP_FontAsset Latin;
    public TMP_FontAsset Vietnamese;
    public TMP_FontAsset Chinese;
    public TMP_FontAsset Hindi;
    public TMP_FontAsset Arabic;

    [Header("Error Screen")]
    public bool errorOpened = false;

    [Header("Save Data")]

    public int allTimeHighscore;
    public bool musicOn;
    public bool soundsOn;
    public string currentLanguage;
    public int langIndex;
    public int gold;
    public int silver;
    public int ads;

    [Header("Other")]
    public int score;
    public int currentGameHighscore;

    public bool Paused;

    public Camera cam;
    Vector3 topRight;

    [Header("Platform variables")]

    public float minPlatLength;
    public float maxPlatLength;
    public float CapShrinkAtScore;
    public float platLength;
    public bool nextPlatIsHole = false;

    [Header("Flying Enemy Variables")]

    public float highestSpawnTime = 15f;
    public float lowestSpawnTime = 3f;
    public float sinGraphExaggeration = 1f;
    public int flyingEnemiesKilled;

    [Header("Enemies Active")]
    public List<GameObject> enemiesActive = new List<GameObject>();

    void Awake()
    {
        Application.targetFrameRate = 60;
        Paused = false;

        CreateFirstDataFile();
        InitialiseAppropriateFonts();
        LoadGameData();

        allowSlideMove = true;

        scoreText.text = "0";
        score = 0;
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane)); //Coords of top right corner of screen

        flyingEnemiesKilled = 0;
        maxPlatLength = (topRight.x * 2) / 3;
        CalculateMinPlatLength();
    }

    public void Start()
    {

        LeanTween.cancelAll();
        curtainCG.alpha = 1f;
        LeanTween.alphaCanvas(curtainCG, 0f, 0.4f).setIgnoreTimeScale(true);
    }

    void Update()
    {
        topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane)); //Coords of top right corner of screen

        UpdateScore();
        UpdateHighscore();

        highscore.text = allTimeHighscore.ToString();
    }

    void UpdateScore()
    {
        string scoreAsString = score.ToString();
        scoreText.text = scoreAsString;
    }

    public void CalculateMinPlatLength()
    {
        if(score > 0)
        {
            float percentage = (CapShrinkAtScore - score) / CapShrinkAtScore;
            minPlatLength = Mathf.Max(0.4f, maxPlatLength * percentage);
        }
        else
        {
            minPlatLength = maxPlatLength;
        }
    }

    void UpdateHighscore()
    {
        if(Convert.ToInt32(score) > currentGameHighscore)
        {
            currentGameHighscore = Convert.ToInt32(score);
        }
        if(currentGameHighscore > allTimeHighscore)
        {
            allTimeHighscore = currentGameHighscore;

            //MAKE THIS HAPPEN UPON A CONDITION SUCH AS IF THAT IT HAPPENS ONCE PLAYER DIES
        }
    }

    public void CreateFirstDataFile()
    {
        string path = Path.Combine(Application.persistentDataPath, "gamedatafile.gd");
        if (!File.Exists(path))
        {
            float availableSpace = SimpleDiskUtils.DiskUtils.CheckAvailableSpace();
            if (availableSpace > 10)
            {
                musicOn = true;  //Default values 
                soundsOn = true;
                gold = 0;
                silver = 0;
                tutorialComplete = false;
                currentLanguage = "english";
                langIndex = 0;
                ads = 0;

                BinaryFormatter formatter = new BinaryFormatter();

                FileStream stream = new FileStream(path, FileMode.Create);

                //SaveData data = new SaveData(this.allTimeHighscore, this.musicOn, this.soundsOn, this.currentLanguage, this.langIndex, this.gold, this.silver, 
                    //tutorialComplete, ads);

                File.SetAttributes(path, FileAttributes.ReadOnly);

                //formatter.Serialize(stream, data);
                stream.Close();

                ReLocalizeTexts();
            }
            else
            {
                DisplayErrorScreen();
            }
        }
    }

    public void SaveGameData()
    {
        float availableSpace = SimpleDiskUtils.DiskUtils.CheckAvailableSpace();
        if (availableSpace > 10)
        {

            BinaryFormatter formatter = new BinaryFormatter();
            string path = Path.Combine(Application.persistentDataPath, "gamedatafile.gd");
            File.SetAttributes(path, FileAttributes.Normal);

            FileStream stream = new FileStream(path, FileMode.Create);

            //SaveData data = new SaveData(this.allTimeHighscore, this.musicOn, this.soundsOn, this.currentLanguage, this.langIndex, this.gold, this.silver, 
            //    tutorialComplete, ads);

            File.SetAttributes(path, FileAttributes.ReadOnly);

            //formatter.Serialize(stream, data);
            stream.Close();
        }
        else
        {
            DisplayErrorScreen();
        }

    }

    public void LoadGameData()
    {
        float availableSpace = SimpleDiskUtils.DiskUtils.CheckAvailableSpace();
        if (availableSpace > 10)
        {

            string path = Path.Combine(Application.persistentDataPath, "gamedatafile.gd");
            if (File.Exists(path))
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
            }
            ReLocalizeTexts();
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
        LocalizationSystem.Instance.CurrentLanguage = (LocalizationSystem.Language)langIndex;

        FindAppropriateFont();

        TextLocaliserUI[] textItems = FindObjectsOfType(typeof(TextLocaliserUI)) as TextLocaliserUI[];
        foreach (TextLocaliserUI text in textItems)
        {
            if(text.key == "localLang")
            {
                text.language = currentLanguage;
                text.ApplyFontForLangMenu();
            }
            else
            {
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
        if(errorOpened == false)
        {
            SaveGameData();
        }
    }
}
