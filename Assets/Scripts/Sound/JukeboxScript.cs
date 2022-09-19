using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static UnityEngine.UI.Image;

public class JukeboxScript : MonoBehaviour
{
    [Serializable] public struct Song
    {
        public string name;
        public IntroLoopMusicController Controller;
    }
    [SerializeField] List<Song> musicControllers = new List<Song>();

    private Song GetSongByName(string name)
    {
        for (int i = 0; i < musicControllers.Count; i++)
        {
            if (name == musicControllers[i].name)
            {
                return musicControllers[i];
            }
        }

        throw new ArgumentException("Song name does not exist", nameof(name));
    }

    public IntroLoopMusicController GetSongController(string song)
    {
        return GetSongByName(song).Controller;
    }

    public void SetMute(bool mute)
    {
        for (int i = 0; i < musicControllers.Count; i++)
        {
            musicControllers[i].Controller.SetMute(mute);
        }
    }

    public void StopAll()
    {
        for (int i = 0; i < musicControllers.Count; i++)
        {
            musicControllers[i].Controller.Stop();
        }
    }

    public void Play(string song)
    {
        StopAll();

        GetSongByName(song).Controller.Play();
    }
}
