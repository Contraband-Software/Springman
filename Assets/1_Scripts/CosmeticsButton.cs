using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CosmeticsButton : MonoBehaviour
{
    [Header("Main Menu Stuff")]
    public Canvas mainMenuCanvas;
    public GameObject currencyTab;
    Vector2 originalCoords;
    RectTransform mmCanvasRect;

    [Header("Cosmetics Menu Stuff")]
    public Canvas cosmeticsCanvas;
    public CosmeticsMenuController cmCon;

    [Header("Tweens")]
    public LeanTweenType openEase;
    public LeanTweenType closeEase;

    [Header("Other")]
    public Canvas transitionCanvas;
    List<GameObject> transitionObjects = new List<GameObject>();

    private void Start()
    {
        mmCanvasRect = mainMenuCanvas.GetComponent<RectTransform>();
        originalCoords = mmCanvasRect.transform.position;
        cosmeticsCanvas.enabled = false;
    }

    public void SlideMainMenuUp()
    {
        cosmeticsCanvas.enabled = true;
        transitionObjects.Add(currencyTab);
        DepositIntoTransitionCanvas();

        LeanTween.value(mainMenuCanvas.gameObject, SMMUCallback, mmCanvasRect.transform.position.y, mmCanvasRect.transform.position.y * 3, 0.4f).setIgnoreTimeScale(true)
            .setEase(closeEase).setOnComplete(DeactivateMainMenuCanvas);


        LeanTween.delayedCall(0.15f, cmCon.OpenMenu);
    }

    public void SlideMainMenuDown()
    {
        //enable main menu canvas
        mainMenuCanvas.enabled = true;

        LeanTween.value(mainMenuCanvas.gameObject, SMMUCallback, mmCanvasRect.transform.position.y, originalCoords.y, 0.4f).setIgnoreTimeScale(true)
            .setEase(openEase).setOnComplete(CollectFromTransitionCanvas);
    }
    public void SMMUCallback(float y)
    {
        mmCanvasRect.transform.position = new Vector2(mmCanvasRect.transform.position.x, y);
    }

    void CollectFromTransitionCanvas()
    {
        cmCon.TurnOffAllRectMasks();

        for (int child = 0; child < transitionCanvas.transform.childCount; child++)
        {
            transitionObjects.Add(transitionCanvas.transform.GetChild(child).gameObject);
            transitionCanvas.transform.GetChild(child).transform.SetParent(mainMenuCanvas.transform);
        }
    }

    void DepositIntoTransitionCanvas()
    {
        foreach (GameObject gameObj in transitionObjects)
        {
            gameObj.transform.SetParent(transitionCanvas.transform);
        }
    }

    public void DeactivateMainMenuCanvas()
    {
        mainMenuCanvas.enabled = false;
    } 
}
