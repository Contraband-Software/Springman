using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Architecture.Audio
{
    public class IntroLoopMusicController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] AudioSource IntroSource;
        [SerializeField] AudioSource LoopSource;

        [Header("Settings")]
        [SerializeField, Range(0, 1)] float Volume;

        private bool playing = false;
        private bool switched = false;

        public void SetMute(bool mute)
        {
            IntroSource.mute = mute;
            LoopSource.mute = mute;
        }

        public void Play()
        {
            IntroSource.Play();

            playing = true;
        }

        public void Stop()
        {
            IntroSource.Stop();
            LoopSource.Stop();
            playing = false;
            switched = false;
        }

        public void SetVolume(float volume)
        {
            IntroSource.volume = volume;
            LoopSource.volume = volume;
        }

        private void Awake()
        {
            SetVolume(Volume);
        }

        //Using fixed update avoids the delay between the intro section ending and the loop section playing
        //this can be replaced with the normal update function if there are any problems
        private void FixedUpdate()
        {
            if (playing)
            {
                if (IntroSource.isPlaying == false && switched == false)
                {
                    switched = true;
                    LoopSource.Play();
                }
            }
        }
    }
}