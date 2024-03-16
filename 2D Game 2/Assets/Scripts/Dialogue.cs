using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using Strobotnik.Klattersynth;
using UnityEngine.SceneManagement;

public class Dialogue : MonoBehaviour
{
    public float clickCooldown = 1;
    public string[] introText;
    public string[] DeathText;

    [Header("Settings")]
    public bool introDialogue = false;
    public bool deathDialogue = false;

    public bool sprinklesMode = false;
    public AudioClip[] sprinklesClip;
    public AudioSource sprinklesSource;


    private int introIndex = 0;
    private int deathIndex = 0;
    private bool cooldown = true;

    private bool introDialogueRunning;
    private bool deathDialogueRunning;

    EnemyController enemyController;
    private Board boardScript;
    private Speech speech;
    private DialogueController dialogueController;
    private CanvasGroup fadeBlack;

    void Start()
    {
        GameObject boardObj = GameObject.FindGameObjectWithTag("Board");
        boardScript = boardObj.GetComponent<Board>();
        GameObject dialogueObj = GameObject.FindGameObjectWithTag("Dialogue Controller");
        dialogueController = dialogueObj.GetComponent<DialogueController>();
        GameObject voice = GameObject.FindGameObjectWithTag("Voice");
        speech = voice.GetComponent<Speech>();
        GameObject fadeBlackObj = GameObject.FindGameObjectWithTag("Fade Black");
        fadeBlack = fadeBlackObj.GetComponent<CanvasGroup>();
        GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
        enemyController = enemy.GetComponent<EnemyController>();
        Dialogue1();
    }
    public void DoDeath()
    {
        if (!deathDialogue)
        {
            return;
        }
        deathDialogueRunning = true;
    }
    public void DoIntro()
    {
        if (!introDialogue)
        {
            return;
        }
        introDialogueRunning = true;
    }

    void Update()
    {
        if (cooldown)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                if (introDialogueRunning)
                {
                    CooldownStart();
                    if (introIndex < introText.Length)
                    {
                        Dialogue1();
                        if (introIndex >= introText.Length)
                        {
                            Debug.Log("Intro Dialogue end");
                            if (enemyController.doAttack)
                            {
                                enemyController.Attack();
                            }
                            boardScript.NewGame();
                        }
                    }
                }
                if (deathDialogueRunning)
                {
                    CooldownStart();
                    if (deathIndex < DeathText.Length)
                    {
                        Dialogue2();
                        if (deathIndex >= DeathText.Length)
                        {
                            Debug.Log("death dialogue end");
                            FadeBlack();
                            StartCoroutine(NextScene());
                        }
                    }
                }
            }
        }
    }
    private void Dialogue1()
    {
        speech.speak(introText[introIndex], false);
        dialogueController.DisplayText(introText[introIndex], false);
        if (sprinklesMode && introIndex == 3)
        {
            sprinklesSource.PlayOneShot(sprinklesClip[0], 0.5f);
        }
        else if (sprinklesMode && introIndex == 4)
        {
            sprinklesSource.PlayOneShot(sprinklesClip[1], 0.5f);
        }
        if (sprinklesMode && introIndex == 5)
        {
            sprinklesSource.PlayOneShot(sprinklesClip[2], 0.5f);
        }
        introIndex++;


    }
    private void Dialogue2()
    {
        speech.speak(DeathText[deathIndex], false);
        dialogueController.DisplayText(DeathText[deathIndex], false);
        deathIndex++;
    }
    public void PlayerDeath()
    {
        speech.speak("You have fallen, so long friend.", false);
        dialogueController.DisplayText("You have fallen, so long friend.", false);
        FadeBlack();
        StartCoroutine(ReloadScene());
    }


    void CooldownStart()
    {
        StartCoroutine(CooldownCoroutine());
    }
    IEnumerator CooldownCoroutine()
    {
        cooldown = false;
        yield return new WaitForSeconds(clickCooldown);
        cooldown = true;
    }
    IEnumerator NextScene()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    IEnumerator ReloadScene()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    private void FadeIn(CanvasGroup canvasGroup, float fadeTime)
    {
        canvasGroup.DOFade(1, fadeTime);
    }
    private void FadeOut(CanvasGroup canvasGroup, float fadeTime)
    {
        canvasGroup.DOFade(0, fadeTime);
    }

    void FadeBlack()
    {
        FadeIn(fadeBlack, 3);
    }
}
