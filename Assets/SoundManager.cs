using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioSource audioSourceA;
    [SerializeField] private AudioSource audioSourceB;
    private AudioSource activeSource;
    private AudioSource inactiveSource;

    private Dictionary<int, AudioClip> ambienceClips = new Dictionary<int, AudioClip>();
    private int currentAppro;
    private float fadeDuration = 1f;
    private float maxVolume = 0.3f; // Cap both sources at 50%

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Load audio clips into dictionary
        for (int i = -3; i <= 3; i++)
        {
            string path = $"Ambience Tracks/Ambience_Approval{i:+#;-#;0}";
            AudioClip clip = Resources.Load<AudioClip>(path);
            if (clip != null)
            {
                ambienceClips[i] = clip;
            }
            else
            {
                Debug.LogError($"Missing audio clip at: {path}");
            }
        }

        activeSource = audioSourceA;
        inactiveSource = audioSourceB;

        // Set volume caps
        audioSourceA.volume = maxVolume;
        audioSourceB.volume = 0f; // Start with one muted
    }

    private void Start()
    {
        currentAppro = TouchCheckScript.appro; // Get current approval value
        PlayAmbience(currentAppro); // Start with the correct track
    }

    public void UpdateApprovalSound(int newAppro)
    {
        if (newAppro == currentAppro || !ambienceClips.ContainsKey(newAppro))
            return;  // No change or missing clip

        StartCoroutine(SwitchTrack(newAppro));
    }

    private IEnumerator SwitchTrack(int newAppro)
    {
        float currentTime = activeSource.time;  // Save playback position
        currentAppro = newAppro;
        AudioClip newClip = ambienceClips[newAppro];

        Debug.Log($"Switching track to: {newClip.name} at time {currentTime}");

        inactiveSource.clip = newClip;
        inactiveSource.time = currentTime;
        inactiveSource.Play();

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float t = elapsedTime / fadeDuration;
            activeSource.volume = Mathf.Lerp(maxVolume, 0f, t);
            inactiveSource.volume = Mathf.Lerp(0f, maxVolume, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        activeSource.volume = 0f;
        inactiveSource.volume = maxVolume;

        Debug.Log($"Now playing: {inactiveSource.clip.name}");

        // Swap sources
        (activeSource, inactiveSource) = (inactiveSource, activeSource);
        inactiveSource.Stop();
    }

    private void PlayAmbience(int approValue)
    {
        if (!ambienceClips.ContainsKey(approValue)) return;

        activeSource.clip = ambienceClips[approValue];
        activeSource.loop = true;
        activeSource.volume = 0f;
        activeSource.Play();
        StartCoroutine(FadeIn(activeSource));
        Debug.Log($"Playing ambience: {activeSource.clip.name}");
    }

    // Stop all ambience with fade-out
    public void StopAllSounds()
    {
        StartCoroutine(FadeOutAndStop(activeSource));
        StartCoroutine(FadeOutAndStop(inactiveSource));
        Debug.Log("All ambience stopping.");
    }

    // Restart ambience at the correct approval and fade-in
    public void RestartAmbience()
    {
        StopAllSounds(); // Ensure both are stopped
        currentAppro = TouchCheckScript.appro;
        StartCoroutine(DelayedRestart(currentAppro));
    }

    private IEnumerator DelayedRestart(int approValue)
    {
        yield return new WaitForSeconds(fadeDuration);
        PlayAmbience(approValue);
        Debug.Log($"Restarting ambience for Approval: {approValue}");
    }

    private IEnumerator FadeOutAndStop(AudioSource source)
    {
        float startVolume = source.volume;
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            source.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        source.volume = 0f;
        source.Stop();
    }

    private IEnumerator FadeIn(AudioSource source)
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            source.volume = Mathf.Lerp(0f, maxVolume, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        source.volume = maxVolume;
    }
}
