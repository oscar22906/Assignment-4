using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Strobotnik.Klattersynth;

public class IntroDialogue : MonoBehaviour
{
    public float clickCooldown = 1;
    public int fadeTime = 2;


    public string[] texts1;
    public string[] texts2;
    public string[] texts3;

    private int currentIndex1 = 0;
    public int currentIndex2 = 0;
    private int currentIndex3 = 0;

    private bool cooldown = true;
    private bool dialogueBegun = false;
    private bool animationsPlayed = false;

    private bool isFaded = false;

    public GameObject button;
    public Animator handAnimator;
    public Animator buttonAnimator;
    public Animator SolarGlare;
    public CanvasGroup canvasGroup;
    public CanvasGroup fadeBlack;
    public CanvasGroup textCanvas;
    public CanvasGroup bjan;
    public CanvasGroup buttonCanvas;
    public Speech speech;


    private DialogueController dialogueController;
    private Board boardScript;
    private Persistent persistent;
    private GameObject canvas;
    private string playerName;

    void Start()
    {
        GameObject boardObj = GameObject.FindGameObjectWithTag("Board");
        boardScript = boardObj.GetComponent<Board>();
        GameObject persistentObj = GameObject.FindGameObjectWithTag("Persistent");
        persistent = persistentObj.GetComponent<Persistent>();
        GameObject dialogueObj = GameObject.FindGameObjectWithTag("Dialogue Controller");
        dialogueController = dialogueObj.GetComponent<DialogueController>();
        canvas = GameObject.FindGameObjectWithTag("Canvas");

        StartScene();
        StartCoroutine(DelayedStart());
    }
    void Update()
    {
        if (cooldown && dialogueBegun)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || playerName != null && Input.GetKeyDown(KeyCode.Return))
            {
                CooldownStart();
                if (currentIndex1 < texts1.Length)
                {
                    Dialogue();
                    if (currentIndex1 >= texts1.Length)
                    {
                        Debug.Log("Dialogue 1 end");
                        Debug.Log("Play animation here");
                        button.SetActive(true);
                        FadeOut(textCanvas, 1);
                        FadeOut(buttonCanvas, 0);
                        FadeIn(buttonCanvas, 2);
                    }
                }
                if (animationsPlayed && currentIndex2 < texts2.Length)
                {
                    Dialogue2();
                    if (currentIndex2 >= texts2.Length)
                    {
                        Debug.Log("Dialogue 2 end");
                        FadeIn(canvasGroup, 5);
                        boardScript.NewGame();
                        cooldown = true;
                    }
                }
                if (playerName != null && currentIndex3 < texts3.Length)
                {
                    FadeOut(canvasGroup, 1);
                    Dialogue3();
                    if (currentIndex3 >= texts3.Length)
                    {
                        Debug.Log("Dialogue 3 end");
                        FadeIn(fadeBlack, 3);
                    }
                }
            }
        }
    }
    public void ButtonPress()
    {
        buttonAnimator.SetBool("Pressed", true);
        handAnimator.SetBool("Grab", true);
        SolarGlare.SetBool("Grab", true);
        StartCoroutine(AnimationWait());
    }   

    private void Dialogue()
    {
        if (currentIndex1 == texts1.Length - 4)
        {
            handAnimator.SetBool("Touch", true);
        }
        speech.speak(texts1[currentIndex1], false);
        dialogueController.DisplayText(texts1[currentIndex1], false);
        currentIndex1++;
    }

    private void Dialogue2()
    {
        speech.speak(texts2[currentIndex2], false);
        dialogueController.DisplayText(texts2[currentIndex2], false);
        currentIndex2++;
    }

    private void Dialogue3()
    {
        if (currentIndex3 == 0)
        {
            string message = playerName + ", curious name... " + playerName + ", " + playerName + ", " + playerName + ".";
            speech.speak(message, false);
            dialogueController.DisplayText(message, false);
            currentIndex3++;
        }
        else
        {
            speech.speak(texts3[currentIndex3], false);
            dialogueController.DisplayText(texts3[currentIndex3], false);
            currentIndex3++;
        }
    }

    private void StartScene()
    {
        button.SetActive(false);
        FadeOut(bjan, 0);
        FadeOut(textCanvas, 0);
        FadeOut(canvasGroup, 0);
    }

    private void FadeIn(CanvasGroup canvasGroup, float fadeTime)
    {
        canvasGroup.DOFade(1, fadeTime);
    }
    private void FadeOut(CanvasGroup canvasGroup, float fadeTime)
    {
        canvasGroup.DOFade(0, fadeTime);
    }

    public void GiveName(string name)
    {
        playerName = name.ToLower().Trim('\0');
        boardScript.playerName = playerName;
        persistent.playerName = playerName;
    }
    public void CooldownStart()
    {
        StartCoroutine(CooldownCoroutine());
    }
    IEnumerator CooldownCoroutine()
    {
        cooldown = false;
        yield return new WaitForSeconds(clickCooldown);
        cooldown = true;
    }
    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(2);
        FadeIn(textCanvas, 1);
        FadeIn(bjan, 6);
        dialogueBegun = true;
    }

    IEnumerator AnimationWait()
    {
        yield return new WaitForSeconds(5);
        handAnimator.SetBool("Leave", true);
        SolarGlare.SetBool("Grab", false);
        button.SetActive(false);
        animationsPlayed = true;
        FadeIn(textCanvas, 1);
        Dialogue2();
    }
}
