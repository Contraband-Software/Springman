using Architecture.Managers;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class AgeDialogue : MonoBehaviour
{
    public Sprite unselected;
    public Sprite selected;

    public Image currentSelection;
    public int selectedAgeLevel;

    private void Awake()
    {
        if (!UserGameData.Instance.EULA_Accepted)
        {
            Show();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void AgeButtonClicked(Image btnImage, int ageLevel)
    {
        if(currentSelection != null)
        {
            currentSelection.sprite = unselected;
            currentSelection = btnImage;
            btnImage.sprite = selected;
            selectedAgeLevel = ageLevel;
        }
        else
        {
            currentSelection = btnImage;
            btnImage.sprite = selected;
            selectedAgeLevel = ageLevel; 
        }
    }

    public void ConfirmAndClose()
    {
        //were also gonna accept the EULA here instead
        if(currentSelection != null)
        {
            UserGameData.Instance.EULA_Accepted = true;
            UserGameData.Instance.ageLevel = selectedAgeLevel;
            gameObject.SetActive(false);
            UserGameData.Instance.SaveGameData();
        }
    }
}
