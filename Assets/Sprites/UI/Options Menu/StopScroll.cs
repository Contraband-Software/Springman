using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StopScroll : MonoBehaviour
{
    public ScrollRect rectToStop;

    public void OnClick()
    {
        rectToStop.enabled = false;

        StartCoroutine(ReEnable());
    }

    public IEnumerator ReEnable()
    {
        yield return new WaitForSecondsRealtime(0.2f);

        rectToStop.enabled = true;
    }
}
