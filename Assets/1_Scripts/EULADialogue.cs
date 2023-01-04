using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Architecture
{
    public class EULADialogue : MonoBehaviour
    {
        public MenuData md;

        private void Awake()
        {
            UserGameData.Instance.ShowEULA.AddListener(() =>
            {
                Show();
            });
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Accept()
        {
            gameObject.SetActive(false);
            md.SetEulaAccepted();
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
}