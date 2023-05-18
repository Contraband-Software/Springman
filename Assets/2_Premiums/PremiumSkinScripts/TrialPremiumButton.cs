using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrialPremiumButton : MonoBehaviour
{
    public void LoadTrialLevel()
    {
        SceneManager.LoadScene("Trial");
    }
}
