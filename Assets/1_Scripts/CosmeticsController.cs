using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Architecture.Managers;

public class CosmeticsController : Backend.AbstractSingleton<CosmeticsController>
{
    public SkinSpawner skinSpawner;
    private Scene currentScene;

    [Header("Player Parts")]
    public SpriteRenderer topSkinSprite;
    public SpriteRenderer eyesSprite;
    public SpriteRenderer topSprite;
    public SpriteRenderer bottomSprite;
    public SpriteRenderer springSprite;

    protected override void SingletonAwake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        print("COSMETICS CONTROLLER AWAKE");
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        print("LOADED SCENE: " + scene.name);

        currentScene = scene;
        if (scene.name == "Game")// && this!=null
        {
            print("LOADING COSMETICS");

            GameObject player = GameObject.Find("Player");
            player.GetComponent<PlayerController>().cosCon = this;
            topSkinSprite = player.GetComponent<SpriteRenderer>();
            eyesSprite = GameObject.Find("Player/Eyes").GetComponent<SpriteRenderer>();
            topSprite = GameObject.Find("Player/Base").GetComponent<SpriteRenderer>();
            bottomSprite = GameObject.Find("Player/PlayerBottom").GetComponent<SpriteRenderer>();
            springSprite = GameObject.Find("Player/PlayerBottom/Spring").GetComponent<SpriteRenderer>();
            springSprite.color = UserGameData.Instance.springColor;

            LoadCosmeticValues();
        }
    }

    public void LoadCosmeticValues()
    {
        if(currentScene.name == "Game")
        {
            if (!UserGameData.Instance.currentSkinPremium)
            {
                UserGameData.Instance.cSpecs = UserGameData.Instance.allSkinSpecs[UserGameData.Instance.allSkinsCodes.IndexOf(UserGameData.Instance.currentSkin)];
                SkinSpecsSolid cSpecs = UserGameData.Instance.cSpecs;

                //print(Architecture.Managers.UserGameData.Instance.cSpecs.altBounceSound);

                if (!cSpecs.alt_base)
                {

                    topSprite.sprite = cSpecs.base_Top;
                    eyesSprite.sprite = cSpecs.eyes;
                    bottomSprite.sprite = cSpecs.base_Bottom;
                    springSprite.sprite = cSpecs.spring_sprite;


                    //set colours
                    SetColours(cSpecs);

                    //set skin
                    topSkinSprite.sprite = cSpecs.skin_Top;
                    if (cSpecs.skin_Bottom != null)
                    {
                        bottomSprite.sprite = cSpecs.skin_Bottom;
                    }

                    //hide missing
                    HideMissing(cSpecs);
                }
                else
                {
                    topSprite.sprite = cSpecs.alt_Base_Sprite;
                    eyesSprite.sprite = cSpecs.eyes;

                    //set colours
                    SetColours(cSpecs);

                    //set skin
                    topSkinSprite.sprite = cSpecs.skin_Top;

                    //hide missing

                    var temp = springSprite.color;
                    temp.a = 0f;
                    springSprite.color = temp;
                    temp = bottomSprite.color;
                    temp.a = 0f;
                    bottomSprite.color = temp;

                    HideMissing(cSpecs);
                }
            }
            else
            {
                skinSpawner.SpawnPremium(UserGameData.Instance.activePremiumSkinName);
            }
            
        }
    }

    public void SetColours(SkinSpecsSolid cSpecs)
    {
        if (cSpecs.colour_changeable_top)
        {
            topSprite.color = UserGameData.Instance.topColor;
        }
        else
        {
            topSprite.color = Color.white;
        }

        if (cSpecs.colour_changeable_bottom)
        {
            bottomSprite.color = UserGameData.Instance.bottomColor;
        }
        else
        {
            bottomSprite.color = Color.white;
        }

        if (cSpecs.colour_changeable_eyes)
        {
            eyesSprite.color = UserGameData.Instance.topColor;
        }
        else
        {
            eyesSprite.color = Color.white;
        }

        if (cSpecs.colour_top_equal_to_bottom)
        {
            bottomSprite.color = UserGameData.Instance.topColor;
        }
        else
        {
            bottomSprite.color = bottomSprite.color;
        }

        springSprite.color = UserGameData.Instance.springColor;
        topSkinSprite.color = Color.white;
    }

    public void HideMissing(SkinSpecsSolid cSpecs)
    {
        Color temp;
        if (cSpecs.demo_spring_sprite == null)
        {
            temp = springSprite.color;
            temp.a = 0f;
            springSprite.color = temp;
        }
        if (cSpecs.skin_Top == null)
        {
            temp = topSkinSprite.color;
            temp.a = 0f;
            topSkinSprite.color = temp;
        }
        if (topSprite.sprite == null)
        {
            temp = topSprite.color;
            temp.a = 0f;
            topSprite.color = temp;
        }
        if (bottomSprite.sprite == null)
        {
            temp = bottomSprite.color;
            temp.a = 0f;
            bottomSprite.color = temp;
        }
        if (cSpecs.eyes == null)
        {
            temp = eyesSprite.color;
            temp.a = 0f;
            eyesSprite.color = temp;
        }
    }
}
