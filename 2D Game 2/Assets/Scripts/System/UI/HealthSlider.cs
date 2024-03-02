using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSlider : MonoBehaviour
{
    public Slider healthSlider;
    public Slider easeHealthSlider;
    public float maxHealth = 100f;
    public float health;
    public float lerpSpeed = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (healthSlider.value != health)
        {
            healthSlider.value = health / 100;
        }

        /* // testing damage 
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(10);
        }
        */

        if (healthSlider.value != easeHealthSlider.value)
        {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, health / 100, lerpSpeed);

        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
    }
}
