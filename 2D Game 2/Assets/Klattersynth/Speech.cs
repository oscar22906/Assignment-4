/// Klattersynth TTS
/// Copyright 2017 Strobotnik Ltd
/// www.strobotnik.com

using System.Text;
using System.Collections.Generic;
using UnityEngine;
using Strobotnik.Klattersynth;


namespace Strobotnik.Klattersynth
{
    public class Speech : MonoBehaviour
    {
        [Tooltip("When true, speech is real-time generated (played using a single small looping audio clip).\n\nNOTE: Not supported with WebGL, will be auto-disabled in Start() when running in WebGL!")]
#if UNITY_WEBGL
        public bool useStreamingMode = false;
#else
        public bool useStreamingMode = true;
#endif
        public DialogueController dialogueController;

        [Tooltip("Maximum amount of speech clips to automatically cache in non-streaming mode.\n(Least recently used are discarded when going over this amount.)")]
        public int maxAutoCachedClips = 10;

        [Tooltip("Base frequency for the synthesized voice.\nCan be runtime-adjusted.")]
        public int voiceBaseFrequency = 220;

        [Tooltip("Type of \"voicing source\".\nCan be runtime-adjusted.")]
        public SpeechSynth.VoicingSource voicingSource = SpeechSynth.VoicingSource.natural;

        [Tooltip("How many milliseconds to use per one \"speech frame\".")]
        [Range(1, 100)]
        public int msPerSpeechFrame = 10;

        [Tooltip("Amount of flutter in voice.")]
        [Range(0, 200)]
        public int flutter = 10;

        [Tooltip("Speed of the flutter.")]
        [Range(0.001f, 100.0f)]
        public float flutterSpeed = 1.0f;

        struct ScheduledUnit
        {
            public string text;
            public int voiceBaseFrequency;
            public SpeechSynth.VoicingSource voicingSource;
            public bool bracketsAsPhonemes;
            public SpeechClip pregenClip;
        };

        const int sampleRate = 11025;

        bool talking;
        AudioSource audioSrc;
        SpeechSynth speechSynth;
        StringBuilder speakSB;
        List<SpeechClip> cachedSpeechClips;
        List<ScheduledUnit> scheduled = new List<ScheduledUnit>(5);


        bool errCheck(bool errorWhenTrue, string logErrorString)
        {
            if (errorWhenTrue)
            {
                if (logErrorString != null)
                    Debug.LogError(logErrorString, this);
                return true; // true = we're in error situation
            }
            return false; // false = Ok
        }

        // Caches a speech clip.
        // Mainly meant to be used in non-streaming mode.
        void cache(SpeechClip sc)
        {
            if (maxAutoCachedClips <= 0)
                return;
            if (cachedSpeechClips == null)
                cachedSpeechClips = new List<SpeechClip>(maxAutoCachedClips);
            else
            {
                int existingIdx = cachedSpeechClips.FindIndex(x => x.hash == sc.hash);
                if (existingIdx >= 0)
                {
                    // already in cache?! replace older one with this one
                    cachedSpeechClips[existingIdx] = sc;
                    return;
                }
                if (cachedSpeechClips.Count >= maxAutoCachedClips)
                    cachedSpeechClips.RemoveRange(0, cachedSpeechClips.Count - (maxAutoCachedClips - 1));
            }
            cachedSpeechClips.Add(sc);
        }

        // Returns a matching speech clip from cache, or null if not found.
        // When clip was found, it's also moved to end of cache list (makes it most recently used).
        // Mainly meant to be used in non-streaming mode.
        SpeechClip findFromCache(StringBuilder text, int freq, SpeechSynth.VoicingSource voicingSrc, bool bracketsAsPhonemes)
        {
            if (cachedSpeechClips == null)
                return null;
            ulong hash = SpeechSynth.makeHashCode(text, freq, voicingSrc, bracketsAsPhonemes);
            int idx = cachedSpeechClips.FindIndex(x => x.hash == hash);
            if (idx < 0)
                return null;
            SpeechClip sc = cachedSpeechClips[idx];
            if (idx < cachedSpeechClips.Count - 1)
            {
                cachedSpeechClips.RemoveAt(idx);
                cachedSpeechClips.Add(sc);
            }
            return sc;
        }

        void speakNextScheduled()
        {
            if (scheduled == null || scheduled.Count == 0)
                return;
            ScheduledUnit su = scheduled[0];
            scheduled.RemoveAt(0);
            if (su.pregenClip != null)
                speak(su.pregenClip);
            else
                speak(su.voiceBaseFrequency, su.voicingSource, su.text, su.bracketsAsPhonemes);
            
        }


