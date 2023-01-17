using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using Backend;

namespace Architecture.Managers
{
    public class GamePlay : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] TextMeshProUGUI scoreText;
        [SerializeField] TextMeshProUGUI highscore;
        [SerializeField] CanvasGroup curtainCG;

        [Header("Flying Enemy Settings")]
        [SerializeField] float highestSpawnTime = 15f;
        public float HighestSpawnTime { get { return highestSpawnTime; } }
        [SerializeField] float lowestSpawnTime = 3f;
        public float LowestSpawnTime { get { return lowestSpawnTime; } }
        [SerializeField] float sinGraphExaggeration = 1f;
        public float SinGraphExaggeration { get { return sinGraphExaggeration; } }

        [Header("Platform Settings")]
        [SerializeField] float CapShrinkAtScore;

        #region STATE
        public bool TutorialComplete { get; private set; } = false;
        public void CompleteTutorial()
        {
            TutorialComplete = true;
        }
        public bool AllowSlideMove { get; set; } = true;
        public int Score { get; set; } = 0;
        public bool Paused { get; set; } = false;
        public float MinPlatLength { get; set; } = 0;
        public float MaxPlatLength { get; set; } = 0;
        public float PlatLength { get; set; } = 0;
        public bool NextPlatIsHole { get; set; } = false;
        public int FlyingEnemiesKilled { get; set; } = 0;
        public List<GameObject> EnemiesActive { get; set; } = new List<GameObject>();
        #endregion

        Camera cam;
        int currentGameHighscore;

        #region UNITY
        void Awake()
        {
            scoreText.text = "0";
            Score = 0;
            cam = GameObject.Find("Main Camera").GetComponent<Camera>();
            Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane)); //Coords of top right corner of screen

            FlyingEnemiesKilled = 0;
            MaxPlatLength = (topRight.x * 2) / 3;
            CalculateMinPlatLength();
        }

        void Start()
        {
            LeanTween.cancelAll();
            curtainCG.alpha = 1f;
            LeanTween.alphaCanvas(curtainCG, 0f, 0.4f).setIgnoreTimeScale(true);

            Localisation.LocalizationSystem.Instance.ReLocalizeTexts();
        }

        void Update()
        {
            Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane)); //Coords of top right corner of screen

            UpdateScore();
            UpdateHighscore();

            highscore.text = UserGameData.Instance.allTimeHighscore.ToString();
        }
        #endregion

        void UpdateScore()
        {
            scoreText.text = Score.ToString();
        }

        void UpdateHighscore()
        {
            if (Convert.ToInt32(Score) > currentGameHighscore)
            {
                currentGameHighscore = Convert.ToInt32(Score);
            }
            if (currentGameHighscore > UserGameData.Instance.allTimeHighscore)
            {
                UserGameData.Instance.allTimeHighscore = currentGameHighscore;

                //MAKE THIS HAPPEN UPON A CONDITION SUCH AS IF THAT IT HAPPENS ONCE PLAYER DIES
            }
        }

        public void CalculateMinPlatLength()
        {
            if (Score > 0)
            {
                float percentage = (CapShrinkAtScore - Score) / CapShrinkAtScore;
                MinPlatLength = Mathf.Max(0.4f, MaxPlatLength * percentage);
            }
            else
            {
                MinPlatLength = MaxPlatLength;
            }
        }

        public static GamePlay GetReference()
        {
            return GameObject.FindGameObjectWithTag("GameController").GetComponent<GamePlay>();
        }
    }
}