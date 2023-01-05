using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Architecture.Localisation
{
    [RequireComponent(typeof(TextMeshProUGUI))]

    public class TextLocaliserUI : MonoBehaviour
    {
        TextMeshProUGUI textField;
        public string language;

        FontDetails prevFontDetails;
        public int defaultStyle;
        float defaultMaxSize;
        bool defaultsSet;

        public string key;
        public string Optional2ndKey;

        public void Localize()
        {
            textField = GetComponent<TextMeshProUGUI>();

            if (defaultStyle == 0 && defaultsSet == false)
            {
                defaultStyle = (int)textField.fontStyle;
                defaultMaxSize = textField.fontSizeMax;

                defaultsSet = true;
            }

            if (language == "arabic")
            {
                textField.isRightToLeftText = true;
            }
            if (language != "arabic")
            {
                textField.isRightToLeftText = false;
            }

            ApplyFont();

            if (key == "errorPara")
            {
                string value1 = LocalizationSystem.Instance.GetLocalisedValue(key);
                value1 = value1.TrimStart(' ', '"'); value1 = value1.Replace("\"", "");

                textField = GetComponent<TextMeshProUGUI>();
                string value2 = LocalizationSystem.Instance.GetLocalisedValue(Optional2ndKey);
                value2 = value2.TrimStart(' ', '"'); value2 = value2.Replace("\"", "");

                string ValueMerged = value1 + "\n\n" + value2;

                textField.text = ValueMerged;
            }
            else
            {
                //print(key);
                string value = LocalizationSystem.Instance.GetLocalisedValue(key);
                //print("GameObject Name: " + gameObject.name + " localizing key: " + key);
                value = value.TrimStart(' ', '"'); value = value.Replace("\"", "");
                //print("localized");
                textField.text = value;
            }


        }

        public void ApplyFont()
        {
            prevFontDetails = new FontDetails(textField.fontMaterial.GetColor("_FaceColor"), textField.fontMaterial.GetFloat("_OutlineSoftness"),
                textField.fontMaterial.GetFloat("_FaceDilate"), textField.fontMaterial.GetColor("_OutlineColor"), (textField.fontMaterial.GetFloat("_OutlineWidth")));


            textField.font = LocalizationSystem.Instance.AppropriateFont;
            language = LocalizationSystem.Instance.CurrentLanguage.ToString().ToLower();

            ApplyFontDetails();
        }
        public void ApplyFontForLangMenu()
        {
            textField = GetComponent<TextMeshProUGUI>();
            if (defaultStyle == 0 && defaultsSet == false)
            {
                defaultStyle = (int)textField.fontStyle;
                defaultMaxSize = textField.fontSizeMax;

                defaultsSet = true;
            }

            prevFontDetails = new FontDetails(textField.fontMaterial.GetColor("_FaceColor"), textField.fontMaterial.GetFloat("_OutlineSoftness"),
                textField.fontMaterial.GetFloat("_FaceDilate"), textField.fontMaterial.GetColor("_OutlineColor"), (textField.fontMaterial.GetFloat("_OutlineWidth")));


            textField.font = LocalizationSystem.Instance.GiveAppropriateFont(transform.parent.gameObject.GetComponent<LanguageToggle>().storedLanguage);
            language = LocalizationSystem.Instance.CurrentLanguage.ToString().ToLower();

            ApplyFontDetails();
        }


        public void ApplyFontDetails()
        {
            DefaultApply();
            if (language.ToLower() == "chinese" && key != "localLang")
            {
                textField.fontStyle = FontStyles.Normal;
                textField.fontSizeMax = defaultMaxSize * 1.3f;
            }
            if (key == "localLang" && transform.parent.gameObject.GetComponent<LanguageToggle>().storedLanguage == "hindi")
            {
                textField.fontMaterial.SetFloat("_OutlineWidth", 0.3f);
            }
            if (language.ToLower() == "arabic" && gameObject.name == "TutorialText")
            {
                textField.fontStyle = FontStyles.Bold;
            }
            else
            {
                textField.fontStyle = (FontStyles)defaultStyle;
                textField.fontSizeMax = defaultMaxSize;
            }
        }

        public void DefaultApply()
        {
            textField.fontMaterial.SetColor("_FaceColor", prevFontDetails.faceColor);
            textField.fontMaterial.SetFloat("_OutlineSoftness", prevFontDetails.softness);
            textField.fontMaterial.SetFloat("_FaceDilate", prevFontDetails.faceDilate);
            textField.fontMaterial.SetColor("_OutlineColor", prevFontDetails.outlineColor);
            textField.fontMaterial.SetFloat("_OutlineWidth", prevFontDetails.outlineWidth);
        }
    }
}