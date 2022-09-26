using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ui_IgnoreAlpha : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		gameObject.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
	}
}
