using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Architecture.Localisation;
using Architecture.Managers;

public class HighScoreMainMenu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI hsText;


    private void Start()
    {
        AddOnScore();
    }


    public void AddOnScore()
    {
        hsText.gameObject.GetComponent<TextLocaliserUI>().Localize();
        string puretext = hsText.text;



        hsText.text = "";
        if (LocalizationSystem.Instance.CurrentLanguage.ToString().ToLower() == "arabic")
        {
            puretext = puretext.Remove(puretext.Length - 1);

            string scoreBackwards = UserGameData.Instance.allTimeHighscore.ToString();
            List<string> charList = new List<string>();
            for (int i = scoreBackwards.Length - 1; i >= 0; i--)
            {
                charList.Add(scoreBackwards[i].ToString());
            }
            string scoreForwards = string.Join("", charList);

            hsText.text = puretext + " " + scoreForwards;
        }
        else
        {
            hsText.text = puretext + " " + UserGameData.Instance.allTimeHighscore.ToString();
        }
    }
}
