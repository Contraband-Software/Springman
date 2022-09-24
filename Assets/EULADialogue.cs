using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EULADialogue : MonoBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Accept()
    {
        gameObject.SetActive(false);
        //write EULA_Accepted to disk?
    }

    public void Deny()
    {
        //shut game, delete all gamedata, hard factory reset
    }
}