        // Returns true when speech is being played.
        public bool isTalking()
        {
            return talking;
        }

        // Returns the current loudness level of speech, calculated from a short range of samples.
        public float getCurrentLoudness()
        {
            return speechSynth.getCurrentLoudness();
        }

        // Returns phonemes translated and used for the last call to speak() or pregenerate().
        public string getPhonemes()
        {
            return speechSynth.getPhonemes();
        }

        // Plays a pre-generated speech clip.
        public void speak(SpeechClip pregenSpeech)
        {
            speechSynth.speak(pregenSpeech);
            talking = true;
        }

        // Speaks given text. If bracketsAsPhonemes is true, text inside brackets [...] will be considered phonemes "as is".
        public void speak(string text, bool bracketsAsPhonemes = false)
        {
            speak(voiceBaseFrequency, voicingSource, text, bracketsAsPhonemes);
        }

        // Speaks given text with given voice base frequency and voicingSource.
        // If bracketsAsPhonemes is true, text inside brackets [...] will be considered phonemes "as is".
        // In non-streaming mode this will also automatically use cached clips or pregenerate and add to cache as needed.
        public void speak(int voiceBaseFrequency, SpeechSynth.VoicingSource voicingSource,
                          string text, bool bracketsAsPhonemes = false)
        {
            if (errCheck(text == null, "null text"))
                return;
            if (speakSB == null)
                speakSB = new StringBuilder(text.Length * 3 / 2);
            speakSB.Length = 0;
            speakSB.Append(text);
            speak(voiceBaseFrequency, voicingSource, speakSB, bracketsAsPhonemes);
        }

        // Speaks given text. If bracketsAsPhonemes is true, text inside brackets [...] will be considered phonemes "as is".
        // In non-streaming mode this will also automatically use cached clips or pregenerate and add to cache as needed.
        public void speak(StringBuilder text, bool bracketsAsPhonemes = false)
        {
            speak(voiceBaseFrequency, voicingSource, text, bracketsAsPhonemes);
        }

        // Speaks given text with given voice base frequency and voicingSource.
        // If bracketsAsPhonemes is true, text inside brackets [...] will be considered phonemes "as is".
        // In non-streaming mode this will also automatically use cached clips or pregenerate and add to cache as needed.
        public void speak(int voiceBaseFrequency, SpeechSynth.VoicingSource voicingSource,
                          StringBuilder text, bool bracketsAsPhonemes = false)
        {
            if (errCheck(text == null, "null text (SB)"))
                return;
            if (!useStreamingMode)
            {
                SpeechClip sc = findFromCache(text, voiceBaseFrequency, voicingSource, bracketsAsPhonemes);
                if (sc == null)
                    pregenerate(out sc, voiceBaseFrequency, voicingSource, text, bracketsAsPhonemes, true);
                if (sc != null)
                {
                    talking = true;
                    speechSynth.speak(sc);
                }
            }
            else
            {
                talking = true;
                speechSynth.speak(text, voiceBaseFrequency, voicingSource, bracketsAsPhonemes);
            }
        }

        // Pregenerates given text and caches the result, so that cached clip is played when asked to speak() the same text.
        // If bracketsAsPhonemes is true, text inside brackets [...] will be considered phonemes "as is".
        // Mainly meant to be used in non-streaming mode.
        public void pregenerate(string text, bool bracketsAsPhonemes = false)
        {
            SpeechClip sc;
            pregenerate(out sc, text, bracketsAsPhonemes, true);
        }

        // Pregenerates given text to a given speech clip reference.
        // If bracketsAsPhonemes is true, text inside brackets [...] will be considered phonemes "as is".
        // Mainly meant to be used in non-streaming mode.
        public void pregenerate(out SpeechClip speechClip, string text, bool bracketsAsPhonemes = false, bool addToCache = false)
        {
            speechClip = null;
            if (errCheck(text == null, "null text"))
                return;
            if (speakSB == null)
                speakSB = new StringBuilder(text.Length * 3 / 2);
            speakSB.Length = 0;
            speakSB.Append(text);
            pregenerate(out speechClip, speakSB, bracketsAsPhonemes, addToCache);
        }

        // Pregenerates given text to a given speech clip reference.
        // If bracketsAsPhonemes is true, text inside brackets [...] will be considered phonemes "as is".
        // Mainly meant to be used in non-streaming mode.
        public void pregenerate(out SpeechClip speechClip, StringBuilder text, bool bracketsAsPhonemes = false, bool addToCache = false)
        {
            pregenerate(out speechClip, voiceBaseFrequency, voicingSource, text, bracketsAsPhonemes, addToCache);
        }

