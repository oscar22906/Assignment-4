using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEffects : MonoBehaviour
{
    public GameObject mainEffectObject;
    public GameObject damageEffectObject;
    Animator mainEffectAnimator;
    Animator DamageEffectAnimator;
    Animator animator;

    void Start()
    {
        mainEffectAnimator = mainEffectObject.GetComponent<Animator>();
        DamageEffectAnimator = damageEffectObject.GetComponent<Animator>();
    }

    public void DamageUpdate(float healthPercentage)
    {
        mainEffectAnimator.SetFloat("Blend", 1 - healthPercentage);

        DamageEffectAnimator.SetTrigger("Damage");
    }
}
