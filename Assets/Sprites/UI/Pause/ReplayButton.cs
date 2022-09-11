using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReplayButton : MonoBehaviour
{
    public GameData gameData;

    public void OnClick()
    {
        gameData.SaveGameData();
        SceneManager.LoadScene("Game");
        Time.timeScale = 1;
    }
}
