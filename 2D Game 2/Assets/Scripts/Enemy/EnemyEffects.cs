using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEffects : MonoBehaviour
{
    public GameObject mainEffectObject;
    public GameObject damageEffectObject;
    public GameObject damageEffectObject2;
    Animator mainEffectAnimator;
    Animator damageEffectAnimator;
    Animator damageEffectAnimator2;
    private AnimationClip[] clips;
    private AnimationClip[] clips2;
    public bool playDamageRandomClip;
    public bool UseSecondAnimator;

    void Start()
    {
        mainEffectAnimator = mainEffectObject.GetComponent<Animator>();
        damageEffectAnimator = damageEffectObject.GetComponent<Animator>();
        if (UseSecondAnimator)
        {
            SecondAnimator();
        }

        clips = damageEffectAnimator.runtimeAnimatorController.animationClips;
    }

    void SecondAnimator()
    {
        damageEffectAnimator2 = damageEffectObject2.GetComponent<Animator>();
        clips2 = damageEffectAnimator2.runtimeAnimatorController.animationClips;
    }

    public void DamageUpdate(float healthPercentage)
    {
        mainEffectAnimator.SetFloat("Blend", 1 - healthPercentage);

        damageEffectAnimator.SetTrigger("Damage");
        if (playDamageRandomClip)
        {
            StartCoroutine(PlayRandomly());
            if (UseSecondAnimator)
            {
                StartCoroutine(PlayRandomly2());
            }
        }
    }
    private IEnumerator PlayRandomly()
    {
        var randInd = Random.Range(0, clips.Length);

        var randClip = clips[randInd];

        damageEffectAnimator.Play(randClip.name);

        yield return new WaitForSeconds(1);
    }
    private IEnumerator PlayRandomly2()
    {
        var randInd = Random.Range(0, clips2.Length);

        var randClip = clips2[randInd];

        damageEffectAnimator2.Play(randClip.name);

        yield return new WaitForSeconds(1);
    }


}
