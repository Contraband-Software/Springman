using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PlatformIntegrations
{
    using GooglePlayGames;
    using GooglePlayGames.BasicApi;
    using GooglePlayGames.BasicApi.SavedGame;
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using UnityEngine.SocialPlatforms;

    public class SocialManager : MonoBehaviour
    {
        #region EVENTS
        public class AuthenticationEvent : UnityEvent<bool> { }
        public AuthenticationEvent AuthenticatorCallback;
        public class SaveDataLoadEvent : UnityEvent<Object> { }
        public SaveDataLoadEvent SaveDataLoadCallback;
        public class SaveDataWriteEvent : UnityEvent<bool> { }
        public SaveDataWriteEvent SaveDataWriteCallback;
        #endregion

        bool available = false;
        bool saveGameLoaded = false;
        ISavedGameMetadata currentSavedGameMetadata = null;

        private void Awake()
        {
            AuthenticatorCallback = new AuthenticationEvent();
            AuthenticatorCallback.AddListener((bool status) =>
            {
                Debug.Log("GPGS: Platform signin status: " + status.ToString());
            });
            SaveDataLoadCallback = new SaveDataLoadEvent();
            SaveDataLoadCallback.AddListener((object data) =>
            {
                Debug.Log("GPGS: Cloud save loaded ");
            });
            SaveDataWriteCallback = new SaveDataWriteEvent();
            SaveDataWriteCallback.AddListener((bool status) =>
            {
                Debug.Log("GPGS: Game data write status: " + status.ToString());
            });
        }
        private void Start()
        {
#if UNITY_ANDROID
            PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
#elif UNITY_IOS
            //Log into icloud/game center
#endif
        }

        /// <summary>
        /// To be attached to a button to prompt and allow the user to sign into GPG.
        /// </summary>
        public void TrySignIn()
        {
#if UNITY_ANDROID
            PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);
#elif UNITY_IOS
            //Log into icloud/game center
#endif
        }

        /// <summary>
        /// Returns if google play game services is available currently (meaning the user is signed in and connected to the internet)
        /// </summary>
        /// <returns></returns>
        public bool IsAvailable()
        {
            return available;
        }
        
        /// <summary>
        /// Returns if a cloud save is loaded
        /// </summary>
        /// <returns></returns>
        public bool SaveGameLoaded()
        {
            return saveGameLoaded;
        }

        /// <summary>
        /// Opens the Social leaderboard UI
        /// </summary>
        public void OpenLeaderBoardUI()
        {
            Social.ShowLeaderboardUI();
        }

#region GOOGLE_PLAY_GAMES
        internal void ProcessAuthentication(SignInStatus status)
        {
            if (status == SignInStatus.Success)
            {
                // Continue with Play Games Services
                available = true;
                OpenSavedGame("UserGameSave.dat");
            }
            else
            {
                available = false;
                // Disable your integration with Play Games Services or show a login button
                // to ask users to sign-in. Clicking it should call
                // PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication).
            }

            AuthenticatorCallback.Invoke(available);
        }

        /// <summary>
        /// Posts a score to the global game leaderboard
        /// </summary>
        /// <param name="score"></param>
        public void PostLeaderboardScore(int score, Action<bool> callback)
        {
            //CgkI6NDuufMeEAIQAQ is the API ID for OUR leaderboard
            Social.ReportScore(score, "CgkI6NDuufMeEAIQAQ", (bool success) => {
                callback(success);
            });
        }

        //SAVE GAMES
        private void OpenSavedGame(string filename)
        {
            ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
            savedGameClient.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpened);
        }
        private void OnSavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata game)
        {
            Debug.Log("GPGS: Game Data Load Status: " + status.ToString());

            if (status == SavedGameRequestStatus.Success)
            {
                currentSavedGameMetadata = game;
                saveGameLoaded = true;

                ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
                savedGameClient.ReadBinaryData(game, OnSavedGameDataRead);
            }
            else
            {
                // handle error
            }
        }

        private void OnSavedGameDataRead(SavedGameRequestStatus status, byte[] data)
        {
            //Debug.Log("GPGS: Game Data Read Status: " + status.ToString());

            if (status == SavedGameRequestStatus.Success)
            {
                // handle processing the byte array data
                SaveDataLoadCallback.Invoke(ByteArrayToObject(data));
            }
            else
            {
                // handle error
            }
        }

        /// <summary>
        /// Saves an object to the cloud save
        /// </summary>
        /// <param name="savedData">A serializable object with some serializable fields to save</param>
        /// <param name="totalPlaytime">The total play time for this save so far</param>
        public void SaveGame(object savedData, TimeSpan totalPlaytime)
        {
            byte[] saveBlob = ObjectToByteArray(savedData);
            ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

            SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
            builder = builder
                .WithUpdatedPlayedTime(totalPlaytime);

            SavedGameMetadataUpdate updatedMetadata = builder.Build();
            savedGameClient.CommitUpdate(currentSavedGameMetadata, updatedMetadata, saveBlob, OnSavedGameWritten);
        }

        private void OnSavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
        {
            if (status == SavedGameRequestStatus.Success)
            {
                SaveDataWriteCallback.Invoke(true);
            }
            else
            {
                SaveDataWriteCallback.Invoke(false);
            }
        }
        #endregion

        private byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
        private object ByteArrayToObject(byte[] arrBytes)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                BinaryFormatter binForm = new BinaryFormatter();

                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);

                //empty file
                if (memStream.Length != 0)
                {
                    return binForm.Deserialize(memStream);
                } else
                {
                    return null;
                }
            }
        }
    }
}