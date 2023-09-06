using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgeButton : MonoBehaviour
{
    public AgeDialogue ageDialog;
    public Image thisImage;
    public int ageLevel;

    private void Start()
    {
        thisImage = GetComponent<Image>();
    }


    public void Click()
    {
        ageDialog.AgeButtonClicked(thisImage, ageLevel);
    }
}
