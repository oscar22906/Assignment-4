using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Persistent : MonoBehaviour
{

    public string playerName;
    public int currentLevel;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        
    }
}
