using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public List<GameObject> poolObjects { get; private set; } = new List<GameObject>();

    void Awake()
    {
        for(int i = 0; i < transform.childCount; i++) {
            poolObjects.Add(transform.GetChild(i).gameObject);
        }
    }
}
