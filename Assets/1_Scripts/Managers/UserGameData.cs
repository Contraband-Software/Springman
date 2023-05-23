using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Events;

using PlatformIntegrations;
using Architecture.Localisation;
using Backend;
using UnityEngine.Purchasing;

namespace Architecture.Managers
{
    public class UserGameData : AbstractSingleton<UserGameData>
    {
        #region EVENTS
        //Emits an event containing the current language index
        public sealed class LanguageLoadEventType : UnityEvent<int> { }
        public LanguageLoadEventType LanguageLoadEvent { private set; get; } = new LanguageLoadEventType();
        public UnityEvent ErrorEvent { private set; get; } = new UnityEvent();
        public UnityEvent RequestColourData { private set; get; } = new UnityEvent();
        #endregion

        #region GAME_DATA
        public int allTimeHighscore { get; set; } = 0;
        public bool musicOn { get; set; } = true;
        public bool soundsOn { get; set; } = true;
        public int langIndex { get; set; } = 0;
        public int gold { get; set; } = 0;
        public int silver { get; set; } = 0;
        public int ads { get; set; } = 0;
        public bool tutorialComplete { get; set; } = false;

        public bool EULA_Accepted { get; set; } = false;
        #endregion

        #region COSMETICS_DATA
        public Color themeColour;

        public Color topColor;
        public Color bottomColor;
        public Color springColor;

        public Vector3 topObject;
        public Vector3 bottomObject;
        public Vector3 springObject;

        public List<string> unlockedColours = new List<string>();
        public List<string> allColours = new List<string>();

        public string currentSkin;

        public SkinSpecsSolid cSpecs = new SkinSpecsSolid();

        public List<string> unlockedSkins = new List<string>();
        public List<string> allSkins = new List<string>();

        public List<string> allSkinsCodes = new List<string>();
        public List<SkinSpecsSolid> allSkinSpecs = new List<SkinSpecsSolid>();

        public enum PlayerCosmeticType { None, Color };
        public PlayerCosmeticType playerCosmeticType = PlayerCosmeticType.None;

        public bool currentSkinPremium = false;
        public string activePremiumSkinName;
        public List<string> allPremiums = new List<string>();
        public List<string> allPremiumCodes = new List<string>();

        //COSMETICS PUBLIC INTERFACE
        [HideInInspector] public List<string> glowColours { get; set; } = new List<string>();
        [HideInInspector] public List<bool> hasSpecialColour { get; set; } = new List<bool>();
        [HideInInspector] public List<bool> specialColourModes { get; set; } = new List<bool>();
        #endregion

        string gameDataPath = "";

#pragma warning disable S1450
        SocialManager socialManager = null;
#pragma warning restore S1450

        #region UNITY
        protected override void SingletonAwake()
        {
            ProductCatalog pc = ProductCatalog.LoadDefaultCatalog();
            foreach (ProductCatalogItem item in pc.allProducts)
            {
                allPremiums.Add(item.defaultDescription.Title);
                allPremiumCodes.Add(item.id);
            }

#if UNITY_EDITOR
            gameDataPath = Path.Combine(Application.persistentDataPath, "gamedatafile.gd");
#endif

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
            socialManager.SaveDataLoadCallback.AddListener((bool status, object data) =>
            {
                if (status)
                {
#pragma warning disable S5773
                    if (data == null)
                    {
                        Debug.Log("UserGameData: CREATED FIRST GAME DATA FILE (cloud)");
                        DefaultDataFileSettings();
                        SaveGameData();
                        EULA_Accepted = false;
                    }
                    else
                    {
                        Debug.Log("UserGameData: READ GAME DATA FILE");
                        UnpackLoadedSaveDataFile(data as SaveData);
                    }
#pragma warning restore S5773
                }
                else
                {
                    Debug.Log("UserGameData: no connection to cloud!");
                }
            });
        }

        private void Start()
        {
#if UNITY_EDITOR

            // This code allows the game to be tested in the editor
            // without it, we start a fresh save every run (annoying).

            #region LOCAL_FALLBACK

            // create default user data to save to dummy file
            float availableSpace = SimpleDiskUtils.DiskUtils.CheckAvailableSpace();
            if (availableSpace > 10)
            {
                if (!File.Exists(gameDataPath))
                {
                    Debug.Log("UserGameData: CREATED FIRST GAME DATA FILE (local)");

                    BinaryFormatter bf = new BinaryFormatter();
                    FileStream localFile = new FileStream(gameDataPath, FileMode.OpenOrCreate);

                    DefaultDataFileSettings();
                    SaveData saveData = PackSaveDataWithCurrentValues();

                    bf.Serialize(localFile, saveData);
                    localFile.Close();

                    EULA_Accepted = false;

                    localFile.Close();
                }
            }
            else
            {
                ErrorEvent.Invoke();
            }

            #region READ_FILE

            File.SetAttributes(gameDataPath, FileAttributes.Normal);
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(gameDataPath, FileMode.Open);

#pragma warning disable S5773
            // is actually safe
            SaveData data = formatter.Deserialize(stream) as SaveData;
#pragma warning restore S5773

            stream.Close();
            UnpackLoadedSaveDataFile(data);

            #endregion

            #endregion
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

                //reset cloud file?
                dataDir.Delete(true);
            }
        }
        #endregion

