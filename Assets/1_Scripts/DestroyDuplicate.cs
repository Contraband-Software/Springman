using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyDuplicate : MonoBehaviour
{
    public bool destroyOld = false;
    public bool destroyNew = false;
    public float timeAlive = 0f;
    private static GameObject objectIdentifier;
    public bool spawnedByScene = false;

    public void Awake()
    {

        LeanTween.cancelAll();

        GameObject otherCopy = GameObject.Find(gameObject.name);

        if (destroyOld)
        {
            if (otherCopy != gameObject)
            {
                float otherTime = otherCopy.GetComponent<DestroyDuplicate>().timeAlive;


                if (otherTime > timeAlive)
                {

                    Destroy(otherCopy);
                }
            }
        }
        if (destroyNew)
        {
            if(objectIdentifier == null)
            {
                objectIdentifier = gameObject;
                spawnedByScene = true;
            }
            else
            {
                spawnedByScene = false;
            }

            if (!spawnedByScene)
            {
                Destroy(gameObject);
            }
        }
        
    }

    public void Update()
    {
        timeAlive += Time.deltaTime;
    }
}
