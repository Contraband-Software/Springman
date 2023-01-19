using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class FilterFade : MonoBehaviour
{
    public CanvasGroup filter;
    public Image filterImage;
    public bool filterActive = false;

    private void Start()
    {
        if (!filterActive && filterImage != null)
        {
            filterImage.raycastTarget = false;
        }
    }

    public void FadeToBlack()
    {
        filterActive = true;
        LeanTween.alphaCanvas(filter, 1f, 0.2f).setIgnoreTimeScale(true);

        if(filterImage != null)
        {
            filterImage.raycastTarget = true;
        }
    }

    public void FadeToClear()
    {
        filterActive = false;
        LeanTween.alphaCanvas(filter, 0f, 0.2f).setIgnoreTimeScale(true).setOnComplete(disableClickBlock);
        
    }

    private void disableClickBlock()
    {
        if (filterImage != null)
        {
            filterImage.raycastTarget = false;
        }
    }

    public void FadeToClear_SceneLoad()
    {
        filterActive = false;
        LeanTween.alphaCanvas(filter, 0f, 0.5f).setIgnoreTimeScale(true);
    }
}
