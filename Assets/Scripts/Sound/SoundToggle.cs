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
    Scene currentScene;

    public TextMeshProUGUI soundsOnText;
	public TextMeshProUGUI ToggleON;

	public TextMeshProUGUI soundsOffText;
	public TextMeshProUGUI ToggleOFF;

	public Sprite ON;
	public Sprite OFF;

    private void SetMute(bool mute)
    {
        gameObject.GetComponent<Image>().sprite = (mute) ? OFF : ON;

        soundsOnText.enabled = !mute;
        ToggleON.enabled = !mute;

        soundsOffText.enabled = mute;
        ToggleOFF.enabled = mute;

        switch (currentScene.name)
        {
            case "Main Menu":
                menuData.soundsOn = !mute;
                break;
            case "Game":
                gameData.soundsOn = !mute;
                break;
        }
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
#region FINDSCENEDATA
        currentScene = SceneManager.GetActiveScene();
        switch (currentScene.name)
        {
            case "Main Menu":
                menuData = GameObject.Find("MenuController").GetComponent<MenuData>();
                SetMute(!menuData.soundsOn);
                break;
            case "Game":
                gameData = GameObject.Find("GameController").GetComponent<GameData>();
                SetMute(!gameData.soundsOn);
                break;
        }
#endregion
    }
}
