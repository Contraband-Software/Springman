using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Events;

using PlatformIntegrations;
using Architecture.Localisation;
using Backend;

namespace Architecture
{
    public class UserGameData : AbstractSingleton<UserGameData>
    {
        #region EVENTS
        //Emits an event containing the current language index
        public sealed class LanguageLoadEventType : UnityEvent<int> { }
        public LanguageLoadEventType LanguageLoadEvent { private set; get; }
        public UnityEvent ErrorEvent { private set; get; }
        public UnityEvent ShowEULA { private set; get; }
        public UnityEvent RequestColourData { private set; get; }
        #endregion

        #region GAME_DATA
        public int allTimeHighscore { get; set; }
        public bool musicOn { get; set; }
        public bool soundsOn { get; set; }
        public int langIndex { get; set; }
        public int gold { get; set; }
        public int silver { get; set; }
        public int ads { get; set; }
        public bool tutorialComplete { get; set; } = false;

        public bool EULA_Accepted { get; set; } = false;
        #endregion

        #region COSMETICS_DATA
        [Header("Colour Data")]
        [SerializeField] Color topColor;
        [SerializeField] Color bottomColor;
        [SerializeField] Color springColor;

        [SerializeField] Vector3 topObject;
        [SerializeField] Vector3 bottomObject;
        [SerializeField] Vector3 springObject;

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

        [Header("Premium Skins")]
        public bool currentSkinPremium = false;
        public string activePremiumSkinName;
        public List<string> unlockedPremiums = new List<string>();
        public List<string> allPremiums = new List<string>();
        public List<string> allPremiumCodes = new List<string>();

        public List<string> glowColours = new List<string>();
        public List<bool> hasSpecialColour = new List<bool>();
        public List<bool> specialColourModes = new List<bool>();
        #endregion

        string gameDataPath = "";
        SocialManager socialManager = null;

        protected override void SingletonAwake()
        {
            LanguageLoadEvent = new LanguageLoadEventType();
            ErrorEvent = new UnityEvent();
            ShowEULA = new UnityEvent();
            RequestColourData = new UnityEvent();

            gameDataPath = Path.Combine(Application.persistentDataPath, "gamedatafile.gd");

            socialManager = IntegrationsManager.Instance.socialManager;
            socialManager.SaveDataWriteCallback.AddListener((bool status) =>
            {
                if (!status)
                {
                    Debug.Log("MenuData: Save data cloud write unsuccessful");
                }
                else
                {
                    Debug.Log("MenuData: Save data written to cloud successfully");
                }
            });
        }

        private void Start()
        {
#if !UNITY_EDITOR
            SaveData loadedSaveData = (SaveData)IntegrationsManager.instance.socialManager.GetCachedSaveGame();
            Debug.Log(IntegrationsManager.instance.socialManager.GetCachedSaveGame());
            Debug.Log(loadedSaveData);

            //no data in loadedSaveData, but the load from cloud was succesful = save a default file onto the cloud
            if(PlatformIntegrations.IntegrationsManager.Instance.socialManager.HasLoadedFromCloud()
            && loadedSaveData == null){
                
                DefaultDataFileSettings();
                SaveGameData();
                ShowEULA.Invoke();
            }

            ReLocalizeTexts();
#else
            // Unity editor local fallback
            // create default save data to create dummy file
            CreateFirstDataFile();

            File.SetAttributes(gameDataPath, FileAttributes.Normal);
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(gameDataPath, FileMode.Open);

            SaveData data = formatter.Deserialize(stream) as SaveData;
            stream.Close();

            allTimeHighscore = data.highscore;
            musicOn = data.musicOn;
            soundsOn = data.soundsOn;
            LocalizationSystem.Instance.CurrentLanguage = (LocalizationSystem.Language)data.langIndex;

            langIndex = data.langIndex;
            gold = data.gold;
            silver = data.silver;
            tutorialComplete = data.tutorialComplete;
            ads = data.ads;

            EULA_Accepted = true;
#endif
        }

