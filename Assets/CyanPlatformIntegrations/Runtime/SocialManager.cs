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
                Debug.Log("Platform signin STATUS: " + status.ToString());
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

        ISavedGameClient savedGameClient;

        internal void ProcessAuthentication(SignInStatus status)
        {
            if (status == SignInStatus.Success)
            {
                // Continue with Play Games Services
                available = true;
                ShowSaveGameSelectUI();
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

        //SELECT A SAVE GAME
        /// <summary>
        /// Prompt the user to select a cloud save game
        /// </summary>
        public void ShowSaveGameSelectUI()
        {
            uint maxNumToDisplay = 2;
            bool allowCreateNew = true;
            bool allowDelete = true;

            savedGameClient = PlayGamesPlatform.Instance.SavedGame;
            savedGameClient.ShowSelectSavedGameUI("Select or create a save-game",
                maxNumToDisplay,
                allowCreateNew,
                allowDelete,
                OnSavedGameSelected);
        }
        public void OnSavedGameSelected(SelectUIStatus status, ISavedGameMetadata game)
        {
            if (status == SelectUIStatus.SavedGameSelected)
            {
                // handle selected game save
            }
            else
            {
                // handle cancel or error
            }
            Debug.Log("GPGS saved game selected: " + status.ToString());
        }

        public void LoadSaveGame()
        {
            Debug.Log("GPGS saved game loaded");
        }

#endregion
    }
}