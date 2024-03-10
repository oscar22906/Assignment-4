using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroHand : MonoBehaviour
{
    public AudioClip[] clips;
    public AudioClip[] clipsRandom;
    public AudioSource source;
    public Animator _animator;

    void Start()
    {
        
    }

    public void PlayClip(int clipNum)
    {
        source.clip = clips[clipNum];
        source.Play();
        _animator.SetTrigger(clipNum.ToString());
    }
    public void RandomClip()
    {
        int num = Random.Range(0, clips.Length - 1);
        source.clip = clipsRandom[num];
        source.Play();
        _animator.SetTrigger(num.ToString());
    }
}
