using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroLoopMusicController : MonoBehaviour
{
    [SerializeField] AudioSource IntroSource;
    [SerializeField] AudioSource LoopSource;

    private void Awake()
    {
        IntroSource.Play();
        Invoke("StartLoopSection", IntroSource.clip.length);

        StartCoroutine(StartLoopSection());
    }

    private IEnumerator StartLoopSection()
    {
        yield return new WaitForSeconds(IntroSource.clip.length - 0.52f);
        LoopSource.Play();
    }
}
