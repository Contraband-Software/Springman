using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrameButton : MonoBehaviour
{
    public FrameController frameController;
    public void SetSelfAsFirst()
    {
        frameController.SetFrameAsTop(gameObject.transform.parent.gameObject);
    }
}