        void CreateFirstDataFile()
        {
            float availableSpace = SimpleDiskUtils.DiskUtils.CheckAvailableSpace();
            if (availableSpace > 10)
            {
                if (!File.Exists(gameDataPath))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    FileStream stream = new FileStream(gameDataPath, FileMode.OpenOrCreate);

                    DefaultDataFileSettings();
                    SaveData data = PackSaveDataWithCurrentValues();

                    formatter.Serialize(stream, data);
                    stream.Close();

                    Debug.Log("CREATED FIRST GAME DATA FILE");

                    ShowEULA.Invoke();
                }
            }
            else
            {
                ErrorEvent.Invoke();
            }
        }

        private void DefaultDataFileSettings()
        {
            EULA_Accepted = false;
            musicOn = true;
            soundsOn = true;
            gold = 60;
            silver = 300;
            tutorialComplete = false;
            ads = 0;
            langIndex = (int)LocalizationSystem.Instance.CurrentLanguage;

            unlockedColours.Add("FFFFFF");
            unlockedColours.Add("373737");

            //unlockedPremiums.Add("lpqok951139");
            unlockedPremiums = new List<string>{ "lpqok951139", "bonvmm916571", "jkhqys871421", "xxclpu871531", "kljqye098901", "opiuqa9815211", "loiqyv904091", "gqulpo090861"
                , "oilpqu876019", "vbtqeq651064"};


            unlockedSkins.Add("109651fc");

            currentSkin = "109651fc";

            RequestColourData.Invoke();

            //skinSelectorPremium.CollectGlowColours(); RequestColourData.Invoke() does this to avoid needing a reference

            playerCosmeticType = PlayerCosmeticType.Color;
            topColor = Color.white;
            bottomColor = Color.white;
            springColor = Utilities.StringToColor("373737");
        }

        private SaveData PackSaveDataWithCurrentValues()
        {
            SaveData data = new SaveData(
                this.allTimeHighscore,
                this.musicOn, this.soundsOn,
                LocalizationSystem.Instance.CurrentLanguage.ToString().ToLower(), this.langIndex,
                this.gold, this.silver,
                tutorialComplete,
                this.ads,
                Utilities.ColorToString(topColor),
                Utilities.ColorToString(bottomColor),
                Utilities.ColorToString(springColor),
                topObject, bottomObject, springObject, 
                (int)playerCosmeticType, 
                unlockedColours, unlockedSkins, 
                currentSkin, unlockedPremiums, 
                currentSkinPremium, 
                glowColours, hasSpecialColour, specialColourModes
            );

            return data;
        }

        public void SaveGameData()
        {
            SaveData data = PackSaveDataWithCurrentValues();

#if UNITY_EDITOR
            // Editor local save fallback
            float availableSpace = SimpleDiskUtils.DiskUtils.CheckAvailableSpace();
            if (availableSpace > 10)
            {
                BinaryFormatter formatter = new BinaryFormatter();
                File.SetAttributes(gameDataPath, FileAttributes.Normal);

                FileStream stream = new FileStream(gameDataPath, FileMode.Create);

                File.SetAttributes(gameDataPath, FileAttributes.ReadOnly);

                formatter.Serialize(stream, data);
                stream.Close();

                Debug.Log("MenuData: Save data written to local fallback");
            }
            else
            {
                ErrorEvent.Invoke();
            }
#else
            if (socialManager.IsAvailable() && socialManager.SaveGameLoaded())
            {
                socialManager.SaveGame(data);
            }
            else
            {
                ErrorEvent.Invoke();
                Debug.Log("Social integration not availible");
                throw new System.NotImplementedException("GOOGLE CANNOT BE REACHED, WE SHOULD ERROR OUT AND KILL THE APP HERE");
                //killing an app without saving could be dangerous. Potential of losing a premium purchase
            }
#endif
        }

        private void OnApplicationQuit()
        {
            if (EULA_Accepted)
            {
                //There isnt a check here for if the error screen is open (Really Bad)
                    Debug.Log("Saving on exit");
                    SaveGameData();
            }
            else
            {
                //shut game, delete all gamedata, hard factory reset
                DirectoryInfo dataDir = new DirectoryInfo(Application.persistentDataPath);
                dataDir.Delete(true);
            }
        }
    }
}

