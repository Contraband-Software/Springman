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

	Scene currentScene;

	JukeboxScript jukebox;

    private void SetMute(bool mute)
    {
        gameObject.GetComponent<Image>().sprite = (mute) ? OFF : ON;

        musicOnText.enabled = !mute;
        ToggleON.enabled = !mute;

        musicOffText.enabled = mute;
        ToggleOFF.enabled = mute;

        jukebox.SetMute(mute);
    }

    public void Toggle()
	{
		FindScenesData();

		if (gameObject.GetComponent<Image>().sprite == ON)
		{
            SetMute(true);

            if (currentScene.name == "Main Menu")
			{
				menuData.musicOn = false;
			}
			else if (currentScene.name == "Game")
			{
				gameData.musicOn = false;
			}
		}
		else
		{
			SetMute(false);

            if (currentScene.name == "Main Menu")
			{
				menuData.musicOn = true;
			}
            else if (currentScene.name == "Game")
			{
				gameData.musicOn = true;
			}
		}
	}

	private void FindScenesData()
	{
		currentScene = SceneManager.GetActiveScene();
		if (currentScene.name == "Main Menu")
		{
			menuData = GameObject.Find("MenuController").GetComponent<MenuData>();
		}
        else if (currentScene.name == "Game")
		{
			gameData = GameObject.Find("GameController").GetComponent<GameData>();
		}
	}

	private void Start()
	{
        jukebox = GameObject.FindGameObjectWithTag("GameMusicController").GetComponent<JukeboxScript>();

        currentScene = SceneManager.GetActiveScene();

		if (currentScene.name == "Main Menu")
		{
			menuData = GameObject.Find("MenuController").GetComponent<MenuData>();
			SetMute(!menuData.musicOn);
		}
        else if (currentScene.name == "Game")
		{
			gameData = GameObject.Find("GameController").GetComponent<GameData>();
			SetMute(!gameData.musicOn);
		}
	}
}
