using System.Collections;
using System.Collections.Generic;
using Meta.WitAi.TTS.Utilities;
using UnityEngine;

public class VSTextToSpeechControl : MonoBehaviour
{
    public TTSSpeaker speaker;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SpeakText(string text)
    {
        speaker.Speak(text);
    }
}
