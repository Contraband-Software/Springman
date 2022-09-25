using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectScrewPC : MonoBehaviour
{
    [SerializeField]private GameData gameData;
    [SerializeField] private Animator anim;
    private int silvers;
    private int golds;
    // Start is called before the first frame update
    void Start()
    {
        gameData = GameObject.Find("GameController").GetComponent<GameData>();
        UpdateCurrency();
    }

    // Update is called once per frame
    void Update()
    {
        if(gameData.silver != silvers || gameData.gold != golds)
        {
            UpdateCurrency();
            anim.Play("pc_flash_money");
        }
    }

    private void UpdateCurrency()
    {
        silvers = gameData.silver;
        golds = gameData.gold;
    }

    public void MoneyCollected()
    {
        anim.SetBool("moneyAnimPlaying", true);
    }
    public void MoneyCollectable()
    {
        anim.SetBool("moneyAnimPlaying", false);
    }
}
