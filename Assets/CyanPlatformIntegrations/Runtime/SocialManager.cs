using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PlatformIntegrations
{
    using GooglePlayGames;
    using GooglePlayGames.BasicApi;
    using GooglePlayGames.BasicApi.SavedGame;

    public class SocialManager : MonoBehaviour
    {
        public class AuthenticationEvent : UnityEvent<bool>
        {

        }
        public AuthenticationEvent AuthenticatorCallback;

        bool available = false;

        private void Awake()
        {
            AuthenticatorCallback = new AuthenticationEvent();
            AuthenticatorCallback.AddListener((bool status) =>
            {
                Debug.Log("GPGS: Platform signin STATUS: " + status.ToString());
            });
        }
        public void Start()
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
        public bool isAvailable()
        {
            return available;
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

        //SAVE GAMES
        void OpenSavedGame(string filename)
        {
            ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
            savedGameClient.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpened);
        }
        public void OnSavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata game)
        {
            Debug.Log("GPGS: Game Data Load Status: " + status.ToString());
            if (status == SavedGameRequestStatus.Success)
            {
                // handle reading or writing of saved game.
            }
            else
            {
                // handle error
            }
        }

        public void LoadSaveGame()
        {
            Debug.Log("GPGS: saved game loaded");
        }

#endregion
    }
}