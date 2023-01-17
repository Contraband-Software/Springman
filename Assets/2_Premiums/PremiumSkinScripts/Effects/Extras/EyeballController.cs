using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeballController : MonoBehaviour
{
    public GameObject eye;
    public GameObject eyeSocket;
    private Rigidbody2D playerRb;
    public float xLowBound;
    public float xHighBound;
    public float yLowBound;
    public float yHighBound;

    public List<GameObject> enemiesActive = new List<GameObject>();

    private PlayerController playerCon;

    private float eyeBallRadius = 0.13f;

    private Vector3 desiredEyePos;

    // Start is called before the first frame update
    void Start()
    {
        playerCon = transform.root.gameObject.GetComponent<PlayerController>();
        playerRb = playerCon.rb;
    }

    // Update is called once per frame
    void Update()
    {
        TargetOrIdle();
    }

    private void TargetOrIdle()
    {
        if (Architecture.Managers.GamePlay.GetReference().EnemiesActive.Count > 0)
        {
            SnapToFirstEnemy();
        }
        else
        {
            EyeIdle();
        }
    }


    private void SnapToFirstEnemy()
    {
        Vector3 target = Architecture.Managers.GamePlay.GetReference().EnemiesActive[0].transform.position;
        Vector3 centrePosition = eyeSocket.transform.position;
        Vector3 difference = (target - eyeSocket.transform.position).normalized * eyeBallRadius;
        
        
        if(difference.x > xHighBound)
        {
            difference.x = xHighBound;
        }
        if(difference.x < xLowBound)
        {
            difference.x = xLowBound;
        }
        if(difference.y > yHighBound)
        {
            difference.y = yHighBound;
        }
        if(difference.y < yLowBound)
        {
            difference.y = yLowBound;
        }
        
        eye.transform.localPosition = difference;
    }

    private void EyeIdle()
    {
        desiredEyePos = eye.transform.localPosition;
        if (playerRb.velocity.x > 0)
        {
            desiredEyePos.x = xHighBound;
            desiredEyePos.y = yLowBound;
        }
        if (playerRb.velocity.x < 0)
        {
            desiredEyePos.x = xLowBound;
            desiredEyePos.y = yLowBound;
        }
        eye.transform.localPosition = Vector3.Lerp(eye.transform.localPosition, desiredEyePos, 0.07f);
    }
}
