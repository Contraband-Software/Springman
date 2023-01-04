using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using Development;

[CustomEditor(typeof(GameDebugController))]
public class TestingConsoleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GameDebugController controller = (GameDebugController)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Cheats", EditorStyles.boldLabel);
        if (GUILayout.Button("+100 Silver Bolts"))
        {
            switch (SceneManager.GetActiveScene().name)
            {
                case "Main Menu":
                    MenuData menuObj = GameObject.Find("MenuController").GetComponent<MenuData>();
                    menuObj.silver += 100;
                    break;
                case "Game":
                    GameData gameObj = GameObject.Find("GameController").GetComponent<GameData>();
                    gameObj.silver += 100;
                    break;
            }
        }
        if (GUILayout.Button("+100 Golden Bolts"))
        {
            switch (SceneManager.GetActiveScene().name)
            {
                case "Main Menu":
                    MenuData menuObj = GameObject.Find("MenuController").GetComponent<MenuData>();
                    menuObj.gold += 100;
                    break;
                case "Game":
                    GameData gameObj = GameObject.Find("GameController").GetComponent<GameData>();
                    gameObj.gold += 100;
                    break;
            }
        }

        //EditorGUILayout.Space();
        //EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);
        //if (GUILayout.Button("Reset progression"))
        //{
        //    switch (SceneManager.GetActiveScene().name)
        //    {
        //        case "Main Menu":
        //            MenuData menuObj = GameObject.Find("MenuController").GetComponent<MenuData>();
        //            menuObj.ads
        //            break;
        //        case "Game":
        //            GameData gameObj = GameObject.Find("GameController").GetComponent<GameData>();
        //            gameObj.gold += 100;
        //            break;
        //    }
        //}
    }
}
