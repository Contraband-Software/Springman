using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace Architecture
{
	public class MusicToggle : MonoBehaviour
	{
		public TextMeshProUGUI musicOnText;
		public TextMeshProUGUI ToggleON;

		public TextMeshProUGUI musicOffText;
		public TextMeshProUGUI ToggleOFF;

		public Sprite ON;
		public Sprite OFF;

		JukeboxScript jukebox;

		private void SetMute(bool mute)
		{
			gameObject.GetComponent<Image>().sprite = (mute) ? OFF : ON;

			musicOnText.enabled = !mute;
			ToggleON.enabled = !mute;

			musicOffText.enabled = mute;
			ToggleOFF.enabled = mute;

			Managers.UserGameData.Instance.musicOn = !mute;

            jukebox.SetMute(mute);
		}

		public void Toggle()
		{
			if (gameObject.GetComponent<Image>().sprite == ON)
			{
				SetMute(true);
			}
			else
			{
				SetMute(false);
			}
		}

		private void Start()
		{
			jukebox = GameObject.FindGameObjectWithTag("GameMusicController").GetComponent<JukeboxScript>();

			SetMute(!Managers.UserGameData.Instance.musicOn);
		}
	}
}