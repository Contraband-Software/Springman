using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//#if UNITY_ANALYTICS
namespace PlatformIntegrations
{
    using UnityEngine.Analytics;

    public class DataPrivacyHandler : MonoBehaviour
    {
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void OpenNewWindow(string url);
#endif

        Action<bool> successCallback;
        bool callbackSet = false;

        bool urlOpened = false;

        void OnFailure(string reason)
        {
            Debug.LogWarning(String.Format("Failed to get data privacy page URL: {0}", reason));

            if (callbackSet)
            {
                successCallback(false);
                callbackSet = false;
            }
        }

        void OnURLReceived(string url)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
        OpenNewWindow(url);
#else
            Application.OpenURL(url);
#endif
            urlOpened = true;

            if (callbackSet)
            {
                successCallback(true);
                callbackSet = false;
            }
        }

        void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus && urlOpened)
            {
                urlOpened = false;
                // Immediately refresh the remote config so new privacy settings can be enabled
                // as soon as possible if they have changed.
                RemoteSettings.ForceUpdate();
            }
        }
        public void OpenDataPrivacyURL()
        {
            DataPrivacy.FetchPrivacyUrl(OnURLReceived, OnFailure);
        }
        public void OpenDataPrivacyURL(Action<bool> callback)
        {
            callbackSet = true;
            successCallback = callback;
            DataPrivacy.FetchPrivacyUrl(OnURLReceived, OnFailure);
        }

        public bool GetURLOpened()
        {
            return urlOpened;
        }
    }
}
//#endif