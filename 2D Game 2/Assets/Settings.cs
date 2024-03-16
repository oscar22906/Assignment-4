using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;

public class Settings : MonoBehaviour
{
    public InputField inputField;

    public void GoBack()
    {
        SceneManager.LoadScene(0);
    }



    public void TryLoadScene()
    {
        string text = inputField.text;

        int level;
        if (int.TryParse(text, out level))
        {
            if (level >= 0 && level < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(level);
            }
            else
            {
                Debug.LogError("Scene index is outside the bounds of available scenes.");
            }
        }
        else
        {
            Debug.LogError("Invalid input. Please enter a valid integer.");
        }
    }
}
