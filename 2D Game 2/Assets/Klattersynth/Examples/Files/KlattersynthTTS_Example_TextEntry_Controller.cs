using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Strobotnik.Klattersynth;

namespace Strobotnik.Klattersynth.Examples {

public class KlattersynthTTS_Example_TextEntry_Controller : MonoBehaviour
{
    public Text baseFreqHzLabel;
    public Speech[] voices;

    Speech activeVoice;
    int activeVoiceNum;
    int voiceBaseFrequency;
    SpeechSynth.VoicingSource voicingSource;

    public delegate void SpeakEventDelegate(string text);
    public SpeakEventDelegate speakEvent;

    public int getActiveVoiceNum()
    {
        return activeVoiceNum;
    }

    public int getVoiceBaseFrequency()
    {
        return voiceBaseFrequency;
    }

    public SpeechSynth.VoicingSource getVoicingSource()
    {
        return voicingSource;
    }

    public void setVoice(int num)
    {
        if (num >= 0 && num < voices.Length)
        {
            activeVoice = voices[num];
            activeVoiceNum = num;
        }
        else
            Debug.LogWarning("Invalid voice: " + num, this);
    }

    public void setVoiceBaseFrequency(float hz)
    {
        baseFreqHzLabel.text = hz.ToString() + " Hz";
        voiceBaseFrequency = (int)hz;
    }

    public void setVoicingSource(int num)
    {
        voicingSource = (SpeechSynth.VoicingSource)num;
    }

    public void speak(string text)
    {
        speakEvent(text);
    }

    void doSpeak(string text)
    {
        activeVoice.voiceBaseFrequency = voiceBaseFrequency;
        activeVoice.voicingSource = voicingSource;
        activeVoice.speak(text);
        Debug.Log("Phonemes: " + activeVoice.getPhonemes());
    }

    public void stop()
    {
        for (int a = 0; a < voices.Length; ++a)
            voices[a].stop();
    }

    void Init()
    {
        voiceBaseFrequency = 220;
        voicingSource = SpeechSynth.VoicingSource.natural;
        speakEvent += doSpeak;
        if (voices.Length > 0)
            setVoice(0);
        else
        {
            Debug.LogError("Empty voices array!", this);
            gameObject.SetActive(false);
            return;
        }
    }

    void Start()
    {
        Init();
    }
}

} // namespace
