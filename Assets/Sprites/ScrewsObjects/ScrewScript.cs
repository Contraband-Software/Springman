using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrewScript : MonoBehaviour
{
    private float topTarget;
    public LeanTweenType easingType;
    public GameData gameData;
    public AudioSource collectSound;
    private void Start()
    {
        gameData = GameObject.Find("GameController").GetComponent<GameData>();
        topTarget = transform.position.y + 0.2f;
        LeanTween.moveY(gameObject, topTarget, 0.8f).setEase(easingType).setLoopPingPong().setIgnoreTimeScale(true);
    }

    public void OnCollect()
    {
        if (gameData.soundsOn)
        {
            collectSound.Play();
        }
        
        if (name == "GoldScrew" || name == "GoldScrew(Clone)")
        {
            gameData.gold++;
        }
        if(name == "SilverScrew" || name == "SilverScrew(Clone)")
        {
            gameData.silver++;
        }
    }


}
