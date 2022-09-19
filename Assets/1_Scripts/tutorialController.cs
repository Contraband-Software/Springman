using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class tutorialController : MonoBehaviour
{
    public Canvas thisCanvas;
    public WaterRise waterRise;
    public GameObject fade;
    public GameObject PABot;
    public GameObject speechBubble;
    public TextMeshProUGUI speechBubbleText;
    public TextLocaliserUI tutorialTL;
    public GameData gameData;
    public PlayerController pController;
    private RectTransform PABotRect;
    private RectTransform fadeRect;
    public Animator flameAnimator;
    public Canvas gameCanvas;
    public Camera cam;
    public TapToContinue tapToContinue;

    private float distToZerof;
    private float distToZerofHand;

    [Header("Semi UI")]
    public GameObject circles;
    public LeanTweenType circleFadeEase;

    public GameObject responsiveArea;
    public GameObject pointingHand;
    public LeanTweenType pointingHandEase;
    public LeanTweenType handFadeEase;

    public GameObject demoPlatform;
    public GameObject lowestPlatform;
    public SlideMove demoPlatformScript;
    public GameObject pointingHandPoint;
    public float demoPlatformDeltaX;
    public float lowestPlatformSize;

    [Header("Tutorial Status")]
    public bool tutorialComplete;
    public int tutorialPhase = -1;
    public bool floating = false;
    public bool allowedToContinue;
    private bool tutorialHidden = false;

    [Header("Float Eases")]
    public LeanTweenType floatDown;
    public LeanTweenType floatUp;
    public LeanTweenType scaleSpeechBubble;
    public LeanTweenType timeScaleDownEase;
    public LeanTweenType timeScaleUpEase;

    [Header("LocalizationKeys")]
    private string[] keys = { "tut1", "tut2", "tut3", "tut4", "tut5", "tut6", "tut7", "tut8", "tut9", "tut10" };
    public List<Action> tutorialPhases;
    public Action tutorialPhaseAction;

    private Vector3 topRight;
    private Vector3 bottomLeft;

    private void Awake()
    {
        tutorialTL.key = keys[0];
    }
    void Start()
    {
        tutorialPhases = new List<Action> { null, null, null, null, TutorialPhase_PlatIndicators, TutorialPhase_ResponsiveArea, TutorialPhase_RA_Plus_HandDemo,
            null, null, TutorialPhase_PreFinal };

        thisCanvas.enabled = false;
        tutorialHidden = false;
        tutorialPhase = -1;
        tutorialTL.key = keys[0];
        floating = false;
        allowedToContinue = false;

        tutorialComplete = gameData.tutorialComplete;

        topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane)); //Coords of top right corner of screen
        bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane)); //coords of bottom left corner of screen

        PABotRect = PABot.GetComponent<RectTransform>();
        fadeRect = fade.GetComponent<RectTransform>();

        if(gameData.tutorialComplete == false)
        {
            waterRise.enabled = false;
            gameData.allowSlideMove = false;
        }

    }

    void Update()
    {
        //POR4
        if(pController.isGrounded && pController.state == PlayerController.State.Alive && tutorialComplete == false && tutorialPhase == -1)
        {
            TutorialInitiate();
        }

        if (Input.GetKeyDown(KeyCode.Space) && tutorialPhase < keys.Length && allowedToContinue && tutorialComplete == false && gameData.Paused == false)
        {
            speechBubble.gameObject.SetActive(false);
            ShowSpeechBubble();
            tapToContinue.FadeOut();
        }
        if(tutorialPhase == keys.Length)
        {
            if (lowestPlatform.transform.position.x + lowestPlatformSize > pController.transform.position.x - pController.bounds.x && tutorialHidden == false)
            {
                HideTutorial();
            }
        }
    }

    public void ContinueIfAllowed()
    {
        if (tutorialPhase < keys.Length && allowedToContinue && tutorialComplete == false && gameData.Paused == false)
        {
            speechBubble.gameObject.SetActive(false);
            tapToContinue.FadeOut();
            ShowSpeechBubble();
        }
    }

    void TutorialInitiate()
    {
        thisCanvas.enabled = true;
        gameData.allowSlideMove = false;

        pController.rb.constraints = RigidbodyConstraints2D.FreezePositionX;

        //LeanTween.value(gameObject, ChangeTimeScale, 1f, 0f, 1.2f).setEase(timeScaleDownEase).setIgnoreTimeScale(true);
        LeanTween.value(gameObject, ChangeGravScale, 1f, 0f, 1.75f).setEase(timeScaleDownEase).setIgnoreTimeScale(true);
        LeanTween.value(gameObject, ChangeLinearDrag, 0f, 3f, 1.75f).setEase(timeScaleDownEase).setIgnoreTimeScale(true);

        demoPlatform = GameObject.Find("DemoPlatform");
        lowestPlatform = GameObject.Find("LowestPlatform");
        demoPlatformScript = demoPlatform.GetComponent<SlideMove>();
        pointingHandPoint = pointingHand.transform.GetChild(0).gameObject;

        lowestPlatformSize = lowestPlatform.transform.GetChild(0).GetComponent<BoxCollider2D>().bounds.extents.x +
            lowestPlatform.transform.GetChild(1).GetComponent<BoxCollider2D>().bounds.extents.x;

        //CanvasGroup gameCanvasCG = gameCanvas.GetComponent<CanvasGroup>();
        //gameCanvas.transform.Find("PauseButton").GetComponent<Button>().enabled = false;
        //LeanTween.alphaCanvas(gameCanvasCG, 0f, 0.4f).setIgnoreTimeScale(true).setEase(LeanTweenType.easeInQuad).setOnComplete(TurnOffGameCanvas);

        CanvasGroup fadeCG = fade.GetComponent<CanvasGroup>();

        fadeCG.alpha = 0f;
        //fadeRect.anchoredPosition = new Vector2(fadeRect.anchoredPosition.x, 18f);
        LeanTween.alphaCanvas(fadeCG, 1f, 0.8f).setIgnoreTimeScale(true).setEase(LeanTweenType.easeInOutQuad);
        //LeanTween.value(fade, ChangeYValueFade, fadeRect.anchoredPosition.y, -403f, 0.8f).setIgnoreTimeScale(true);

        PABotRect.anchoredPosition = new Vector2(PABotRect.anchoredPosition.x, 268f);
        flameAnimator.SetFloat("flameSpeed", 1.375f);
        LeanTween.delayedCall(0.8f, ResetFlameSpeed).setIgnoreTimeScale(true);
        LeanTween.value(PABot, ChangeYValue, 268f, -280f, 0.8f).setEase(LeanTweenType.easeOutQuad).setOnComplete(FloatUp).setIgnoreTimeScale(true);

        speechBubble.gameObject.SetActive(false);
        LeanTween.delayedCall(0.4f, ShowSpeechBubble).setIgnoreTimeScale(true);

        IncrementTutorialPhase();
    }

    void ShowSpeechBubble()
    {
        allowedToContinue = false;

        tutorialPhaseAction?.Invoke(); //this is to call the previous action again so it can deactivate the tweening
        tutorialPhaseAction = tutorialPhases[tutorialPhase];
        tutorialPhaseAction?.Invoke();

        speechBubble.gameObject.SetActive(true);
        tutorialTL.key = keys[tutorialPhase];
        tutorialTL.Localize();

        speechBubble.transform.localScale = new Vector3(0.4f, 0.4f, 1f);
        LeanTween.scale(speechBubble, new Vector3(1f, 1f, 1f), 0.3f).setEase(scaleSpeechBubble).setIgnoreTimeScale(true);

        if(tutorialPhaseAction != TutorialPhase_RA_Plus_HandDemo)
        {
            LeanTween.delayedCall(0.5f, AllowContinue).setIgnoreTimeScale(true);
        }

        IncrementTutorialPhase();
    }

    void AllowContinue()
    {
        if (allowedToContinue == false && tapToContinue.enabled == true && speechBubble.gameObject.activeSelf)
        {
            allowedToContinue = true;
            tapToContinue.FadeIn();
        }
    }

    void TutorialPhase_PlatIndicators()
    {
        GameObject circle1 = circles.transform.GetChild(0).gameObject;
        GameObject circle2 = circles.transform.GetChild(1).gameObject;

        SpriteRenderer c1Rend = circle1.GetComponent<SpriteRenderer>();
        SpriteRenderer c2Rend = circle2.GetComponent<SpriteRenderer>();

        if (tutorialPhase == tutorialPhases.IndexOf(TutorialPhase_PlatIndicators))
        {
            circles.SetActive(true);

            c1Rend.color = new Color(c1Rend.color.r, c1Rend.color.g, c1Rend.color.b, 0f);
            c2Rend.color = new Color(c2Rend.color.r, c2Rend.color.g, c2Rend.color.b, 0f);

            LeanTween.alpha(circle1, 1f, 1f).setLoopPingPong().setIgnoreTimeScale(true).setEase(circleFadeEase);
            LeanTween.alpha(circle2, 1f, 1f).setLoopPingPong().setIgnoreTimeScale(true).setEase(circleFadeEase);
        }
        else
        {
            LeanTween.cancel(circle1);
            LeanTween.cancel(circle2);

            distToZerof = c1Rend.color.a / 2f; //no need to alter as the normal time = 1f and color.a max = 1f too //EDIT done twice as fast

            LeanTween.alpha(circle1, 0f, distToZerof).setIgnoreTimeScale(true).setEase(circleFadeEase);
            LeanTween.alpha(circle2, 0f, distToZerof).setIgnoreTimeScale(true).setEase(circleFadeEase).setOnComplete(TurnOffCircles);
        }
 
    }

    void TutorialPhase_ResponsiveArea()
    {
        SpriteRenderer resAreaRend = responsiveArea.GetComponent<SpriteRenderer>();

        if(tutorialPhase == tutorialPhases.IndexOf(TutorialPhase_ResponsiveArea))
        {
            responsiveArea.SetActive(true);

            resAreaRend.color = new Color(resAreaRend.color.r, resAreaRend.color.g, resAreaRend.color.b, 0f);
            LeanTween.alpha(responsiveArea, 1f, 1f).setIgnoreTimeScale(true).setEase(circleFadeEase).setOnComplete(Tutorial_ResponsiveArea_StartLooping);
        }
    }

    void TutorialPhase_RA_Plus_HandDemo()
    {
        SpriteRenderer resAreaRend = responsiveArea.GetComponent<SpriteRenderer>();

        if (tutorialPhase == tutorialPhases.IndexOf(TutorialPhase_RA_Plus_HandDemo))
        {
            pointingHand.SetActive(true);
            pointingHand.transform.position = new Vector3(topRight.x - 0.717587f, -2.5f);

            FadeHandIn();

            LeanTween.value(pointingHand, UpdateXPositionHand, pointingHand.transform.position.x, pointingHand.transform.position.x - 2.25f, 1f)
                .setIgnoreTimeScale(true).setEase(pointingHandEase).setOnComplete(FadeHandOut);
            LeanTween.delayedCall(1.6f, TutorialPhase_RA_Plus_HandDemo_P2).setIgnoreTimeScale(true);
        }
        else
        {
            LeanTween.cancel(responsiveArea);

            distToZerof = resAreaRend.color.a / 2f; //no need to alter as the normal time = 1f and color.a max = 1f too EDIT: made it 2x speed lol

            LeanTween.alpha(responsiveArea, 0f, distToZerof).setIgnoreTimeScale(true).setEase(circleFadeEase).setOnComplete(TurnOffResponsiveArea);
            
            LeanTween.cancel(pointingHand);

            distToZerofHand = pointingHand.GetComponent<SpriteRenderer>().color.a * 0.12f;

            LeanTween.alpha(pointingHand, 0f, 0.25f).setIgnoreTimeScale(true).setEase(handFadeEase).setOnComplete(TurnOffHand);
        }
    }
    void TutorialPhase_RA_Plus_HandDemo_P2()
    {
        pointingHand.transform.position = new Vector3(topRight.x - 0.717587f, -1.15f);
        FadeHandIn();

        LeanTween.value(pointingHand, UpdateXPositionHand, pointingHand.transform.position.x, pointingHand.transform.position.x - 2.25f, 1f)
            .setIgnoreTimeScale(true).setLoopPingPong(1).setEase(pointingHandEase).setOnComplete(FadeHandOut);
        LeanTween.delayedCall(2.6f, TutorialPhase_RA_Plus_HandDemo_P3).setIgnoreTimeScale(true);
    }

    void TutorialPhase_RA_Plus_HandDemo_P3()
    {
        pointingHand.transform.position = new Vector3(topRight.x - 0.717587f, 0.5f);
        FadeHandIn();

        LeanTween.value(pointingHand, UpdateXPositionHand, pointingHand.transform.position.x, pointingHand.transform.position.x - 2.25f, 1f).
            setIgnoreTimeScale(true).setLoopPingPong(1).setEase(pointingHandEase).setOnComplete(FadeHandOut);


        LeanTween.delayedCall(2.7f, AutoShowNextSpeechBubble).setIgnoreTimeScale(true);
    }

    void AutoShowNextSpeechBubble()
    {
        speechBubble.gameObject.SetActive(false);
        ShowSpeechBubble();
    }

    void TutorialPhase_PreFinal()
    {
        gameData.allowSlideMove = true;

        tapToContinue.FadeOutFinal();

        pointingHand.gameObject.SetActive(true);
        SpriteRenderer handRend = pointingHand.GetComponent<SpriteRenderer>();
        handRend.color = new Color(handRend.color.r, handRend.color.g, handRend.color.b, 0f);

        TutorialPhase_PreFinal_HandLoop1();
    }

    void TutorialPhase_PreFinal_HandLoop1()
    {
        pointingHand.transform.position = new Vector3(bottomLeft.x + topRight.x * 0.2f, -2.9f);

        LeanTween.alpha(pointingHand, 1f, 0.3f).setIgnoreTimeScale(true).setEase(handFadeEase);
        LeanTween.moveLocalX(pointingHand, pointingHand.transform.position.x + 2.25f, 1.4f).
            setIgnoreTimeScale(true).setEase(pointingHandEase).setOnComplete(TutorialPhase_PreFinal_HandLoop2);
    }
    void TutorialPhase_PreFinal_HandLoop2()
    {
        LeanTween.alpha(pointingHand, 0f, 0.6f).setIgnoreTimeScale(true).setEase(handFadeEase).setOnComplete(TutorialPhase_PreFinal_HandLoop1);
    }

    void HideTutorial()
    {
        tutorialHidden = true;

        pController.rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        pController.rb.gravityScale = 1f;
        pController.rb.drag = 0f;

        LeanTween.value(gameObject, ChangeTimeScale, Time.timeScale, 1f, 0.5f).setEase(timeScaleUpEase).setIgnoreTimeScale(true);

        LeanTween.alpha(pointingHand, 0f, 0.25f).setIgnoreTimeScale(true).setEase(handFadeEase);

        CanvasGroup fadeCG = fade.GetComponent<CanvasGroup>();

        LeanTween.alphaCanvas(fadeCG, 0f, 0.8f).setIgnoreTimeScale(true).setEase(LeanTweenType.easeInOutQuad);

        LeanTween.scale(speechBubble, new Vector3(0f, 0f, 0f), 0.5f).setIgnoreTimeScale(true).setEaseInBack();

        flameAnimator.SetFloat("flameSpeed", 1.375f);
        LeanTween.cancel(PABot);
        LeanTween.value(PABot, ChangeYValue, PABotRect.anchoredPosition.y, 286f, 0.8f).setEase(LeanTweenType.easeInQuad).setIgnoreTimeScale(true)
            .setOnComplete(DestroyTutorialItems);

        tutorialComplete = true;
        gameData.tutorialComplete = true;

        waterRise.enabled = true;
    }

    void DestroyTutorialItems()
    {
        Destroy(circles);
        Destroy(responsiveArea);
        Destroy(pointingHand);
        Destroy(gameObject);
    }

    void FadeHandIn()
    {
        SpriteRenderer handRend = pointingHand.GetComponent<SpriteRenderer>();
        handRend.color = new Color(handRend.color.r, handRend.color.g, handRend.color.b, 0f);
        LeanTween.alpha(pointingHand, 1f, 0.3f).setIgnoreTimeScale(true).setEase(handFadeEase);

        demoPlatformDeltaX = pointingHandPoint.transform.position.x - demoPlatformScript.transform.position.x;
    }

    void FadeHandOut()
    {
        SpriteRenderer handRend = pointingHand.GetComponent<SpriteRenderer>();
        handRend.color = new Color(handRend.color.r, handRend.color.g, handRend.color.b, 1f);
        LeanTween.alpha(pointingHand, 0f, 0.6f).setIgnoreTimeScale(true).setEase(handFadeEase);

        demoPlatformScript.MoveByDemoEnd();
    }

    void UpdateXPositionHand(float x)
    {
        pointingHand.transform.position = new Vector3(x, pointingHand.transform.position.y);
        demoPlatformScript.moving = true;

        if(pointingHand.transform.position.y != -2.5f)
        {
            demoPlatformScript.MoveByDemo(demoPlatformDeltaX, pointingHandPoint.transform.position);
        }
    }

    void Tutorial_ResponsiveArea_StartLooping()
    {
        LeanTween.alpha(responsiveArea, 0.5f, 1f).setIgnoreTimeScale(true).setEase(circleFadeEase).setLoopPingPong();
    }

    void ChangeTimeScale(float newTimeScale)
    {
        Time.timeScale = newTimeScale;
    }
    void FreezeTime()
    {
        Time.timeScale = 0f;
    }
    void ChangeGravScale(float newGravScale)
    {
        pController.rb.gravityScale = newGravScale;

        /*
        if(pController.rb.velocity.y == 0f || (pController.rb.velocity.y > -0.03f && pController.rb.velocity.y < 0f) ||
            (pController.rb.velocity.y < 0.03f && pController.rb.velocity.y > 0f) && Time.unscaledTime > 0.4f && pController.isGrounded == false)
        {
            print("freezing");
            pController.rb.constraints = RigidbodyConstraints2D.FreezeAll;
            FreezeTime();
        }
        */

        if(pController.rb.velocity.y == 0f || (pController.rb.velocity.y > -0.03f && pController.rb.velocity.y < 0f) ||
            (pController.rb.velocity.y < 0.03f && pController.rb.velocity.y > 0f))
        {
            if(Time.unscaledTime > 0.4f)
            {
                if(pController.isGrounded == false)
                {
                    pController.rb.constraints = RigidbodyConstraints2D.FreezeAll;
                    FreezeTime();
                }
            }
        }
    }
    void ChangeLinearDrag(float newLD)
    {
        pController.rb.drag = newLD;
    }
    void FloatDown()
    {
        LeanTween.value(PABot, ChangeYValue, PABotRect.anchoredPosition.y, -280f, 1.1f).setEase(floatDown).setOnComplete(FloatUp).setIgnoreTimeScale(true);
    }
    void FloatUp()
    {
        LeanTween.value(PABot, ChangeYValue, PABotRect.anchoredPosition.y, -250f, 1.1f).setEase(floatUp).setOnComplete(FloatDown).setIgnoreTimeScale(true);
    }

    void ResetFlameSpeed()
    {
        flameAnimator.SetFloat("flameSpeed", 1f);
    }

    void TurnOffGameCanvas()
    {
        gameCanvas.gameObject.SetActive(false);
    }

    void TurnOffCircles()
    {
        circles.gameObject.SetActive(false);
    }

    void TurnOffResponsiveArea()
    {
        responsiveArea.gameObject.SetActive(false);
    }

    void TurnOffHand()
    {
        pointingHand.gameObject.SetActive(false);
        //LeanTween.alpha(pointingHand, 1f, 1f).setIgnoreTimeScale(true);
    }

    public void ChangeYValue(float y)
    {
        PABotRect.anchoredPosition = new Vector2(PABotRect.anchoredPosition.x, y);
    }

    public void ChangeYValueFade(float y)
    {
        fadeRect.anchoredPosition = new Vector2(fadeRect.anchoredPosition.x, y);
    }

    void IncrementTutorialPhase()
    {
        tutorialPhase++;
    }
}
