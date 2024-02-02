using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private AudioSource source;
    public FadeToBlack fadeToBlack;


    public List<AudioClip> audioClips;
    public AudioClip currentClip;


    void Start()
    {
        source = GetComponent<AudioSource>();
        currentClip = audioClips[Random.Range(0, audioClips.Count)];
    }
    public void Play()
    {
        ButtonSound();
        fadeToBlack.StartFade();
        Invoke("LoadGame", 1);
    }
    public void Exit()
    {
        ButtonSound();
        fadeToBlack.StartFade();
        Invoke("ExitGame", 1);
    }
    public void Settings()
    {
        ButtonSound();
        fadeToBlack.StartFade();
        Invoke("LoadSettings", 1);
    }
    private void LoadGame()
    {
        SceneManager.LoadScene(1);

    }
    private void LoadSettings()
    {
        SceneManager.LoadScene(2);
    }


    private void ExitGame()
    {
        Application.Quit();
    }


    public void ButtonSound()
    {
        currentClip = audioClips[Random.Range(0, audioClips.Count)];
        source.clip = currentClip;
        source.Play();
    }
}
