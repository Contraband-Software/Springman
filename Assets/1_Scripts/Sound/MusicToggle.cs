using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MusicToggle : MonoBehaviour
{
	public GameData gameData;
	public MenuData menuData;

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

		switch (SceneManager.GetActiveScene().name) {
			case "Main Menu":
				menuData.musicOn = !mute;
				break;
			case "Game":
				gameData.musicOn = !mute;
				break;
		}

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

#region FINDSCENEDATA
        switch (SceneManager.GetActiveScene().name)
        {
            case "Main Menu":
                menuData = GameObject.Find("MenuController").GetComponent<MenuData>();
                SetMute(!menuData.musicOn);
                break;
            case "Game":
                gameData = GameObject.Find("GameController").GetComponent<GameData>();
                SetMute(!gameData.musicOn);
                break;
        }
#endregion
	}
}