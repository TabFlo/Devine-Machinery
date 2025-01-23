using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlitchManager : MonoBehaviour
{
    public Kino.AnalogGlitch glitchEffect; // Reference to the AnalogGlitch component
    private float lerpSpeed = 10f; // Speed at which parameters lerp

    void Start()
    {
        // Ensure the glitchEffect reference is assigned
        if (glitchEffect == null)
        {
            Debug.LogError("AnalogGlitch reference is not set! Please assign it in the inspector.");
        }
    }

    void Update()
    {
        // Gradually reset parameters to zero
        if (glitchEffect != null)
        {
            glitchEffect.scanLineJitter = Mathf.Lerp(glitchEffect.scanLineJitter, 0f, Time.deltaTime * lerpSpeed);
            glitchEffect.colorDrift = Mathf.Lerp(glitchEffect.colorDrift, 0f, Time.deltaTime * lerpSpeed);
        }
    }

    // Method to handle messages
    public void ApplyGlitchEffect(string parameter)
    {
        Debug.Log("gotMessage");
        if (glitchEffect == null) return;

        // Parse the parameter and adjust glitch settings
        switch (parameter.ToLower())
        {
            case "intense":
                glitchEffect.scanLineJitter = Random.Range(0.5f, 1f);
                glitchEffect.colorDrift = Random.Range(0.5f, 1f);
                break;

            case "mild":
                glitchEffect.scanLineJitter = Random.Range(0.1f, 0.3f);
                glitchEffect.colorDrift = Random.Range(0.1f, 0.3f);
                break;

            default:
                Debug.LogWarning($"Unknown glitch parameter: {parameter}");
                break;
        }
    }
}