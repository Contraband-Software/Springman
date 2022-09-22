//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Events;

//public class argumentoverevents : MonoBehaviour
//{
//    public class objecty
//    {
//        public string name;
//    }
//    public class MyEvent : UnityEvent<objecty, string>
//    {

//    }

//    public MyEvent daEvent = new MyEvent();
//    public objecty theObject = new objecty();

//    private void Start()
//    {
//        daEvent.AddListener((objecty one, string two) =>
//        {
//            Debug.Log(one.ToString() + " " + two);
//        });

//        StartCoroutine(ExecuteAfterTime(10f));
//    }

//    IEnumerator ExecuteAfterTime(float time)
//    {
//        yield return new WaitForSeconds(time);

//        daEvent.Invoke(theObject, "hello");
//    }
//}
