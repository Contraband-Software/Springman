using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEditor;

namespace Architecture.Localisation
{
	public class LanguageToggle : MonoBehaviour
	{
		public string storedLanguage;

		public LanguageOrganiser langOrganiser;

		public TextMeshProUGUI langUnselected;
		public TextMeshProUGUI langSELECTED;

		public Sprite SELECTED;
		public Sprite UNSELECTED;

		Scene currentScene;

		public AudioSource click_sound;

		public void Awake()
		{
			GetAudioRules();
		}

		public void Update()
		{
			soundsOn = Managers.UserGameData.Instance.soundsOn;
		}

		private bool soundsOn;
		private void GetAudioRules()
		{
			soundsOn = Managers.UserGameData.Instance.soundsOn;
		}


		public void Toggle()
		{
			FindSceneData();

			if (gameObject.GetComponent<Image>().sprite == SELECTED)
			{
				UnSelect();
			}
			else
			{
				langOrganiser.UnSelectPrevious();

				Select();

				langOrganiser.selectedLanguage = storedLanguage;
			}
		}
		public void Select()
		{
			gameObject.GetComponent<Image>().sprite = SELECTED;
			langUnselected.enabled = false;
			langSELECTED.enabled = true;

			if (soundsOn)
			{
				click_sound.Play();
			}
		}

		public void UnSelect()
		{
			gameObject.GetComponent<Image>().sprite = UNSELECTED;
			langUnselected.enabled = true;
			langSELECTED.enabled = false;
		}

		private void Start()
		{
			click_sound = GameObject.Find("MenuAudio/PosClick").gameObject.GetComponent<AudioSource>();
			UnSelect();
		}

		private void FindSceneData()
		{
			langOrganiser = transform.parent.GetComponent<LanguageOrganiser>();
		}
	}
}