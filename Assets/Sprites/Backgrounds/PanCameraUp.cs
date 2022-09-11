using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanCameraUp : MonoBehaviour
{
    public float panSpeed = 1f;
    void Update()
    {
        transform.Translate(0f, panSpeed * Time.deltaTime, 0f);
    }
}
