using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Events;

using PlatformIntegrations;
using UnityEngine.Analytics;

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

        string gameDataPath = "";
        SocialManager socialManager = null;

        protected override void SingletonAwake()
        {
            LanguageLoadEvent = new LanguageLoadEventType();
            ErrorEvent = new UnityEvent();
            ShowEULA = new UnityEvent();

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
            ReLocalizeTexts();
#else
            // Unity editor local fallback
            // create default save data to create dummy file
            CreateFirstDataFile();
            LoadLocalGameData();
#endif
        }

        void CreateFirstDataFile()
        {
            float availableSpace = SimpleDiskUtils.DiskUtils.CheckAvailableSpace();
            if (availableSpace > 10)
            {
                if (!File.Exists(gameDataPath))
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


                    BinaryFormatter formatter = new BinaryFormatter();
                    FileStream stream = new FileStream(gameDataPath, FileMode.OpenOrCreate);

                    SaveData data = new SaveData(
                        this.allTimeHighscore, 
                        this.musicOn, this.soundsOn, 
                        LocalizationSystem.language.ToString().ToLower(), this.langIndex, 
                        this.gold, this.silver,
                        tutorialComplete, 
                        this.ads
                    );

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

        private void LoadLocalGameData()
        {
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

            //Debug.Log("LOADED GAMEDATA FILE");
        }

        public void SaveGameData()
        {
            SaveData data = new SaveData(
                this.allTimeHighscore,
                this.musicOn, this.soundsOn,
                LocalizationSystem.language.ToString().ToLower(), this.langIndex,
                this.gold, this.silver,
                tutorialComplete,
                this.ads
            );

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
            }
#endif
        }
    }
}