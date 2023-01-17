using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Architecture.Managers;

public class ButtonMouseDown : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform masterCanvasRect;
    public RectTransform buttonRect;
    public RectTransform targetRect;
    public GameObject text1;
    public GameObject text2;
    public float deltaY;

    [Header("Click State")]
    public bool buttonBeingPressed = false;

    [Header("Click Options")]
    public bool tintOnPress = false;
    public TextMeshProUGUI textDetails;
    public Image imageDetails;
    public Color org_color = Color.white;
    public Color tintColor = Color.white;

    [Header("Clicks")]
    public AudioSource pos_click;
    public AudioSource neg_click;

    public bool negative_first = false;
    public bool ignoreClick = false;
    public bool single_click = false;
    public bool single_on_up = false;

    [Header("BLOCK CLICKS (DISABLE BUTTON")]
    public bool disabledButton = false;

    public void Awake()
    {
        GetAudioRules();
    }

    public void Update()
    {
        if(gameObject.activeSelf == true)
        {
            CheckForClick();
        }

        soundsOn = UserGameData.Instance.soundsOn;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!disabledButton)
        {
            buttonBeingPressed = true;

            if (soundsOn)
            {
                if (!ignoreClick && !single_on_up)
                {
                    if (!negative_first)
                    {
                        pos_click.Play();
                    }
                    else
                    {
                        neg_click.Play();
                    }
                }
            }

            ChangeDueToPress();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!disabledButton)
        {
            buttonBeingPressed = false;

            if (soundsOn)
            {
                if (!ignoreClick && !single_click)
                {
                    if (!negative_first)
                    {
                        neg_click.Play();
                    }
                    else
                    {
                        pos_click.Play();
                    }
                }
                if (single_on_up)
                {
                    pos_click.Play();
                }
            }

            ReturnToDefault();
        }
    }

    private bool soundsOn;
    private void GetAudioRules()
    {
        soundsOn = UserGameData.Instance.soundsOn;
    }

    private void Start()
    {
        if(text2 != null)
        {
            text2.SetActive(false);
        }

        GameObject menuAudio = GameObject.Find("MenuAudio").gameObject;
        pos_click = menuAudio.transform.Find("PosClick").gameObject.GetComponent<AudioSource>();
        neg_click = menuAudio.transform.Find("NegClick").gameObject.GetComponent<AudioSource>();
    }

    public void CheckForClick()
    {
        
        /*
        if (Input.GetMouseButtonDown(0))
        {
            thisClick = Input.mousePosition;

            if(click_blocker != null)
            {
                if (click_blocker.filterActive == false && InMaskBounds(thisClick))
                {
                    buttonBeingPressed = true;

                    if (!ignoreClick)
                    {
                        if (!negative_first)
                        {
                            pos_click.Play();
                        }
                        else
                        {
                            neg_click.Play();
                        }
                    }

                    ChangeDueToPress();
                }
            }
            else
            {
                if (InMaskBounds(thisClick))
                {
                    buttonBeingPressed = true;

                    if (!ignoreClick)
                    {
                        if (!negative_first)
                        {
                            pos_click.Play();
                        }
                        else
                        {
                            neg_click.Play();
                        }
                    }

                    ChangeDueToPress();
                }
            }
        }

        if(click_blocker != null)
        {
            if (click_blocker.filterActive == false && Input.GetMouseButtonUp(0) && buttonBeingPressed)
            {
                buttonBeingPressed = false;

                if (!ignoreClick && !single_click)
                {
                    if (!negative_first)
                    {
                        neg_click.Play();
                    }
                    else
                    {
                        pos_click.Play();
                    }
                }

                ReturnToDefault();
            }
        }
        else
        {
            if (Input.GetMouseButtonUp(0) && buttonBeingPressed)
            {
                buttonBeingPressed = false;

                if (!ignoreClick && !single_click)
                {
                    if (!negative_first)
                    {
                        neg_click.Play();
                    }
                    else
                    {
                        pos_click.Play();
                    }
                }

                ReturnToDefault();
            }
        }
        /*
        if(Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);

            if(touch.phase == TouchPhase.Began)
            {
                if (InMaskBounds(touch.position))
                {
                    buttonBeingPressed = true;
                    ChangeDueToPress();
                }
            }
            if(touch.phase == TouchPhase.Ended)
            {
                print(buttonBeingPressed);
                print(gameObject.name + " return to default");
                buttonBeingPressed = false;
                ReturnToDefault();
            }
        }
        */
    }

    private void ChangeDueToPress()
    {

        if (text2 != null)
        {
            text2.SetActive(true);
            text1.SetActive(false);
        }

        if (tintOnPress)
        {
            if(textDetails != null)
            {
                textDetails.color = tintColor;
            }
            if(imageDetails != null)
            {
                imageDetails.color = tintColor;
            }
        }

        if(targetRect != null)
        {
            targetRect.localPosition = new Vector2(targetRect.localPosition.x, targetRect.localPosition.y - deltaY);
        }
    }

    private void ReturnToDefault()
    {

        if (text2 != null)
        {
            text1.SetActive(true);
            text2.SetActive(false);
        }

        if (tintOnPress)
        {
            if (textDetails != null)
            {
                textDetails.color = org_color;
            }
            if (imageDetails != null)
            {
                imageDetails.color = org_color;
            }
        }

        if (targetRect != null)
        {
            targetRect.localPosition = new Vector2(targetRect.localPosition.x, targetRect.localPosition.y + deltaY);
        }
    }

    private bool InMaskBounds(Vector3 clickPos)
    {
        bool withinX = false;
        bool withinY = false;

        if (clickPos.y >= buttonRect.position.y - ((buttonRect.sizeDelta.y * masterCanvasRect.localScale.y) / 2f) &&
            clickPos.y <= buttonRect.position.y + ((buttonRect.sizeDelta.y * masterCanvasRect.localScale.y) / 2f))
        {
            withinY = true;
        }
        if (clickPos.x >= buttonRect.position.x - ((buttonRect.sizeDelta.x * masterCanvasRect.localScale.x) / 2f) &&
            clickPos.x <= buttonRect.position.x + ((buttonRect.sizeDelta.x * masterCanvasRect.localScale.x) / 2f))
        {
            withinX = true;
        }

        if (withinX && withinY)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
