using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroLoopMusicController : MonoBehaviour
{
    [SerializeField] AudioSource IntroSource;
    [SerializeField] AudioSource LoopSource;

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
