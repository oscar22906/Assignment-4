using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Sprinkles : MonoBehaviour
{
    public SprinklesGun sprinklesGun;
    public AudioSource audioSource;
    public AudioClip[] clips;

    void Start()
    {
        
    }

    public void StartAttack()
    {
        sprinklesGun.Shoot();
        PlayAudio();
    }
    void PlayAudio()
    {
        int randomInt = Random.Range(0, clips.Length - 1);
        audioSource.PlayOneShot(clips[randomInt], 0.5f);
    }
}
