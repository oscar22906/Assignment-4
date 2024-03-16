using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Louis : MonoBehaviour
{

    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Incorrect()
    {
        Debug.Log("Louis mode - Incorrect");
        animator.SetTrigger("Incorrect");
    }
    public void Correct()
    {
        int num = Random.Range(1, 2);
        animator.SetTrigger("Correct"+num);
        Debug.Log("Louis mode - Correct" + num);
    }
}
