using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSounds : MonoBehaviour
{
    public List<AudioClip> audioClips;
    public AudioClip currentClip;
    public AudioSource source;

    void Start()
    {
        source = GetComponent<AudioSource>();
    }


    public void ButtonSound()
    {
        source.Play();
        currentClip = audioClips[Random.Range(0, audioClips.Count)];
        source.clip = currentClip;
    }
}