        // Pregenerates given text, with given voice base frequency and voicingSource, to a given speech clip reference.
        // If bracketsAsPhonemes is true, text inside brackets [...] will be considered phonemes "as is".
        // Mainly meant to be used in non-streaming mode.
        public void pregenerate(out SpeechClip speechClip, int voiceBaseFrequency, SpeechSynth.VoicingSource voicingSource,
                                StringBuilder text, bool bracketsAsPhonemes = false, bool addToCache = false)
        {
            speechClip = null;
            if (errCheck(text == null, "null text (SB)"))
                return;
            speechClip = speechSynth.pregenerate(text, voiceBaseFrequency, voicingSource, bracketsAsPhonemes, true);
            if (speechClip != null && addToCache)
                cache(speechClip);
        }

        // Schedules a pre-generated speech clip to be played when current one ends.
        public void schedule(SpeechClip speechClip)
        {
            ScheduledUnit su = new ScheduledUnit();
            su.pregenClip = speechClip;
        }

        // Schedules given text to speak when current one ends.
        // If bracketsAsPhonemes is true, text inside brackets [...] will be considered phonemes "as is".
        public void schedule(string text, bool bracketsAsPhonemes = false)
        {
            schedule(voiceBaseFrequency, voicingSource, text, bracketsAsPhonemes);
        }

        // Schedules given text to speak when current one ends.
        // If bracketsAsPhonemes is true, text inside brackets [...] will be considered phonemes "as is".
        public void schedule(StringBuilder text, bool bracketsAsPhonemes = false)
        {
            schedule(voiceBaseFrequency, voicingSource, text, bracketsAsPhonemes);
        }

        // Schedules given text to speak when current one ends, with given voice base frequency and voicingSource.
        // If bracketsAsPhonemes is true, text inside brackets [...] will be considered phonemes "as is".
        // In non-streaming mode this will also automatically use cached clips or pregenerate and add to cache as needed.
        public void schedule(int voiceBaseFrequency, SpeechSynth.VoicingSource voicingSource,
                             string text, bool bracketsAsPhonemes = false)
        {
            if (!talking && scheduled.Count == 0)
            {
                speak(voiceBaseFrequency, voicingSource, text, bracketsAsPhonemes);
            }
            else
            {
                ScheduledUnit su = new ScheduledUnit();
                su.voiceBaseFrequency = voiceBaseFrequency;
                su.voicingSource = voicingSource;
                su.text = text;
                su.bracketsAsPhonemes = bracketsAsPhonemes;
                scheduled.Add(su);
            }
        }

        // Schedules given text to speak when current one ends, with given voice base frequency and voicingSource.
        // If bracketsAsPhonemes is true, text inside brackets [...] will be considered phonemes "as is".
        // In non-streaming mode this will also automatically use cached clips or pregenerate and add to cache as needed.
        public void schedule(int voiceBaseFrequency, SpeechSynth.VoicingSource voicingSource,
                             StringBuilder text, bool bracketsAsPhonemes = false)
        {
            if (!talking && scheduled.Count == 0)
                speak(voiceBaseFrequency, voicingSource, text, bracketsAsPhonemes);
            else
                schedule(voiceBaseFrequency, voicingSource, text.ToString(), bracketsAsPhonemes);
        }


        // Stops speaking.
        // If allScheduled is false, then only current one is stopped and next potentially scheduled will start.
        // If allScheduled is true, speaking is stopped and all scheduled talk is erased.
        public void stop(bool allScheduled = false)
        {
            if (allScheduled)
                scheduled.Clear();
            speechSynth.stop();
        }


        void Awake()
        {
#if UNITY_WEBGL
#if UNITY_EDITOR
            if (useStreamingMode)
                Debug.LogWarning("Speech with useStreamingMode=true is NOT supported in WebGL builds. Disabled also in editor for testing purposes.");
#endif
            useStreamingMode = false;
#endif

            audioSrc = GetComponent<AudioSource>();
            if (audioSrc == null)
            {
                audioSrc = gameObject.AddComponent<AudioSource>();
                audioSrc.priority = 1;
            }

            speechSynth = new SpeechSynth();
            speechSynth.init(audioSrc, useStreamingMode, sampleRate, msPerSpeechFrame, flutter, flutterSpeed);
        }

        void Update()
        {
            talking = speechSynth.update();
            if (!talking)
                speakNextScheduled();
        }
    }
}
