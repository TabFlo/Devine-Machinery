using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.Serialization;

public class NumberScript : MonoBehaviour
{
    public AudioSource audioSource;
    public string positivePrefix = "Numbers_Wildcard_Approval Pos_";
    public string negativePrefix = "Numbers_Wildcard_Approval Neg_";
    public AudioClip hundredClip;
    public AudioClip thousandClip;
    
    
    
    public int number;

    public int ConvID;
    public int entryID;

    private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

    [FormerlySerializedAs("playNumber")] public int killedXTimes;

    private void Start()
    {
        applyKill();
        LoadAudioClips("Positive");
        LoadAudioClips("Negative");
        
        audioClips[$"Voicelines/Wildcards Positive/{positivePrefix}1000"] = thousandClip;
        audioClips[$"Voicelines/Wildcards Positive/{positivePrefix}100"] = hundredClip;
        
        audioClips[$"Voicelines/Wildcards Negative/{negativePrefix}1000"] = thousandClip;
        audioClips[$"Voicelines/Wildcards Negative/{negativePrefix}100"] = hundredClip;
    }

    private void LoadAudioClips(string category)
    {
        string prefix = category == "Positive" ? positivePrefix : negativePrefix;
        string path = $"Voicelines/Wildcards {category}/";
        
        for (int i = 1; i <= 19; i++)
        {
            LoadClip(path, prefix, i);
        }
        
        for (int i = 20; i <= 90; i += 10)
        {
            LoadClip(path, prefix, i);
        }
    }

    private void LoadClip(string path, string prefix, int number)
    {
        string fileName = $"{path}{prefix}{number:D2}";
        AudioClip clip = Resources.Load<AudioClip>(fileName);
        if (clip != null)
        {
            audioClips[fileName] = clip;
        }
    }

    public void PlayNumber()
    {
        if (number <= 0) return;
        List<string> audioSequence = GetAudioSequence(number);
        StartCoroutine(PlayAudioSequence(audioSequence));
    }

    private List<string> GetAudioSequence(int number)
    {
        List<string> sequence = new List<string>();

        int thousands = number / 1000;
        if (thousands > 0)
        {
            sequence.Add(GetRandomPath(thousands));
            sequence.Add(GetRandomPath(1000));
            number %= 1000;
        }

        int hundreds = number / 100;
        if (hundreds > 0)
        {
            sequence.Add(GetRandomPath(hundreds));
            sequence.Add(GetRandomPath(100));
            number %= 100;
        }

        int tens = number / 10 * 10;
        int ones = number % 10;

        if (tens > 0 && tens < 20)
        {
            sequence.Add(GetRandomPath(tens + ones));
        }
        else
        {
            if (tens > 0)
            {
                sequence.Add(GetRandomPath(tens));
            }
            if (ones > 0)
            {
                sequence.Add(GetRandomPath(ones));
            }
        }

        return sequence;
    }

    private string GetRandomPath(int value)
    {
        string category = Random.value > 0.5f ? "Positive" : "Negative";
        string prefix = category == "Positive" ? positivePrefix : negativePrefix;
        return $"Voicelines/Wildcards {category}/{prefix}{value:D2}";
    }

    private IEnumerator PlayAudioSequence(List<string> audioSequence)
    {
        foreach (string audioFileName in audioSequence)
        {
            if (audioClips.TryGetValue(audioFileName, out AudioClip clip))
            {
                audioSource.clip = clip;
                audioSource.Play();
                yield return new WaitForSeconds(clip.length);
            }
            else
            {
                Debug.LogWarning("Audio clip not found: " + audioFileName);
            }
        }
        (DialogueManager.dialogueUI as StandardDialogueUI)?.OnContinue();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayNumber();
            Debug.Log(number);
        }
        number = DialogueLua.GetVariable("killedTime").AsInt;
    }


    public void saveKill()
    {
        PlayerPrefs.SetInt("kill", number);
        PlayerPrefs.Save();
        
        
        
    }

    public void applyKill()
    {
        
        number = PlayerPrefs.GetInt("kill", number);
        number = DialogueLua.GetVariable("killedTime").AsInt;
        Debug.LogWarning(number);
    }
}
