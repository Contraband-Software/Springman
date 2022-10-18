using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PlatformIntegrations
{
    using GooglePlayGames;
    using GooglePlayGames.BasicApi;

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
                Debug.Log("GPGS STATUS: " + status.ToString());
            });
        }
        public void Start()
        {
            PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
        }

        internal void ProcessAuthentication(SignInStatus status)
        {
            if (status == SignInStatus.Success)
            {
                // Continue with Play Games Services
                available = true;
            }
            else
            {
                // Disable your integration with Play Games Services or show a login button
                // to ask users to sign-in. Clicking it should call
                // PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication).
            }

            AuthenticatorCallback.Invoke(available);
        }

        /// <summary>
        /// To be attached to a button to prompt and allow the user to sign into GPG.
        /// </summary>
        public void TrySignIn()
        {
            PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);
        }

        /// <summary>
        /// Returns if google play game services is available currently (meaning the user is signed in and connected to the internet)
        /// </summary>
        /// <returns></returns>
        public bool isAvailable()
        {
            return available;
        }
    }
}