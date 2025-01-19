using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.Serialization;

public class NumberScript : MonoBehaviour
{
    public AudioSource audioSource; // Reference to the AudioSource component
    public string audioFilePrefix = "Number_Wildcard_Approval Pos_"; // Prefix for the audio files
    public AudioClip hundredClip; // Clip for "hundred"
    public AudioClip thousandClip; // Clip for "thousand"

    public int ConvID;
    public int entryID;

    // Dictionary to preload the audio clips
    private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

    [FormerlySerializedAs("playNumber")] public int killedXTimes;

    private void Start()
    {
        
        // Preload only valid audio clips into the dictionary
        for (int i = 1; i <= 19; i++)
        {
            string fileName = $"Voicelines/Wildcards Positive/{audioFilePrefix}{i:D2}"; // Format as two digits
            AudioClip clip = Resources.Load<AudioClip>(fileName);
            if (clip != null)
            {
                audioClips[fileName] = clip;
            }
        }

        // Preload multiples of 10 (20, 30, ..., 90)
        for (int i = 20; i <= 90; i += 10)
        {
            string fileName = $"Voicelines/Wildcards Positive/{audioFilePrefix}{i}";
            AudioClip clip = Resources.Load<AudioClip>(fileName);
            if (clip != null)
            {
                audioClips[fileName] = clip;
            }
        }

        // Load clips for 100 and 1000
        audioClips[$"Voicelines/Wildcards Positive/{audioFilePrefix}1000"] = thousandClip;
        audioClips[$"Voicelines/Wildcards Positive/{audioFilePrefix}100"] = hundredClip;
    }

    public void PlayNumber(int number)
    {
        if (number <= 0) return;

        // Break down the number and get the sequence
        List<string> audioSequence = GetAudioSequence(number);

        // Play the audio sequence
        StartCoroutine(PlayAudioSequence(audioSequence));
    }

    private List<string> GetAudioSequence(int number)
    {
        List<string> sequence = new List<string>();

        int thousands = number / 1000; // Get the thousands place
        if (thousands > 0)
        {
            sequence.Add($"Voicelines/Wildcards Positive/{audioFilePrefix}{thousands:D2}"); // Format as two digits
            sequence.Add($"Voicelines/Wildcards Positive/{audioFilePrefix}1000");          
            number %= 1000; // Remove the thousands part
        }

        int hundreds = number / 100; // Get the hundreds place
        if (hundreds > 0)
        {
            sequence.Add($"Voicelines/Wildcards Positive/{audioFilePrefix}{hundreds:D2}"); // Format as two digits
            sequence.Add($"Voicelines/Wildcards Positive/{audioFilePrefix}100");          
            number %= 100; // Remove the hundreds part
        }

        int tens = number / 10 * 10; // Get the tens place
        int ones = number % 10;     // Get the ones place

        if (tens > 0 && tens < 20)
        {
            // Handle numbers from 10 to 19 as single words (e.g., "13" -> "thirteen")
            sequence.Add($"Voicelines/Wildcards Positive/{audioFilePrefix}{tens + ones:D2}");
        }
        else
        {
            if (tens > 0)
            {
                sequence.Add($"Voicelines/Wildcards Positive/{audioFilePrefix}{tens:D2}"); // Format as two digits
            }
            if (ones > 0)
            {
                sequence.Add($"Voicelines/Wildcards Positive/{audioFilePrefix}{ones:D2}"); // Format as two digits
            }
        }

        return sequence;
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
        (DialogueManager.dialogueUI as StandardDialogueUI).OnContinue();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayNumber(killedXTimes);
        }
    }

    public void PlaySoundAncContinue()
    {
        DialogueLua.SetVariable("killedTime", killedXTimes);
        PlayNumber(killedXTimes);
        
    }

    public void setKill(int killedTimes)
    {
        //TODO: handle kill, for now its just to test
       
         killedXTimes = killedTimes;
        

    }
}
