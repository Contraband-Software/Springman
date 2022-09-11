using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleTween : MonoBehaviour
{

    public LeanTweenType easeTypeOpen;
    public LeanTweenType easeTypeClose;

    private bool closing;

    public void OnClose()
    {
        LeanTween.scale(gameObject, new Vector3(0f, 0f, 0f), 0.2f).setEase(easeTypeClose).setIgnoreTimeScale(true);

        closing = true;
    }

    public void OnOpen()
    {

        GetComponent<RectTransform>().localScale = Vector3.zero;
        LeanTween.scale(gameObject, new Vector3(1f, 1f, 1f), 0.2f).setEase(easeTypeOpen).setIgnoreTimeScale(true);
    }

    private void Update()
    {
        if(closing && transform.localScale.x <= 0.0005)
        {
            Canvas parentCanvas = gameObject.transform.parent.GetComponent<Canvas>();
            parentCanvas.enabled = false;
            

            closing = false;
        }
    }
}
