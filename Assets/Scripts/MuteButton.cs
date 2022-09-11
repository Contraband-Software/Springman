using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class MuteButton : MonoBehaviour {
	public MenuData menuData;
	public GameData gameData;

	Scene currentScene;

	public Sprite UnMuted;
	public Sprite Muted;

	/*
	public void Toggle()
	{
		if (gameObject.GetComponent<Image>().sprite == UnMuted)
		{
			gameObject.GetComponent<Image>().sprite = Muted;

			if(currentScene.name == "Main Menu")
			{
				menuData.muted = true;
				menuData.SaveGameData();
			}
			if(currentScene.name == "Game")
			{
				gameData.muted = true;
				gameData.SaveGameData();
			}
		}
		else
		{
			gameObject.GetComponent<Image>().sprite = UnMuted;
			
			if (currentScene.name == "Main Menu")
			{
				menuData.muted = false;
				menuData.SaveGameData();
			}
			if (currentScene.name == "Game")
			{
				gameData.muted = false;
				gameData.SaveGameData();
			}
		}
	}


	private void Start()
	{
		currentScene = SceneManager.GetActiveScene();
		if (currentScene.name == "Main Menu")
		{
			menuData = GameObject.Find("MenuController").GetComponent<MenuData>();

			if(menuData.muted == true)
			{
				gameObject.GetComponent<Image>().sprite = Muted;
			}
			else
			{
				gameObject.GetComponent<Image>().sprite = UnMuted;
			}
		}
		if (currentScene.name == "Game")
		{
			gameData = GameObject.Find("GameController").GetComponent<GameData>();

			if (gameData.muted == true)
			{
				gameObject.GetComponent<Image>().sprite = Muted;
			}
			else
			{
				gameObject.GetComponent<Image>().sprite = UnMuted;
			}
		}
	}
	private void Update()
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
	*/
}
