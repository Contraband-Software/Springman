using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace Architecture.Audio
{
    public class SoundToggle : MonoBehaviour
    {

        public TextMeshProUGUI soundsOnText;
        public TextMeshProUGUI ToggleON;

        public TextMeshProUGUI soundsOffText;
        public TextMeshProUGUI ToggleOFF;

        public Sprite ON;
        public Sprite OFF;

        private void SetMute(bool mute)
        {
            gameObject.GetComponent<Image>().sprite = (mute) ? OFF : ON;

            soundsOnText.enabled = !mute;
            ToggleON.enabled = !mute;

            soundsOffText.enabled = mute;
            ToggleOFF.enabled = mute;

            Managers.UserGameData.Instance.soundsOn = !mute;
        }

        public void Toggle()
        {
            if (gameObject.GetComponent<Image>().sprite == ON)
            {
                SetMute(true);
            }
            else
            {
                SetMute(false);
            }
        }

        private void Start()
        {
            SetMute(!Managers.UserGameData.Instance.soundsOn);
        }
    }
}