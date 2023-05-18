using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Architecture
{
    using Managers;

    public class EULADialogue : MonoBehaviour
    {
        private void Awake()
        {
            if (!UserGameData.Instance.EULA_Accepted)
            {
                Show();
            } else
            {
                gameObject.SetActive(false);
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Accept()
        {
            UserGameData.Instance.EULA_Accepted = true;
            gameObject.SetActive(false);
        }

        public void Deny()
        {
            UserGameData.Instance.EULA_Accepted = false;

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