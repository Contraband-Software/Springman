using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Events;

using PlatformIntegrations;
using UnityEngine.Analytics;
using Architecture.Localisation;

namespace Architecture
{
    public class UserGameData : Backend.AbstractSingleton<UserGameData>
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
            LocalizationSystem.language = (LocalizationSystem.Language)data.langIndex;

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
            //To be implemented
            //langIndex = LocalisationClass.GetSystemLang();

            unlockedColours.Add("FFFFFF");
            unlockedColours.Add("373737");

            //////////////
            //unlockedPremiums.Add("lpqok951139");
            unlockedPremiums = new List<string>{ "lpqok951139", "bonvmm916571", "jkhqys871421", "xxclpu871531", "kljqye098901", "opiuqa9815211", "loiqyv904091", "gqulpo090861"
                , "oilpqu876019", "vbtqeq651064"};
            /////////////

            unlockedSkins.Add("109651fc");

            currentSkin = "109651fc";

            RequestColourData.Invoke();

            //skinSelectorPremium.CollectGlowColours(); RequestColourData.Invoke() does this to avoid needing a reference

            playerCosmeticType = PlayerCosmeticType.Color;
            topColor = Color.white;
            bottomColor = Color.white;
            springColor = UserGameDataHandlingUtilities.StringToColor("373737");
        }

        private SaveData PackSaveDataWithCurrentValues()
        {
            SaveData data = new SaveData(
                this.allTimeHighscore,
                this.musicOn, this.soundsOn,
                LocalizationSystem.language.ToString().ToLower(), this.langIndex,
                this.gold, this.silver,
                tutorialComplete,
                this.ads,
                UserGameDataHandlingUtilities.ColorToString(topColor),
                UserGameDataHandlingUtilities.ColorToString(bottomColor),
                UserGameDataHandlingUtilities.ColorToString(springColor),
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
    }

    public static class UserGameDataHandlingUtilities
    {
        public static int HexToDec(string hex)
        {
            int dec = System.Convert.ToInt32(hex, 16);
            return dec;
        }

        public static string DecToHex(int value)
        {
            return value.ToString("X2");
        }

        public static string FloatToNormalizedToHex(float value)
        {
            return DecToHex(Mathf.RoundToInt(value * 255f));
        }

        public static float HexToFloatNormalized(string hex)
        {
            return HexToDec(hex) / 255f;
        }

        public static Color StringToColor(string hexString)
        {
            float red = HexToFloatNormalized(hexString.Substring(0, 2));
            float green = HexToFloatNormalized(hexString.Substring(2, 2));
            float blue = HexToFloatNormalized(hexString.Substring(4, 2));
            float alpha = 1f;
            if (hexString.Length >= 8)
            {
                alpha = HexToFloatNormalized(hexString.Substring(6, 2));
            }

            return new Color(red, green, blue, alpha);
        }
        public static string ColorToString(Color color, bool useAlpha = false)
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
    }
}

