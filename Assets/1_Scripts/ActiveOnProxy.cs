using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveOnProxy : MonoBehaviour
{

    public GameObject player;
    public Animator animator;
    public PlayerController pController;

    public AudioSource activationSound;
    public bool playedActivation = false;

    float proximity = 2f;
    bool isVisible;
    Bounds bounds;
    // Start is called before the first frame update
    void Start()
    {
        isVisible = false;

        //player = GameObject.Find("Player");
        //gamedata = GameObject.Find("GameController").GetComponent<GameData>();
        //pController = player.GetComponent<PlayerController>();
        animator = GetComponent<Animator>();

        bounds = GetComponent<BoxCollider2D>().bounds;

        pController.revive_Reassign += ReassignPCon;
    }

    private void ReassignPCon(PlayerController pCon)
    {
        pController = pCon;
        player = pController.gameObject;
        pController.revive_Reassign += ReassignPCon;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateProximity();
    }

    void CalculateProximity()
    {
        float deltX = Mathf.Max(transform.position.x, player.transform.position.x) - Mathf.Min(transform.position.x, player.transform.position.x);
        float deltY = Mathf.Max(transform.position.y, player.transform.position.y) - Mathf.Min(transform.position.y, player.transform.position.y);

        float hyp = Mathf.Sqrt((deltX * deltX) + (deltY * deltY));

        if(hyp < proximity && isVisible && pController.dir == PlayerController.Direction.Falling 
            && player.transform.position.y > transform.position.y - bounds.extents.y)
        {
            animator.Play("Activate");
            if (!playedActivation)
            {
                if (Architecture.Managers.UserGameData.Instance.soundsOn)
                {
                    activationSound.Play();
                }
                
                playedActivation = true;
            }
        }
    }

    private void OnBecameInvisible()
    {
        isVisible = false;
    }
    private void OnBecameVisible()
    {
        isVisible = true;
    }
}
