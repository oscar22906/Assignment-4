using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadData : MonoBehaviour
{


    void Start()
    {
        
    }

    public string[] LoadSolutions(int currentLevel)
    {
        // load solutions (eg. 1)
        TextAsset textFile = Resources.Load(currentLevel.ToString()) as TextAsset;
        return textFile.text.Split("\n");
    }

    public string[] LoadFailPhrase(int currentLevel)
    {

        // load fail phrase (eg 1FailPhrase)
        TextAsset failPhraseFile = Resources.Load(currentLevel.ToString() + "FailPhrase") as TextAsset;
        return failPhraseFile.text.Split("\n");
    }

    public string[] LoadSuccessPhrase(int currentLevel)
    {
        // load success phrase (eg 1SuccessPhrase)
        TextAsset successPhraseFile = Resources.Load(currentLevel.ToString() + "SuccessPhrase") as TextAsset;
        return successPhraseFile.text.Split("\n");
    }
}