        #region SAVING
        /// <summary>
        /// Loads a save data struct into this class
        /// </summary>
        /// <param name="data"></param>
        private void UnpackLoadedSaveDataFile(SaveData data)
        {
            //GAMEDATA
            allTimeHighscore = data.highscore;
            musicOn = data.musicOn;
            soundsOn = data.soundsOn;

            langIndex = data.langIndex;
            LocalizationSystem.Instance.CurrentLanguage = (LocalizationSystem.Language)data.langIndex;

            gold = data.gold;
            silver = data.silver;
            EULA_Accepted = data.eulaAccepted;
            tutorialComplete = data.tutorialComplete;
            ads = data.ads;

            //COSMETICS DATA
            themeColour = Utilities.StringToColor(data.themeColour);

            topColor = Utilities.StringToColor(data.topColor);
            bottomColor = Utilities.StringToColor(data.bottomColor);
            springColor = Utilities.StringToColor(data.springColor);

            topObject = data.topObject.V3;
            bottomObject = data.bottomObject.V3;
            springObject = data.springObject.V3;

            playerCosmeticType = (PlayerCosmeticType)data.cosType;
            unlockedColours = data.unlockedColours;
            unlockedSkins = data.unlockedSkins;
            currentSkin = data.currentSkin;
            currentSkinPremium = data.currentSkinPremium;
            glowColours = data.glowColours;
            hasSpecialColour = data.hasSpecialColour;
            specialColourModes = data.specColModes;

            
        }

        public void CheckIfLoadedSkinPremium()
        {
            //Ensures the premium skin name is set if the current skin is premium
            if (currentSkinPremium)
            {
                activePremiumSkinName = allPremiums[allPremiumCodes.IndexOf(currentSkin)];
            }
        }

        /// <summary>
        /// Initializes the user data to its default state
        /// </summary>
        private void DefaultDataFileSettings()
        {
            EULA_Accepted = false;
            tutorialComplete = false;

            musicOn = true;
            soundsOn = true;
            langIndex = (int)LocalizationSystem.Instance.CurrentLanguage;

            gold = 60;
            silver = 300;
            ads = 0;

            themeColour = Utilities.StringToColor("9900BE");

            unlockedColours.Add("FFFFFF");
            unlockedColours.Add("373737");

            unlockedSkins.Add("109651fc");
            currentSkin = "109651fc";

            RequestColourData.Invoke();

            playerCosmeticType = PlayerCosmeticType.Color;
            topColor = Color.white;
            bottomColor = Color.white;
            springColor = Utilities.StringToColor("373737");
        }

        /// <summary>
        /// Packs the user data into a dedicated struct
        /// </summary>
        /// <returns>A class ready to be serialized</returns>
        private SaveData PackSaveDataWithCurrentValues()
        {
            RequestColourData.Invoke();

            SaveData data = new SaveData(
                this.allTimeHighscore,
                this.musicOn, this.soundsOn,
                LocalizationSystem.Instance.CurrentLanguage.ToString().ToLower(), this.langIndex,
                this.gold, this.silver,
                EULA_Accepted,
                tutorialComplete,
                this.ads,
                Utilities.ColorToString(themeColour),
                Utilities.ColorToString(topColor),
                Utilities.ColorToString(bottomColor),
                Utilities.ColorToString(springColor),
                topObject, bottomObject, springObject, 
                (int)playerCosmeticType, 
                unlockedColours, unlockedSkins, 
                currentSkin, 
                currentSkinPremium, 
                glowColours, hasSpecialColour, specialColourModes
            );

            return data;
        }
        #endregion

        #region PUBLIC_INTERFACE
        public bool ShowEULA { private set; get; } = false;

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
            if (socialManager.IsSignedIn() && socialManager.HasConnectedToCloud())
            {
                socialManager.WriteToCloudSave(data);
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

        /// <summary>
        /// Setter for the current skin ID, which is a fucking public variable lol
        /// </summary>
        /// <param name="ID"></param>
        public void HardPassSkinID(string ID)
        {
            currentSkin = ID;
        }
        #endregion
    }
}
