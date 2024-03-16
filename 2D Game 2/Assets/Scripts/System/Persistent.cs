using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Persistent : MonoBehaviour
{

    public Image buttonImage;
    public bool persist;

    void Start()
    {
        if (persist)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    void Update()
    {

    }

    public void SetCheatMode(bool enableCheatMode)
    {
        Color buttonColor = buttonImage.color;
        buttonColor.a = enableCheatMode ? 0f : 1f; // 0f for fully transparent, 1f for fully opaque
        buttonImage.color = buttonColor;
        PlayerPrefs.SetInt("CheatMode", enableCheatMode == true ? 1 : 0);
        if (PlayerPrefs.GetInt("CheatMode") == 1)
        {
            Debug.Log("Cheat mode Enabled");
        }
        else
        {
            Debug.Log("Cheat mode Disabled");
        }
    }
}
