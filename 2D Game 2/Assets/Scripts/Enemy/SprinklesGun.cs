using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprinklesGun : MonoBehaviour
{
    Animator animator;
    public SprinklesBag sprinklesBag;
    public Animator screen;
    public Animator splish;
    public AudioClip splishClip;
    public AudioClip Splashclip;
    public AudioSource source;
    public EnemyController EnemyController;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Shoot()
    {
        animator.SetTrigger("Shoot");
    }

    public void Hit()
    {
        StartCoroutine(DelayedHit());
    }
    public void Splish()
    {
        splish.SetTrigger("Damage");
        source.PlayOneShot(splishClip, 0.4f);
    }
    public void Splash()
    {
        screen.SetTrigger("Damage");
        source.PlayOneShot(Splashclip, 0.4f);
        EnemyController.DealDamage();
    }
    IEnumerator DelayedHit()
    {
        yield return new WaitForSeconds(1.5f);
        sprinklesBag.Hit();
    }
}
