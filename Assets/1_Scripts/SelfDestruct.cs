using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float TimeUntilDetonation;
    public float fadeOutVolTime_OPTIONAL;

    public AudioSource myAudio;

    private bool destroyed;

        
    public void DestructionCountdown()
    {
        //print("called on:  " + gameObject.name);
        //LeanTween.delayedCall(TimeUntilDetonation, selfDestruct).setIgnoreTimeScale(true);
        //print("called on:  " + gameObject.name);
        StartCoroutine(DestructionCountdownIE(TimeUntilDetonation));
    }

    private IEnumerator DestructionCountdownIE(float time)
    {
        // Wait for the specified amount of real time
        yield return new WaitForSecondsRealtime(time);

        // Call selfDestruct on this object
        selfDestruct();
    }

    public void selfDestruct()
    {
        if (!destroyed && gameObject != null)
        {
            destroyed = true;
            //print("DESTROYED BY SELF DESTRUCT: " + gameObject.GetInstanceID().ToString());
            
            Destroy(gameObject);
        }


        //if gameObject not null, destroy
    }

    public void OnDestroy()
    {
        //print("destroyed: " + gameObject.name + " : " + gameObject.GetInstanceID().ToString());
        LeanTween.cancel(gameObject);
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
