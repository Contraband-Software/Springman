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

	public void Toggle()
	{
		FindScenesData();

		if (gameObject.GetComponent<Image>().sprite == ON)
		{
			gameObject.GetComponent<Image>().sprite = OFF;

			musicOnText.enabled = false;
			ToggleON.enabled = false;

			musicOffText.enabled = true;
			ToggleOFF.enabled = true;

			if (currentScene.name == "Main Menu")
			{
				menuData.musicOn = false;
			}
			if (currentScene.name == "Game")
			{
				gameData.musicOn = false;
			}
		}
		else
		{
			gameObject.GetComponent<Image>().sprite = ON;

			musicOnText.enabled = true;
			ToggleON.enabled = true;

			musicOffText.enabled = false;
			ToggleOFF.enabled = false;

			if (currentScene.name == "Main Menu")
			{
				menuData.musicOn = true;
			}
			if (currentScene.name == "Game")
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

			if (menuData.musicOn == true)
			{
				gameObject.GetComponent<Image>().sprite = ON;

				musicOnText.enabled = true;
				ToggleON.enabled = true;

				musicOffText.enabled = false;
				ToggleOFF.enabled = false;
			}
			else
			{
				gameObject.GetComponent<Image>().sprite = OFF;

				musicOnText.enabled = false;
				ToggleON.enabled = false;

				musicOffText.enabled = true;
				ToggleOFF.enabled = true;
			}

		}
		if (currentScene.name == "Game")
		{
			gameData = GameObject.Find("GameController").GetComponent<GameData>();

			if (gameData.musicOn == true)
			{
				gameObject.GetComponent<Image>().sprite = ON;

				musicOnText.enabled = true;
				ToggleON.enabled = true;

				musicOffText.enabled = false;
				ToggleOFF.enabled = false;
			}
			else
			{
				gameObject.GetComponent<Image>().sprite = OFF;

				musicOnText.enabled = false;
				ToggleON.enabled = false;

				musicOffText.enabled = true;
				ToggleOFF.enabled = true;
			}
		}
	}
}
