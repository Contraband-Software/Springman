using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicInit : MonoBehaviour
{
    [SerializeField] string Song;

    private void Awake()
    {
        GameObject.FindGameObjectWithTag("GameMusicController").GetComponent<JukeboxScript>().Play(Song);
    }
}
