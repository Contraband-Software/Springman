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
                Enum.TryParse(langOrganiser.selectedLanguage, out LocalizationSystem.Instance.CurrentLanguage);
                UserGameData.Instance.langIndex = Array.IndexOf(langOrganiser.languages, langOrganiser.selectedLanguage.ToLower());

                UserGameData.Instance.SaveGameData();
                LocalizationSystem.Instance.ReLocalizeTexts();
            }
        }
    }
}