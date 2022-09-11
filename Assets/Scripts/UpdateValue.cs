using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEditor;
using System;

public class UpdateValue : MonoBehaviour
{
    public GameData gameData;
    public MenuData menuData;

    public TextMeshProUGUI thisText;

    Scene currentScene;

    public enum ValueType { Silver, Gold};
    public ValueType valueType;

    private const int BaseDCV = 1;
    public int DecIncValue;

    // Start is called before the first frame update
    void Start()
    {
        thisText = gameObject.GetComponent<TextMeshProUGUI>();
        currentScene = SceneManager.GetActiveScene();

        GameObject menuObj = GameObject.Find("MenuController");
        GameObject gameObj = GameObject.Find("GameController");

        if (menuObj!= null)
        {
            menuData = GameObject.Find("MenuController").GetComponent<MenuData>();
            UpdateCurrency(gameData, menuData, valueType);
        }
        if (gameObj != null)
        {
            gameData = GameObject.Find("GameController").GetComponent<GameData>();

            PauseButton pauseButton = GameObject.Find("Canvas/PauseButton").GetComponent<PauseButton>();
            pauseButton.OnPause += OnPauseOpen;

            ResumeButton resumeButton = GameObject.Find("PauseMenu/Canvas/ResumeButton").GetComponent<ResumeButton>();
            resumeButton.OnResume += OnResumeClose;

            PlayerController playerController = GameObject.Find("Player").GetComponent<PlayerController>();
            playerController.OnDeath += OnDeathOpen;
        }
        UpdateCurrency(gameData, menuData, valueType);
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if(currentScene.name != "Game")
        {
            UpdateCurrency(gameData, menuData, valueType);
        }
        */

    }

    public void UpdateCurrency(GameData gameData, MenuData menuData, ValueType type)
    {
        if(valueType == ValueType.Silver)
        {

            if(gameData != null) { thisText.text = gameData.silver.ToString(); }
            if(menuData != null) { thisText.text = menuData.silver.ToString(); }
        }
        else
        {
            if(gameData != null) { thisText.text = gameData.gold.ToString(); }
            if(menuData != null) { thisText.text = menuData.gold.ToString(); }
        }
        thisText.text = Format(Convert.ToInt32(thisText.text));
    }

    //This money counter thing will start AFTER the value of the currency has changed in Game/MenuData
    public void CurrencyChangeDetails(ValueType targetValType) //ValType is the type of objects youll want to change
    {
        StopAllCoroutines();//resets if already going

        Operation operation;

        int inital_CurrencyOfValType;
        int target_CurrencyOfValType = 0;

        if(targetValType == valueType)
        {
            inital_CurrencyOfValType = DeFormat(thisText.text);     //Future: parse this into basic numbers as reformatter will change to eg: 1,234 or 1.2K

            if(currentScene.name == "Game")
            {
                if(targetValType == ValueType.Silver) { target_CurrencyOfValType = gameData.silver; } //finding 
                if(targetValType == ValueType.Gold) { target_CurrencyOfValType = gameData.gold; }
            }
            else
            {
                if (targetValType == ValueType.Silver) { target_CurrencyOfValType = menuData.silver; }
                if (targetValType == ValueType.Gold) { target_CurrencyOfValType = menuData.gold; }
            }

            //decinc decided here
            AdjustDecIncVal(inital_CurrencyOfValType, target_CurrencyOfValType);

            if(inital_CurrencyOfValType < target_CurrencyOfValType) //selects whether to choose decrement until value reached or increment
            {
                operation = Incriment;
            }
            else
            {
                operation = Decrement;
            }

            StartCoroutine(CashCounterAnimation(inital_CurrencyOfValType, target_CurrencyOfValType, operation));
        }
    }


    public delegate int Operation(int val, int init, int targ);
    public IEnumerator CashCounterAnimation(int startAmount, int targetAmount, Operation op)
    {
        int currentValue = startAmount;

        while (currentValue != targetAmount)
        {
            currentValue = op(currentValue, startAmount, targetAmount);

            //print(op(currentValue));
            thisText.text = Format(currentValue);

            yield return new WaitForSecondsRealtime(0.001f);
        }
        if(currentScene.name == "Game")
        {
            gameData.SaveGameData();
        }
        else
        {
            menuData.SaveGameData();
        }
    }

    public void AdjustDecIncVal(int initialval, int targetVal)
    {
        int difference = Mathf.Max(initialval, targetVal) - Mathf.Min(initialval, targetVal);
        DecIncValue = Mathf.RoundToInt(difference / 50f);
        if(DecIncValue == 0)
        {
            DecIncValue = 1;
        }
    }

    private int Decrement(int val, int init, int targ)
    {
        int v;
        if(val - Mathf.RoundToInt(DecIncValue * 50f * Time.deltaTime) > targ)
        {
            v = val - Mathf.RoundToInt(DecIncValue * 50f * Time.deltaTime);
        }
        else
        {
            v = targ;
        }
        return v;
    }

    private int Incriment(int val, int init, int targ)
    {
        int v;
        if (val + Mathf.RoundToInt(DecIncValue * 50f * Time.deltaTime) < targ)
        {
            v = val + Mathf.RoundToInt(DecIncValue * 50f * Time.deltaTime);
        }
        else
        {
            v = targ;
        }
        return v;
    }

    public string Format(int number)
    {
        string constructedNum = "";
        string num = number.ToString();
        if(num.Length >= 4)
        {
            int leftSideDigs = num.Length - 3;

            for (int i = 0; i < num.Length; i++)
            {
                if(i == leftSideDigs)
                {
                    constructedNum += ",";
                    constructedNum += num[i];
                }
                else
                {
                    constructedNum += num[i];
                }
            }
        }
        else
        {
            constructedNum = number.ToString();
        }

        return constructedNum;
    }

    public int DeFormat(string number)
    {
        string constructedNum = "";

        for(int i = 0; i < number.Length; i++)
        {
            if(number[i] == ',')
            {
                continue;
            }
            else
            {
                constructedNum += number[i];
            }
        }

        return Convert.ToInt32(constructedNum);
    }
    public void OnPauseOpen()
    {
        UpdateCurrency(gameData, menuData, valueType);
    }

    public void OnResumeClose()
    {
        UpdateCurrency(gameData, menuData, valueType);
    }

    public void OnDeathOpen()
    {
        UpdateCurrency(gameData, menuData, valueType);
    }
}
