using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Strobotnik.Klattersynth;
using UnityEngine.SceneManagement;
using Newtonsoft.Json.Linq;

public class Outro : MonoBehaviour
{
    public float clickCooldown = 1;
    public int fadeTime = 2;


    public string[] texts1;
    public string[] texts2;

    private int currentIndex1 = 0;
    public int currentIndex2 = 0;

    private bool cooldown = true;
    private bool dialogueBegun = false;
    private bool animationsPlayed = false;

    private bool isFaded = false;

    public DialogueController jerryDialogueController;
    public CanvasGroup JerryTextCanvas;

    public CanvasGroup jerry;
    public CanvasGroup canvasCanvas;
    public CanvasGroup fadeBlack;
    public CanvasGroup textCanvas;
    public CanvasGroup bjan;
    public Speech speech;
    public Speech jerrySpeech;


    public DialogueController dialogueController;
    private Board boardScript;
    private Persistent persistent;
    private GameObject canvas;

    void Start()
    {
        GameObject boardObj = GameObject.FindGameObjectWithTag("Board");
        boardScript = boardObj.GetComponent<Board>();
        GameObject persistentObj = GameObject.FindGameObjectWithTag("Persistent");
        persistent = persistentObj.GetComponent<Persistent>();
        canvas = GameObject.FindGameObjectWithTag("Canvas");

        StartScene();
        StartCoroutine(DelayedStart());
    }
    void Update()
    {
        if (cooldown && dialogueBegun)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                CooldownStart();
                if (currentIndex1 < texts1.Length)
                {
                    Dialogue();
                    if (currentIndex1 >= texts1.Length)
                    {
                        Debug.Log("Dialogue 1 end");
                        Debug.Log("Play animation here");
                        FadeOut(jerry, 0);
                        FadeIn(jerry, 2);
                        FadeOut(textCanvas, 1);
                        StartCoroutine(AnimationWait());
                    }
                }
                if (animationsPlayed && currentIndex2 < texts2.Length)
                {
                    Dialogue2();
                    if (currentIndex2 >= texts2.Length)
                    {
                        Debug.Log("Dialogue end");
                        FadeIn(fadeBlack, 3);
                        StartCoroutine(NextScene());
                    }
                }
            }
        }
    }

    private void Dialogue()
    {
        speech.speak(texts1[currentIndex1], false);
        dialogueController.DisplayText(texts1[currentIndex1], false);
        currentIndex1++;
    }

    private void Dialogue2()
    {
        if (currentIndex2 == 1 || currentIndex2 == 3)
        {
            jerrySpeech.speak(texts2[currentIndex2], false);
            jerryDialogueController.DisplayText(texts2[currentIndex2], false);
            currentIndex2++;
        }
        else
        {
            speech.speak(texts2[currentIndex2], false);
            dialogueController.DisplayText(texts2[currentIndex2], false);
            currentIndex2++;
        }

    }


    private void StartScene()
    {
        FadeOut(JerryTextCanvas, 0);
        FadeOut(jerry, 0);
        FadeOut(bjan, 0);
        FadeOut(textCanvas, 0);
        FadeOut(canvasCanvas, 0);
    }

    private void FadeIn(CanvasGroup canvasGroup, float fadeTime)
    {
        canvasGroup.DOFade(1, fadeTime);
    }
    private void FadeOut(CanvasGroup canvasGroup, float fadeTime)
    {
        canvasGroup.DOFade(0, fadeTime);
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
        cooldown = false;
        yield return new WaitForSeconds(5);
        animationsPlayed = true;
        cooldown = true;
        FadeIn(textCanvas, 1);
        FadeIn(JerryTextCanvas, 1);
        Dialogue2();
    }
    IEnumerator NextScene()
    {
        yield return new WaitForSeconds(3.5f);
        SceneManager.LoadScene(0);
    }
}
