using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LanguageOrganiser : MonoBehaviour
{
	public string selectedLanguage;

    public GameData gameData;
    public MenuData menuData;

	private LanguageToggle[] langButScripts;
	public string[] languages;
	Scene currentScene;

    private void Start()
	{
		GetScenesData();
		languages = new string[transform.childCount];
		langButScripts = new LanguageToggle[transform.childCount];

		StartCoroutine(GetChildren());

	}

	public IEnumerator GetChildren()
	{
		for(int button = 0; button <= transform.childCount - 1; button++)
		{
			langButScripts[button] = transform.GetChild(button).GetComponent<LanguageToggle>();
			languages[button] = langButScripts[button].storedLanguage;

			yield return null;
		}
	}

	public void UnSelectPrevious()
	{
		if(selectedLanguage != "")
		{
			langButScripts[Array.IndexOf(languages, selectedLanguage)].UnSelect();
		}
	}

	private void GetScenesData()
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

	public void HighlightCurrentLanguage()
	{
		UnSelectPrevious();

		if (currentScene.name == "Main Menu")
		{
			selectedLanguage = menuData.currentLanguage;
		}
		if (currentScene.name == "Game")
		{
			selectedLanguage = gameData.currentLanguage;
		}

		int index = Array.IndexOf(languages, selectedLanguage);
		langButScripts[index].Select();
	}

}
