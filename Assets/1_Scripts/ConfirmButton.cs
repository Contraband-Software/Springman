using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

using Architecture.Managers;

namespace Architecture.Localisation
{
    public class ConfirmButton : MonoBehaviour
    {
        public LanguageOrganiser langOrganiser;

        Scene currentScene;

        public void OnClick()
        {
            if (langOrganiser.selectedLanguage != "")
            {
                LocalizationSystem.Language lang = LocalizationSystem.Language.English;
                Enum.TryParse(langOrganiser.selectedLanguage, out lang);
                LocalizationSystem.Instance.CurrentLanguage = lang;
                UserGameData.Instance.langIndex = Array.IndexOf(langOrganiser.languages, langOrganiser.selectedLanguage);

                UserGameData.Instance.SaveGameData();
                LocalizationSystem.Instance.ReLocalizeTexts();
            }
        }
    }
}