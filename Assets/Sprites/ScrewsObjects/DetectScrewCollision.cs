using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class DetectScrewCollision : MonoBehaviour
{
    public ScrewScript parentScrewScript;
    public GameObject plus_one;

    public LeanTweenType easeTypeFade;
    public LeanTweenType easeTypeTravel;

    [Header("Fade Away Attributes")]
    public float fadeAwayTime;
    public float travelDistance;
    public float travelTime;

    private void Start()
    {
        plus_one = parentScrewScript.transform.GetChild(1).gameObject;
        plus_one.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Player")
        {
            parentScrewScript.gameObject.GetComponentInChildren<BoxCollider2D>().enabled = false;
            parentScrewScript.transform.SetParent(null);
            parentScrewScript.gameObject.LeanCancel();

            if (parentScrewScript.transform.GetChild(0).transform.childCount > 0)
            {
                GameObject screwObjects = parentScrewScript.transform.GetChild(0).gameObject;
                screwObjects.transform.localPosition += new Vector3(0.7f, 0f, 0f);
                
                for (int child = 0; child < screwObjects.transform.childCount; child++)
                {
                    GameObject childObject = screwObjects.transform.GetChild(child).gameObject;

                    var color = childObject.GetComponent<SpriteRenderer>().color;
                    var fadeOutColor = color;
                    fadeOutColor.a = 0;

                    LeanTween.alpha(childObject, 0f, fadeAwayTime).setIgnoreTimeScale(true).setEase(easeTypeFade); //fade out
                }
                parentScrewScript.transform.localScale *= 0.9f;
                plus_one.SetActive(true);
                LeanTween.alpha(plus_one, 0f, fadeAwayTime).setIgnoreTimeScale(true);

            }
            else
            {
                GameObject screwObjects = parentScrewScript.transform.GetChild(0).gameObject;
                screwObjects.transform.localPosition += new Vector3(0.7f, 0f, 0f);


                var color = gameObject.GetComponent<SpriteRenderer>().color;
                var fadeOutColor = color;
                fadeOutColor.a = 0;
                LeanTween.alpha(parentScrewScript.transform.gameObject, 0f, fadeAwayTime).setIgnoreTimeScale(true).setEase(easeTypeFade); //fade out

                parentScrewScript.transform.localScale *= 0.9f;
                plus_one.SetActive(true);
                LeanTween.alpha(plus_one, 0f, fadeAwayTime).setIgnoreTimeScale(true);
            }
            parentScrewScript.OnCollect();

            LeanTween.moveY(parentScrewScript.gameObject, parentScrewScript.gameObject.transform.position.y + travelDistance, travelTime).setIgnoreTimeScale(true).setOnComplete(DestroyOnComplete);

            /*
            LeanTween.cancel(thisScrew);
            thisScrew.GetComponentInChildren<BoxCollider2D>().enabled = false;
            */


        }
    }

    public void DestroyOnComplete()
    {
        Destroy(parentScrewScript.gameObject);
    }
}
