using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float TimeUntilDetonation;
    public float fadeOutVolTime_OPTIONAL;

    public AudioSource myAudio;


    public void DestructionCountdown()
    {
        LeanTween.delayedCall(TimeUntilDetonation, selfDestruct).setIgnoreTimeScale(true);
    }
    public void selfDestruct()
    {
        if(gameObject != null)
        {
            Destroy(gameObject);
        }
        

        //if gameObject not null, destroy
    }

    public void fadeVolumeOut()
    {
        LeanTween.value(gameObject, updateVolume, myAudio.volume, 0f, fadeOutVolTime_OPTIONAL).setIgnoreTimeScale(true);
    }
    public void updateVolume(float v)
    {
        myAudio.volume = v;
    }

}
