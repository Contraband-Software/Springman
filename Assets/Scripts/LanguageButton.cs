using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageButton : MonoBehaviour
{
    public Canvas LanguagesMenu;

    private void Awake()
    {
        LanguagesMenu.enabled = false;
    }

    public void OnClick()
    {
        LanguagesMenu.gameObject.SetActive(true);
        LanguagesMenu.enabled = true;
        LanguagesMenu.gameObject.transform.GetChild(1).GetComponent<ScaleTween>().OnOpen();

        GameObject.Find("ButtonGroup").GetComponent<LanguageOrganiser>().HighlightCurrentLanguage();
    }
}
