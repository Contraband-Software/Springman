using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Architecture;

public class MenuController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Architecture.Localisation.LocalizationSystem.Instance.ReLocalizeTexts();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
