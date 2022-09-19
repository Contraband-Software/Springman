using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEditor;

public class LanguageToggle : MonoBehaviour
{
	public string storedLanguage;

	public LanguageOrganiser langOrganiser;

	public GameData gameData;
	public MenuData menuData;

	public TextMeshProUGUI langUnselected;
	public TextMeshProUGUI langSELECTED;

	public Sprite SELECTED;
	public Sprite UNSELECTED;

	Scene currentScene;

	public AudioSource click_sound;

	[Header("Menu/Game Data")]
	public MenuData menuData_Fetched;
	public GameData gameData_Fetched;

	public void Awake()
	{
		GetAudioRules();
	}

	public void Update()
	{
		if (menuData_Fetched != null)
		{
			soundsOn = menuData_Fetched.soundsOn;
		}
		if (gameData_Fetched != null)
		{
			soundsOn = gameData_Fetched.soundsOn;
		}
	}

	private bool soundsOn;
	private void GetAudioRules()
	{
		GameObject mainMenuObj = GameObject.Find("MenuController");
		GameObject gameObj = GameObject.Find("GameController");

		if (mainMenuObj != null)
		{
			menuData_Fetched = mainMenuObj.GetComponent<MenuData>();
			soundsOn = menuData_Fetched.soundsOn;
		}
		if (gameObj != null)
		{
			gameData_Fetched = gameObj.GetComponent<GameData>();
			soundsOn = gameData_Fetched.soundsOn;
		}
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

			if (currentScene.name == "Main Menu")
			{
				langOrganiser.selectedLanguage = storedLanguage;
			}
			if (currentScene.name == "Game")
			{
				langOrganiser.selectedLanguage = storedLanguage;
			}
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
}
