using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Architecture.Managers
{
    public class GamePlay : MonoBehaviour
    {
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI highscore;
        public CanvasGroup curtainCG;

        [Header("TutorialStatus")]
        public bool tutorialComplete = false;
        public bool allowSlideMove = true;

        [Header("Other")]
        public int score;
        public int currentGameHighscore;

        public bool Paused;

        public Camera cam;
        Vector3 topRight;

        [Header("Platform variables")]

        public float minPlatLength;
        public float maxPlatLength;
        public float CapShrinkAtScore;
        public float platLength;
        public bool nextPlatIsHole = false;

        [Header("Flying Enemy Variables")]

        public float highestSpawnTime = 15f;
        public float lowestSpawnTime = 3f;
        public float sinGraphExaggeration = 1f;
        public int flyingEnemiesKilled;

        [Header("Enemies Active")]
        public List<GameObject> enemiesActive = new List<GameObject>();

        void Awake()
        {
            Paused = false;

            allowSlideMove = true;

            scoreText.text = "0";
            score = 0;
            cam = GameObject.Find("Main Camera").GetComponent<Camera>();
            topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane)); //Coords of top right corner of screen

            flyingEnemiesKilled = 0;
            maxPlatLength = (topRight.x * 2) / 3;
            CalculateMinPlatLength();
        }

        public void Start()
        {

            LeanTween.cancelAll();
            curtainCG.alpha = 1f;
            LeanTween.alphaCanvas(curtainCG, 0f, 0.4f).setIgnoreTimeScale(true);
        }

        void Update()
        {
            topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane)); //Coords of top right corner of screen

            UpdateScore();
            UpdateHighscore();

            highscore.text = UserGameData.Instance.allTimeHighscore.ToString();
        }

        void UpdateScore()
        {
            string scoreAsString = score.ToString();
            scoreText.text = scoreAsString;
        }

        public void CalculateMinPlatLength()
        {
            if (score > 0)
            {
                float percentage = (CapShrinkAtScore - score) / CapShrinkAtScore;
                minPlatLength = Mathf.Max(0.4f, maxPlatLength * percentage);
            }
            else
            {
                minPlatLength = maxPlatLength;
            }
        }

        void UpdateHighscore()
        {
            if (Convert.ToInt32(score) > currentGameHighscore)
            {
                currentGameHighscore = Convert.ToInt32(score);
            }
            if (currentGameHighscore > UserGameData.Instance.allTimeHighscore)
            {
                UserGameData.Instance.allTimeHighscore = currentGameHighscore;

                //MAKE THIS HAPPEN UPON A CONDITION SUCH AS IF THAT IT HAPPENS ONCE PLAYER DIES
            }
        }
    }
}