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

    public class SocialManager
    {
        #region EVENTS
        public class AuthenticationEvent : UnityEvent<bool> { }
        public AuthenticationEvent AuthenticatorCallback { get; private set; }
        public class SaveDataLoadEvent : UnityEvent<bool, object> { }
        public SaveDataLoadEvent SaveDataLoadCallback { get; private set; }
        public class SaveDataWriteEvent : UnityEvent<bool> { }
        public SaveDataWriteEvent SaveDataWriteCallback { get; private set; }
        #endregion

        #region CONFIG
        public static string cloudSaveFileName = "UserGameSave.dat";
        public const string logDecorator = Config.globalLogDecorator + "GPGS: ";
        public static ConflictResolutionStrategy conflictResolutionStrategy = ConflictResolutionStrategy.UseLastKnownGood;
        public static DataSource dataSource = DataSource.ReadCacheOrNetwork;
        #endregion

        #region STATE
        bool userSignedIn = false;
        bool saveFileLoaded = false;

        byte[] cache;
        readonly TimeSpan sessionStart;

        private object dataToSave = null;

        private bool attemptedManualAuth = false;
        #endregion

        public SocialManager()
        {
            sessionStart = DateTime.Now.TimeOfDay;

            cache = new byte[0];

            AuthenticatorCallback = new AuthenticationEvent();
            AuthenticatorCallback.AddListener((bool status) =>
            {
                Debug.Log(logDecorator + "[STATUS] Platform signin status: " + status.ToString());

                if(status == false && !attemptedManualAuth)
                {
                    attemptedManualAuth = true;
                    TrySignIn();
                }
            });
            SaveDataLoadCallback = new SaveDataLoadEvent();
            SaveDataLoadCallback.AddListener((bool status, object data) =>
            {
                Debug.Log(logDecorator + "[STATUS] Cloud save loaded and read into memory: " + status.ToString());
                saveFileLoaded = status;

            });
            SaveDataWriteCallback = new SaveDataWriteEvent();
            SaveDataWriteCallback.AddListener((bool status) =>
            {
                Debug.Log(logDecorator + "[STATUS] Game data write status: " + status.ToString());
            });

#if UNITY_ANDROID
            Debug.Log(logDecorator + "[STATUS] Attempting to automatically authenticate");
            PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
#elif UNITY_IOS
            //Log into icloud/game center
            throw new NotSupportedException(logDecorator + "[ERROR] INVALID PLATFORM, IOS NOT IMPLEMENTED");
#else
            throw new NotSupportedException(logDecorator + "[ERROR] INVALID PLATFORM, MUST BE ANDROID OR IOS");
#endif
        }

        #region PUBLIC_INTERFACE

        /// <summary>
        /// To be attached to a button to prompt and allow the user to sign into GPG.
        /// </summary>
        public void TrySignIn()
        {
#if UNITY_ANDROID
            Debug.Log(logDecorator + "[STATUS] Attempting to manually authenticate");
            PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);
#elif UNITY_IOS
            //Log into icloud/game center
            throw new NotSupportedException(logDecorator + "[ERROR] INVALID PLATFORM, IOS NOT IMPLEMENTED");
#endif
        }

        /// <summary>
        /// Returns if google play game services is userSignedIn currently (meaning the user is signed in and connected to the internet)
        /// </summary>
        /// <returns></returns>
        public bool IsSignedIn()
        {
            return userSignedIn;
        }

        /// <summary>
        /// Returns if a cloud file has been loaded successfully
        /// </summary>
        /// <returns></returns>
        public bool HasConnectedToCloud()
        {
            return saveFileLoaded;
        }

        /// <summary>
        /// Returns if a cloud save is loaded
        /// </summary>
        /// <returns></returns>
        public bool LoadedCloudSaveEmpty()
        {
            return (cache is null) || cache.Length == 0;
        }

        /// <summary>
        /// Opens the Social leaderboard UI
        /// </summary>
        public void OpenLeaderBoardUI()
        {
            Social.ShowLeaderboardUI();
        }

        /// <summary>
        /// Returns the most recently loaded user game save data
        /// </summary>
        /// <returns></returns>
        public object GetCachedSaveGame()
        {
            return ByteArrayToObject(cache);
        }

        /// <summary>
        /// Posts a score to the global game leaderboard
        /// </summary>
        /// <param name="score"></param>
        public void PostLeaderboardScore(string id, int score, Action<bool> callback)
        {
            if (IsSignedIn())
            {
                Social.ReportScore(score, id, (bool success) => {
                    callback(success);
                });
            } else
            {
                callback(false);
            }
        }

        /// <summary>
        /// Starts the process of loading a cloud save into the manager's cache.
        /// </summary>
        public void RetrieveCloudSave()
        {
            ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

            savedGameClient.OpenWithAutomaticConflictResolution(
                cloudSaveFileName,
                dataSource,
                conflictResolutionStrategy, 
                StartSavedGameRead
            );
        }

        /// <summary>
        /// Saves an object to the cloud save
        /// </summary>
        /// <param name="savedData">A serializable object with some serializable fields to save</param>
        /// <param name="totalPlaytime">The total play time for this save so far</param>
        public void WriteToCloudSave(object savedData)
        {
            if (IsSignedIn() && HasConnectedToCloud())
            {
                ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
                dataToSave = savedData;

                savedGameClient.OpenWithAutomaticConflictResolution(
                    cloudSaveFileName,
                    dataSource,
                    conflictResolutionStrategy,
                    StartSavedGameWrite
                );
            }
            else
            {
                Debug.Log(logDecorator + "[ERROR] The user is either not signed in or no cloud save loaded!");
            }
        }
        #endregion

        #region PRIVATE_INTERFACE
        internal void ProcessAuthentication(SignInStatus status)
        {
            if (status == SignInStatus.Success)
            {
                //Activates unity social api integrations
                PlayGamesPlatform.Activate();

                Debug.Log(logDecorator + "[STATUS] Sign in success");

                // Continue with Play Games Services
                userSignedIn = true;
                RetrieveCloudSave();
            }
            else
            {
                userSignedIn = false;
                Debug.Log("SIGN IN FAILED: " + status.ToString());
                // you can still manually authenticate the user with the manually authennticate function
            }

            AuthenticatorCallback.Invoke(userSignedIn);
        }

        private void StartSavedGameRead(SavedGameRequestStatus status, ISavedGameMetadata game)
        {
            Debug.Log(logDecorator + "[STATUS] Game Data Load Status: " + status.ToString());

            if (status == SavedGameRequestStatus.Success)
            {
                ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

                savedGameClient.ReadBinaryData(game, OnSavedGameRead);
            }
            else
            {
                // handle error
                SaveDataLoadCallback.Invoke(false, null);
            }
        }

        private void OnSavedGameRead(SavedGameRequestStatus status, byte[] data)
        {
            Debug.Log(logDecorator + "[STATUS] Game Data Read Status: " + status.ToString());

            if (status == SavedGameRequestStatus.Success)
            {
                // handle processing the byte array data
                SaveDataLoadCallback.Invoke(true, ByteArrayToObject(data));
                cache = data;
            }
            else
            {
                cache = null;
                // handle error
                Debug.Log(logDecorator + "[ERROR] Failed to read and return cloud save data: " + status.ToString());
                SaveDataLoadCallback.Invoke(false, null);
            }
        }

        private void StartSavedGameWrite(SavedGameRequestStatus status, ISavedGameMetadata currentSavedGameMetadata)
        {
            switch (status)
            {
                case SavedGameRequestStatus.Success:
                    ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
                    SavedGameMetadataUpdate.Builder builder = new();
                    builder = builder
                        .WithUpdatedPlayedTime(
                            currentSavedGameMetadata.TotalTimePlayed.Add(DateTime.Now.TimeOfDay.Subtract(sessionStart))
                        );

                    SavedGameMetadataUpdate updatedMetadata = builder.Build();
                    savedGameClient.CommitUpdate(currentSavedGameMetadata, updatedMetadata, ObjectToByteArray(dataToSave), OnSavedGameWrite);
                    break;
                default:
                    Debug.Log(logDecorator + "[ERROR] Writing save data to cloud failed: " + status.ToString());
                    SaveDataWriteCallback.Invoke(false);
                    break;
            }
        }

        private void OnSavedGameWrite(SavedGameRequestStatus status, ISavedGameMetadata game)
        {
            if (status == SavedGameRequestStatus.Success)
            {
                SaveDataWriteCallback.Invoke(true);
            }
            else
            {
                Debug.Log(logDecorator + "[ERROR] Cloud save write could not be finalised: " + status.ToString());
                SaveDataWriteCallback.Invoke(false);
            }
        }

        #region UTILS
        private static byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new();
            using (MemoryStream ms = new())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
        private static object ByteArrayToObject(byte[] arrBytes)
        {
            using (MemoryStream memStream = new())
            {
                BinaryFormatter binForm = new();

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
        #endregion

        #endregion
    }
}