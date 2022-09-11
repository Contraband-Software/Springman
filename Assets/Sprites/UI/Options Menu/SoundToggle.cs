using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SoundToggle : MonoBehaviour
{
	public GameData gameData;
	public MenuData menuData;

	public TextMeshProUGUI soundsOnText;
	public TextMeshProUGUI ToggleON;

	public TextMeshProUGUI soundsOffText;
	public TextMeshProUGUI ToggleOFF;

	public Sprite ON;
	public Sprite OFF;

	Scene currentScene;

	public void Toggle()
	{
		FindScenesData();

		if (gameObject.GetComponent<Image>().sprite == ON)
		{
			gameObject.GetComponent<Image>().sprite = OFF;

			soundsOnText.enabled = false;
			ToggleON.enabled = false;

			soundsOffText.enabled = true;
			ToggleOFF.enabled = true;

			if (currentScene.name == "Main Menu")
			{
				menuData.soundsOn = false;
			}
			if (currentScene.name == "Game")
			{
				gameData.soundsOn = false;
			}
		}
		else
		{
			gameObject.GetComponent<Image>().sprite = ON;

			soundsOnText.enabled = true;
			ToggleON.enabled = true;

			soundsOffText.enabled = false;
			ToggleOFF.enabled = false;

			if (currentScene.name == "Main Menu")
			{
				menuData.soundsOn = true;
			}
			if (currentScene.name == "Game")
			{
				gameData.soundsOn = true;
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
		if (currentScene.name == "Game")
		{
			gameData = GameObject.Find("GameController").GetComponent<GameData>();
		}
	}

	private void Start()
	{
		currentScene = SceneManager.GetActiveScene();
		if (currentScene.name == "Main Menu")
		{
			menuData = GameObject.Find("MenuController").GetComponent<MenuData>();

			if (menuData.soundsOn == true)
			{
				gameObject.GetComponent<Image>().sprite = ON;

				soundsOnText.enabled = true;
				ToggleON.enabled = true;

				soundsOffText.enabled = false;
				ToggleOFF.enabled = false;
			}
			else
			{
				gameObject.GetComponent<Image>().sprite = OFF;

				soundsOnText.enabled = false;
				ToggleON.enabled = false;

				soundsOffText.enabled = true;
				ToggleOFF.enabled = true;
			}

		}
		if (currentScene.name == "Game")
		{
			gameData = GameObject.Find("GameController").GetComponent<GameData>();

			if (gameData.soundsOn == true)
			{
				gameObject.GetComponent<Image>().sprite = ON;

				soundsOnText.enabled = true;
				ToggleON.enabled = true;

				soundsOffText.enabled = false;
				ToggleOFF.enabled = false;
			}
			else
			{
				gameObject.GetComponent<Image>().sprite = OFF;

				soundsOnText.enabled = false;
				ToggleON.enabled = false;

				soundsOffText.enabled = true;
				ToggleOFF.enabled = true;
			}
		}
	}
}
