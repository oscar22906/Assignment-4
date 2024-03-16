using Strobotnik.Klattersynth;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class test : MonoBehaviour
{

    public string[] texts;
    public float clickCooldown = 1;

    private int currentIndex = 0;
    private bool cooldown = true;
    private bool dialogueBegun = false;

    void Start()
    {
        
    }


    void Update()
    {
        if (cooldown && dialogueBegun)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                CooldownStart();
                if (currentIndex < texts.Length)
                {
                    Dialogue();
                    if (currentIndex >= texts.Length)
                    {
                        Debug.Log("Dialogue 1 end");
                    }
                }
            }
        }
    }
    private void Dialogue()
    {
        // set text
        currentIndex++;
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
}
