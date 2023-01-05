using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Architecture.Localisation
{
	public class LanguageOrganiser : MonoBehaviour
	{
		public string selectedLanguage;

		private LanguageToggle[] langButScripts;
		public string[] languages;

		private void Start()
		{
			languages = new string[transform.childCount];
			langButScripts = new LanguageToggle[transform.childCount];

			StartCoroutine(GetChildren());

		}

		public IEnumerator GetChildren()
		{
			for (int button = 0; button <= transform.childCount - 1; button++)
			{
				langButScripts[button] = transform.GetChild(button).GetComponent<LanguageToggle>();
				languages[button] = langButScripts[button].storedLanguage;

				yield return null;
			}
		}

		public void UnSelectPrevious()
		{
			if (selectedLanguage != "")
			{
				langButScripts[Array.IndexOf(languages, selectedLanguage)].UnSelect();
			}
		}

		public void HighlightCurrentLanguage()
		{
			UnSelectPrevious();

			selectedLanguage = LocalizationSystem.Instance.CurrentLanguage.ToString();

			int index = Array.IndexOf(languages, selectedLanguage);
			langButScripts[index].Select();
		}

	}
}