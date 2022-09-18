using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;

public class JukeboxScript : MonoBehaviour
{
    [SerializeField] List<IntroLoopMusicController> musicControllers = new List<IntroLoopMusicController>();

    public void SetMute(bool mute)
    {
        for (int i = 0; i < musicControllers.Count; i++)
        {
            musicControllers[i].SetMute(mute);
        }
    }

    public void StopAll()
    {
        for (int i = 0; i < musicControllers.Count; i++)
        {
            musicControllers[i].Stop();
        }
    }

    public void Play(int index)
    {
        StopAll();

        musicControllers[index].Play();
    }
}
