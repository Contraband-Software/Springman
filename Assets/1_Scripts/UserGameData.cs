using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Events;

using PlatformIntegrations;
using UnityEngine.Analytics;

//namespace Architecture
//{
//    public class UserGameData : Backend.AbstractSingleton<UserGameData>
//    {
//        #region EVENTS
//        //Emits an event containing the current language index
//        public sealed class LanguageLoadEvent : UnityEvent<int> { }
//        public LanguageLoadEvent languageLoadEvent { private set; get; }
//        #endregion

//        #region GAME_DATA
//        public int allTimeHighscore     { get; set; }
//        public bool musicOn             { get; set; }
//        public bool soundsOn            { get; set; }
//        public int langIndex            { get; set; }
//        public int gold                 { get; set; }
//        public int silver               { get; set; }
//        public int ads                  { get; set; }
//        public bool tutorialComplete    { get; set; } = false;
//        #endregion

//        string gameDataPath = "";
//        SocialManager socialManager = null;

//        protected override void SingletonAwake()
//        {
//            gameDataPath = Path.Combine(Application.persistentDataPath, "gamedatafile.gd");

//            socialManager = IntegrationsManager.Instance.socialManager;
//            socialManager.SaveDataWriteCallback.AddListener((bool status) => {
//                if (!status)
//                {
//                    Debug.Log("MenuData: Save data cloud write unsuccessful");
//                }
//                else
//                {
//                    Debug.Log("MenuData: Save data written to cloud successfully");
//                }
//            });
//        }

//        private void Start()
//        {
//#if !UNITY_EDITOR
//        SaveData loadedSaveData = (SaveData)IntegrationsManager.instance.socialManager.GetCachedSaveGame();
//        Debug.Log(IntegrationsManager.instance.socialManager.GetCachedSaveGame());
//        Debug.Log(loadedSaveData);


//        ReLocalizeTexts();
//        //load the vloud
//#else
//            //create default save data to create dummy file
//            //create first data file()
//            CreateFirstDataFile();
//            LoadGameData();
//#endif
//        }

//        public void CreateFirstDataFile()
//        {
//            float availableSpace = SimpleDiskUtils.DiskUtils.CheckAvailableSpace();
//            if (availableSpace > 10)
//            {
//                if (!File.Exists(path))
//                {
//                    EULA_Accepted = false;
//                    musicOn = true;
//                    soundsOn = true;
//                    gold = 60;
//                    silver = 300;
//                    tutorialComplete = false;
//                    ads = 0;

//                    //To be implemented
//                    //langIndex = LocalisationClass.GetSystemLang();


//                    BinaryFormatter formatter = new BinaryFormatter();
//                    FileStream stream = new FileStream(path, FileMode.OpenOrCreate);

//                    SaveData data = new SaveData(this.allTimeHighscore, this.musicOn, this.soundsOn, this.currentLanguage, this.langIndex, this.gold, this.silver,
//                        tutorialComplete, this.ads);

//#if !UNITY_EDITOR
//                File.SetAttributes(path, FileAttributes.ReadOnly);
//#endif

//                    formatter.Serialize(stream, data);
//                    stream.Close();

//                    Debug.Log("CREATED FIRST GAME DATA FILE");



//                    eula.Show();
//                    //if (sm.isAvailable()) { sm.ShowSaveGameSelectUI(); }
//                }
//                else
//                {
//                    //if (sm.isAvailable()) { sm.LoadSaveGame(); }
//                }
//            }
//            else
//            {
//                DisplayErrorScreen();
//            }
//        }

//        public void SaveGameData()
//        {
//            SaveData data = new SaveData(
//                this.allTimeHighscore,
//                this.musicOn,
//                this.soundsOn,
//                this.langIndex,
//            this.gold, this.silver,
//                tutorialComplete,
//                ads
//            );

//#if UNITY_EDITOR
//            float availableSpace = SimpleDiskUtils.DiskUtils.CheckAvailableSpace();
//            if (availableSpace > 10)
//            {
//                BinaryFormatter formatter = new BinaryFormatter();
//                File.SetAttributes(path, FileAttributes.Normal);

//                FileStream stream = new FileStream(path, FileMode.Create);

//                File.SetAttributes(path, FileAttributes.ReadOnly);

//                formatter.Serialize(stream, data);
//                stream.Close();

//                Debug.Log("MenuData: Save data written to local fallback");
//            }
//            else
//            {
//                DisplayErrorScreen();
//            }
//#else
//        if (sm.IsAvailable() && sm.SaveGameLoaded())
//        {
//            sm.SaveGame(data);
//        }
//        else
//        {
//            DisplayErrorScreen();
//            Debug.Log("Social integration not availible");
//            throw new System.NotImplementedException("GOOGLE CANNOT BE REACHED, WE SHOULD ERROR OUT AND KILL THE APP HERE");
//        }
//#endif
//        }
//    }
//}