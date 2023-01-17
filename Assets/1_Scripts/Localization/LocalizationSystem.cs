using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Architecture.Localisation
{
    public class LocalizationSystem : Backend.AbstractSingleton<LocalizationSystem>
    {
        public enum Language
        {
            English,
            French,
            Spanish,
            Russian,
            German,
            Portuguese,
            Malay,
            Polish,
            Italian,
            Chinese,
            Turkish,
            Vietnamese,
            Ukrainian,
            Hindi,
            Indonesian,
            Arabic
        }

        #region INTERFACE
        [Header("Language Fonts")]
        [SerializeField] TMP_FontAsset Latin_Cyrillic;
        [SerializeField] TMP_FontAsset Latin;
        [SerializeField] TMP_FontAsset Vietnamese;
        [SerializeField] TMP_FontAsset Chinese;
        [SerializeField] TMP_FontAsset Hindi;
        [SerializeField] TMP_FontAsset Arabic;
        #endregion

        public TMP_FontAsset AppropriateFont { get; private set; }

        readonly Dictionary<string, TMP_FontAsset> appropriateFonts = new Dictionary<string, TMP_FontAsset>();

        public Language CurrentLanguage { get; set; } = Language.English;

        #region GAME_TRANSLATIONS
        private static Dictionary<string, string> localisedEN;
        private static Dictionary<string, string> localisedFR;
        private static Dictionary<string, string> localisedES;
        private static Dictionary<string, string> localisedRU;
        private static Dictionary<string, string> localisedGE;
        private static Dictionary<string, string> localisedPO;
        private static Dictionary<string, string> localisedMA;
        private static Dictionary<string, string> localisedPL;
        private static Dictionary<string, string> localisedIT;
        private static Dictionary<string, string> localisedCH;
        private static Dictionary<string, string> localisedTU;
        private static Dictionary<string, string> localisedVI;
        private static Dictionary<string, string> localisedUK;
        private static Dictionary<string, string> localisedHI;
        private static Dictionary<string, string> localisedIN;
        private static Dictionary<string, string> localisedAR;
        #endregion

        protected override void SingletonAwake()
        {
            CSVLoader csvLoader = new CSVLoader();
            csvLoader.LoadCSV();

#pragma warning disable S2696
            localisedEN = csvLoader.GetDictionaryValues("en");
            localisedFR = csvLoader.GetDictionaryValues("fr");
            localisedES = csvLoader.GetDictionaryValues("es");
            localisedRU = csvLoader.GetDictionaryValues("ru");
            localisedGE = csvLoader.GetDictionaryValues("ge");
            localisedPO = csvLoader.GetDictionaryValues("po");
            localisedMA = csvLoader.GetDictionaryValues("ma");
            localisedPL = csvLoader.GetDictionaryValues("pl");
            localisedIT = csvLoader.GetDictionaryValues("it");
            localisedCH = csvLoader.GetDictionaryValues("ch");
            localisedTU = csvLoader.GetDictionaryValues("tu");
            localisedVI = csvLoader.GetDictionaryValues("vi");
            localisedUK = csvLoader.GetDictionaryValues("uk");
            localisedHI = csvLoader.GetDictionaryValues("hi");
            localisedIN = csvLoader.GetDictionaryValues("in");
            localisedAR = csvLoader.GetDictionaryValues("ar");
#pragma warning restore S2696

            InitialiseAppropriateFonts();
        }

        public Language GetSystemLanguage()
        {
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Arabic:
                    return Language.Arabic;

                case SystemLanguage.Chinese:
                    return Language.Chinese;

                case SystemLanguage.French:
                    return Language.French;

                case SystemLanguage.German:
                    return Language.German;

                case SystemLanguage.Indonesian:
                    return Language.Indonesian;

                case SystemLanguage.Italian:
                    return Language.Italian;

                case SystemLanguage.Polish:
                    return Language.Polish;

                case SystemLanguage.Portuguese:
                    return Language.Portuguese;

                case SystemLanguage.Russian:
                    return Language.Russian;

                case SystemLanguage.Spanish:
                    return Language.Spanish;

                case SystemLanguage.Turkish:
                    return Language.Turkish;

                case SystemLanguage.Ukrainian:
                    return Language.Ukrainian;

                case SystemLanguage.Vietnamese:
                    return Language.Vietnamese;

                //DEFAULT TO ENGLISH
                default:
                    return Language.English;
            }
        }

        public string GetLocalisedValue(string key)
        {
            string value = key;

            switch (CurrentLanguage)
            {
                case Language.English:
                    localisedEN.TryGetValue(key, out value);
                    break;
                case Language.French:
                    localisedFR.TryGetValue(key, out value);
                    break;
                case Language.Spanish:
                    localisedES.TryGetValue(key, out value);
                    break;
                case Language.Russian:
                    localisedRU.TryGetValue(key, out value);
                    break;
                case Language.German:
                    localisedGE.TryGetValue(key, out value);
                    break;
                case Language.Portuguese:
                    localisedPO.TryGetValue(key, out value);
                    break;
                case Language.Malay:
                    localisedMA.TryGetValue(key, out value);
                    break;
                case Language.Polish:
                    localisedPL.TryGetValue(key, out value);
                    break;
                case Language.Italian:
                    localisedIT.TryGetValue(key, out value);
                    break;
                case Language.Chinese:
                    localisedCH.TryGetValue(key, out value);
                    break;
                case Language.Turkish:
                    localisedTU.TryGetValue(key, out value);
                    break;
                case Language.Vietnamese:
                    localisedVI.TryGetValue(key, out value);
                    break;
                case Language.Ukrainian:
                    localisedUK.TryGetValue(key, out value);
                    break;
                case Language.Hindi:
                    localisedHI.TryGetValue(key, out value);
                    break;
                case Language.Indonesian:
                    localisedIN.TryGetValue(key, out value);
                    break;
                case Language.Arabic:
                    localisedAR.TryGetValue(key, out value);
                    break;
            }

            return value;
        }

        void InitialiseAppropriateFonts()
        {
            appropriateFonts.Add("english", Latin);
            appropriateFonts.Add("french", Latin_Cyrillic);
            appropriateFonts.Add("spanish", Latin_Cyrillic);
            appropriateFonts.Add("russian", Latin_Cyrillic);
            appropriateFonts.Add("german", Latin_Cyrillic);
            appropriateFonts.Add("portugese", Latin_Cyrillic);
            appropriateFonts.Add("malay", Latin_Cyrillic);
            appropriateFonts.Add("polish", Latin_Cyrillic);
            appropriateFonts.Add("italian", Latin_Cyrillic);
            appropriateFonts.Add("chinese", Chinese);
            appropriateFonts.Add("turkish", Latin_Cyrillic);
            appropriateFonts.Add("vietnamese", Vietnamese);
            appropriateFonts.Add("ukrainian", Latin_Cyrillic);
            appropriateFonts.Add("hindi", Hindi);
            appropriateFonts.Add("indonesian", Latin_Cyrillic);
            appropriateFonts.Add("arabic", Arabic);
        }

        void FindAppropriateFont()
        {
            TMP_FontAsset value;
            appropriateFonts.TryGetValue(CurrentLanguage.ToString().ToLower(), out value);

            AppropriateFont = value;
        }

        public TMP_FontAsset GiveAppropriateFont(string language)
        {
            TMP_FontAsset value;
            appropriateFonts.TryGetValue(language, out value);

            return value;
        }

        public void ReLocalizeTexts()
        {
            Debug.Log("TRANSLATING TEXTS TO: " + CurrentLanguage.ToString());

            FindAppropriateFont();

            TextLocaliserUI[] textItems = FindObjectsOfType(typeof(TextLocaliserUI)) as TextLocaliserUI[];
            foreach (TextLocaliserUI text in textItems)
            {
                if (text.key == "localLang")
                {
                    text.language = CurrentLanguage.ToString();
                    text.ApplyFontForLangMenu();
                }
                else
                {
                    text.language = CurrentLanguage.ToString();
                    text.Localize();
                }
            }
        }
    }
}