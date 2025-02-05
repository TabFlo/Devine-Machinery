using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlitchManager : MonoBehaviour
{
    public Kino.AnalogGlitch glitchEffect; // Reference to the AnalogGlitch component
    private float lerpSpeed = 10f; // Speed at which parameters lerp
    public TouchCheckScript touchCheckScript; // Reference to TouchCheckScript
    public SerialManager serialManager;

    public bool ledUpdateDue;
    [SerializeField] private Color[] eyeColors = new Color[7];
    // Reference to SerialManager

    void Start()
    {
        // Ensure the glitchEffect reference is assigned
        if (glitchEffect == null)
        {
            Debug.LogError("AnalogGlitch reference is not set! Please assign it in the inspector.");
        }
        if (serialManager == null)
        {
            Debug.LogError("SerialManager reference is not set! Please assign it in the inspector.");
        }
    }

    void Update()
    {
        // Gradually reset parameters to zero
        if (glitchEffect != null)
        {
            glitchEffect.scanLineJitter = Mathf.Lerp(glitchEffect.scanLineJitter, 0f, Time.deltaTime * lerpSpeed*Random.Range(0.1f, 2f));
            glitchEffect.colorDrift = Mathf.Lerp(glitchEffect.colorDrift, 0f, Time.deltaTime * lerpSpeed);
            glitchEffect.horizontalShake = Mathf.Lerp(glitchEffect.horizontalShake, 0f, Time.deltaTime * lerpSpeed);
            glitchEffect.verticalJump = Mathf.Lerp(glitchEffect.verticalJump, 0f, Time.deltaTime * lerpSpeed*Random.Range(0.3f, 2f));
        }

        // Update LED colors based on the approval value
        if (ledUpdateDue)
        {
            UpdateLEDColor();
            
        }
    }

    void UpdateLEDColor()
    {
        if (touchCheckScript != null && serialManager.canSend())
        {
            int appro = TouchCheckScript.appro; // Get approval value
            SendColorAccordingToAppro(appro); // Update LED colors
            ledUpdateDue = false;
        }
    }

    // Method to handle glitch effects
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
            case "insane":
                glitchEffect.scanLineJitter = Random.Range(0.7f, 1f);
                glitchEffect.colorDrift = Random.Range(0.8f, 1f);
                break;
            case "crazy":
                glitchEffect.scanLineJitter = Random.Range(0.8f, 1f);
                glitchEffect.colorDrift = Random.Range(0.9f, 1f);
                glitchEffect.horizontalShake = Random.Range(0.7f, 1f);
                glitchEffect.verticalJump = Random.Range(0.8f, 1f);

                //StartCoroutine(ResetGlitchAfterDuration(2f));
                break;

            default:
                Debug.LogWarning($"Unknown glitch parameter: {parameter}");
                break;
        }
    }
    
    private IEnumerator ResetGlitchAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
    
        while (glitchEffect.scanLineJitter > 0.1f || 
               glitchEffect.colorDrift > 0.1f || 
               glitchEffect.horizontalShake > 0.1f || 
               glitchEffect.verticalJump > 0.1f)
        {
            glitchEffect.scanLineJitter = Mathf.Lerp(glitchEffect.scanLineJitter, 0f, Time.deltaTime * 3f);
            glitchEffect.colorDrift = Mathf.Lerp(glitchEffect.colorDrift, 0f, Time.deltaTime * 3f);
            glitchEffect.horizontalShake = Mathf.Lerp(glitchEffect.horizontalShake, 0f, Time.deltaTime * 3f);
            glitchEffect.verticalJump = Mathf.Lerp(glitchEffect.verticalJump, 0f, Time.deltaTime * 3f);
            yield return null;
        }
    }


    // Method to send color data based on the appro value
    private void SendColorAccordingToAppro(int appro)
    {
        // Calculate color components based on the approval value
        //float red = 0f;
        // float blue = 0f;
        /*
        if (appro > 0)
        {
            // Positive approval: increase blue intensity
            blue = appro / 3f; // Map appro (1 to 3) to blue intensity (0.33 to 1)
        }
        else if (appro < 0)
        {
            // Negative approval: increase red intensity
            red = Mathf.Abs(appro) / 3f; // Map appro (-1 to -3) to red intensity (0.33 to 1)
        }
        */
        Color color = eyeColors[appro + 3];

        
            
        // Send the color to SerialManager
        if (serialManager != null)
        {
            serialManager.SetLEDColor(color); // Update the LEDs for the
        }

        //Debug.Log($"Approval: {appro}, Color Sent: {color}");
    }
}
