using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        DirectoryInfo dataDir = new DirectoryInfo(Application.persistentDataPath);
        dataDir.Delete(true);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }
}
