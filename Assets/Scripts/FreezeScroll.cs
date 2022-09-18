using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FreezeScroll : MonoBehaviour
{
    public ScrollRect rect;
    public void OnClick()
    {
        rect.enabled = false;
    }
}
