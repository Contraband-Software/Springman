using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectScrewPC : MonoBehaviour
{
    [SerializeField] private Animator anim;
    private int silvers;
    private int golds;
    // Start is called before the first frame update
    void Start()
    {
        UpdateCurrency();
    }

    // Update is called once per frame
    void Update()
    {
        if(Architecture.Managers.UserGameData.Instance.silver != silvers || Architecture.Managers.UserGameData.Instance.gold != golds)
        {
            UpdateCurrency();
            anim.Play("pc_flash_money");
        }
    }

    private void UpdateCurrency()
    {
        silvers = Architecture.Managers.UserGameData.Instance.silver;
        golds = Architecture.Managers.UserGameData.Instance.gold;
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
