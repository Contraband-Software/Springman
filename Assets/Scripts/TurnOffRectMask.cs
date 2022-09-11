using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnOffRectMask : MonoBehaviour
{
    public RectMask2D rectMask;
    // Start is called before the first frame update
    void Start()
    {
        rectMask.enabled = false;
    }


}
