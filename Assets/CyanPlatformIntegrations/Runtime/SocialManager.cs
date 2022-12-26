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

    public class SocialManager
    {
        const string logDecorator = "GPGS: ";

#region EVENTS
        public class AuthenticationEvent : UnityEvent<bool> { }
        public AuthenticationEvent AuthenticatorCallback { get; private set; }
        //public class SaveDataLoadEvent : UnityEvent<bool, object> { }
        //public SaveDataLoadEvent SaveDataLoadCallback { get; private set; }
        public class SaveDataWriteEvent : UnityEvent<bool> { }
        public SaveDataWriteEvent SaveDataWriteCallback { get; private set; }
        #endregion

        private const string cloudSaveFile = "UserGameSave.dat";

        bool available = false;
        ISavedGameMetadata currentSavedGameMetadata = null;
        byte[] cache;

        TimeSpan sessionStart;

        public SocialManager()
        {
            sessionStart = DateTime.Now.TimeOfDay;

            AuthenticatorCallback = new AuthenticationEvent();
            AuthenticatorCallback.AddListener((bool status) =>
            {
                Debug.Log(logDecorator + "Platform signin status: " + status.ToString());
            });
            //SaveDataLoadCallback = new SaveDataLoadEvent();
            //SaveDataLoadCallback.AddListener((bool status, object data) =>
            //{
            //    Debug.Log(logDecorator + "Cloud save loaded and read into memory: " + status.ToString());
            //});
            SaveDataWriteCallback = new SaveDataWriteEvent();
            SaveDataWriteCallback.AddListener((bool status) =>
            {
                Debug.Log(logDecorator + "Game data write status: " + status.ToString());
            });

#if UNITY_ANDROID
            PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
#elif UNITY_IOS
            //Log into icloud/game center
#else
            throw new NotSupportedException(logDecorator + "INVALID PLATFORM, MUST BE ANDROID OR IOS");
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
            return currentSavedGameMetadata != null;
        }

        /// <summary>
        /// Opens the Social leaderboard UI
        /// </summary>
        public void OpenLeaderBoardUI()
        {
            Social.ShowLeaderboardUI();
        }

#region GOOGLE_PLAY_GAMES

        /// <summary>
        /// Returns the most recently loaded user game save data
        /// </summary>
        /// <returns></returns>
        public object GetCachedSaveGame()
        {
            return ByteArrayToObject(cache);
        }

        internal void ProcessAuthentication(SignInStatus status)
        {
            if (status == SignInStatus.Success)
            {
                //Activates unity social api integrations
                PlayGamesPlatform.Activate();

                // Continue with Play Games Services
                available = true;
                OpenSavedGame(cloudSaveFile);
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
            if (IsAvailable())
            {
                Social.ReportScore(score, "CgkI6NDuufMeEAIQAQ", (bool success) => {
                    callback(success);
                });
            } else
            {
                callback(false);
            }
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
            Debug.Log(logDecorator + "Game Data Load Status: " + status.ToString());

            if (status == SavedGameRequestStatus.Success)
            {
                currentSavedGameMetadata = game;

                ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
                savedGameClient.ReadBinaryData(game, OnSavedGameDataRead);
            }
            else
            {
                // handle error
                //SaveDataLoadCallback.Invoke(false, null);
            }
        }

        private void OnSavedGameDataRead(SavedGameRequestStatus status, byte[] data)
        {
            //Debug.Log("GPGS: Game Data Read Status: " + status.ToString());

            if (status == SavedGameRequestStatus.Success)
            {
                // handle processing the byte array data
                //SaveDataLoadCallback.Invoke(true, ByteArrayToObject(data));
                cache = data;
            }
            else
            {
                cache = null;
                // handle error
                Debug.Log(logDecorator + "Failed to read and return cloud save data: " + status.ToString());
                //SaveDataLoadCallback.Invoke(false, null);
            }
        }

        /// <summary>
        /// Saves an object to the cloud save
        /// </summary>
        /// <param name="savedData">A serializable object with some serializable fields to save</param>
        /// <param name="totalPlaytime">The total play time for this save so far</param>
        public void SaveGame(object savedData)
        {
            if (IsAvailable() && currentSavedGameMetadata != null)
            {
                ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

                SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
                builder = builder
                    .WithUpdatedPlayedTime(
                        currentSavedGameMetadata.TotalTimePlayed.Add(DateTime.Now.TimeOfDay.Subtract(sessionStart))
                    );

                SavedGameMetadataUpdate updatedMetadata = builder.Build();
                savedGameClient.CommitUpdate(currentSavedGameMetadata, updatedMetadata, ObjectToByteArray(savedData), OnSavedGameWritten);
            } else
            {
                Debug.Log(logDecorator + "Internal save data error");
            }
        }

        private void OnSavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
        {
            if (status == SavedGameRequestStatus.Success)
            {
                SaveDataWriteCallback.Invoke(true);
            }
            else
            {
                Debug.Log(logDecorator + "Writing save data to cloud failed: " + status.ToString());
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