using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsButton : MonoBehaviour
{
    public Canvas OptionsMenu;

    private void Awake()
    {
        OptionsMenu.enabled = false;
    }

    public void OnClick()
    {
        OptionsMenu.gameObject.SetActive(true);
        OptionsMenu.enabled = true;
        OptionsMenu.gameObject.transform.GetChild(1).GetComponent<ScaleTween>().OnOpen();
    }
}
