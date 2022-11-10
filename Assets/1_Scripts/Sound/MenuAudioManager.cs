using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAudioManager : MonoBehaviour
{
    public static bool instanced = false;

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        if (!instanced)
        {
            instanced = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
