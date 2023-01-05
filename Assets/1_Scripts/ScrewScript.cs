using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrewScript : MonoBehaviour
{
    private float topTarget;
    public LeanTweenType easingType;
    public AudioSource collectSound;
    private void Start()
    {
        topTarget = transform.position.y + 0.2f;
        LeanTween.moveY(gameObject, topTarget, 0.8f).setEase(easingType).setLoopPingPong().setIgnoreTimeScale(true);
    }

    public void OnCollect()
    {
        if (Architecture.Managers.UserGameData.Instance.soundsOn)
        {
            collectSound.Play();
        }
        
        if (name == "GoldScrew" || name == "GoldScrew(Clone)")
        {
            Architecture.Managers.UserGameData.Instance.gold++;
        }
        if(name == "SilverScrew" || name == "SilverScrew(Clone)")
        {
            Architecture.Managers.UserGameData.Instance.silver++;
        }
    }


}
