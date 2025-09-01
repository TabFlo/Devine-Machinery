using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class TouchCheckScript : MonoBehaviour
{

    private float touchTime = 1; 
    public static int appro = 0; // Initialize approval value
    private bool isUpdating = false; // Prevent multiple updates within the same frame
    public static bool touchAllowed = false;

    public bool usePhysicalTouch = true; // Toggle between physical touch and trigger events

    private UdpClient udpClient; // UDP client for sending data
    private IPEndPoint endPoint; // Target endpoint for vvvv
    private IPEndPoint glitchMessageEndPoint;
    private IPEndPoint StopMessageEndPoint;
    
    public SerialManager serialManager;
    public GlitchManager glitchManager;
    private Coroutine touchHandlerCoroutine;
    private bool killChoice = false;

    private const string ApprovalKey = "Approval";  
    public void saveApproval()
    {
        PlayerPrefs.SetInt(ApprovalKey, appro);
        PlayerPrefs.Save();
    }

    public void applyApproval()
    {
        
        
        appro = PlayerPrefs.GetInt(ApprovalKey, appro);
        
        DialogueLua.SetVariable("Approval", appro);

        
        SendApprovalToVVVV(appro);
        SoundManager.Instance.UpdateApprovalSound(appro);
        
        Debug.LogWarning(appro);
    }

    private void Start()
    {
        // Initialize UDP communication
        udpClient = new UdpClient();
        endPoint = new IPEndPoint(IPAddress.Broadcast, 5555); // vvvv listens on port 5555

        glitchMessageEndPoint = new IPEndPoint(IPAddress.Broadcast, 5556);
        StopMessageEndPoint = new IPEndPoint(IPAddress.Broadcast, 5557);
        
        applyApproval();

        
    }

    #region MouseInputOnly

    

   
    private void OnTriggerEnter(Collider other)
    {
        if (isUpdating) return; // Prevent multiple updates within the same frame
        Debug.Log("Ball has entered the trigger!");

        if (DialogueLua.GetVariable("touched").AsBool == false)
        {
            DialogueLua.SetVariable("touched", true);
            (DialogueManager.dialogueUI as StandardDialogueUI).OnContinue();
            var entry = DialogueManager.masterDatabase.GetDialogueEntry(2, 261);
            var state = DialogueManager.conversationModel.GetState(entry);
            DialogueManager.conversationController.GotoState(state);
        }
        else
        {
            if (touchAllowed)
            {
                StartCoroutine(SendAnswer());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Ball has exited the trigger!");
        StopAllCoroutines(); 
        isUpdating = false; 
    }
    #endregion 
    public IEnumerator SendAnswer()
{
    if (isUpdating) yield break; // Prevent overlapping coroutines

    isUpdating = true; // Lock to prevent multiple updates
    yield return new WaitForSeconds(touchTime); // Wait before updating

    // Check if killChoice is true and handle special behavior
    if (killChoice)
    {
        // Special behavior when killChoice is true
        Debug.Log("killChoice is true. Entering special behavior.");

        // Use switch-case to handle different tags
        switch (tag)
        {
            case "Trim":
               
                Debug.Log("Trim touched. Setting 'killing' to true.");
                var entry = DialogueManager.masterDatabase.GetDialogueEntry(2, 325);
                var state = DialogueManager.conversationModel.GetState(entry);
                DialogueManager.conversationController.GotoState(state);
                appro = 0;  // Reset approval as part of the special behavior
                break;

            case "Fasz":
               
                Debug.Log("Fasz touched. Setting 'killing' to true.");
                var entry2 = DialogueManager.masterDatabase.GetDialogueEntry(2, 326);
                var state2 = DialogueManager.conversationModel.GetState(entry2);
                DialogueManager.conversationController.GotoState(state2);
                break;

            default:
                DialogueLua.SetVariable("killing", false);  // Set 'killing' to false for all other tags
                Debug.Log("Not 'Trim' or 'Fasz'. Setting 'killing' to false.");
                break;
        }
    }

    else
    {
        // Normal behavior (if killChoice is false)
        switch (tag)
        {
            case "Trim":
                appro -= 1; // Decrease approval
                Debug.Log("Trim touched. Decreasing approval.");
                break;

            case "Fasz":
                appro += 1; // Increase approval
                Debug.Log("Fasz touched. Increasing approval.");
                
                break;

            default:
                Debug.Log("Unhandled tag. No approval change.");
                break;
        }
    }

    glitchManager.ledUpdateDue = true;

    // Always execute the approval logic after special or normal behavior
    appro = Mathf.Clamp(appro, -3, 3); // Clamp approval value
    UpdateApprovalVariable(); // Update Lua variable
    SendApprovalToVVVV(appro); // Send updated approval to vvvv
    SoundManager.Instance.UpdateApprovalSound(appro);

    // Continue dialogue after handling approval updates
    (DialogueManager.dialogueUI as StandardDialogueUI).OnContinue();
    isUpdating = false; // Unlock after completion
}



    private void UpdateApprovalVariable()
    {
        DialogueLua.SetVariable("Approval", appro);
        Debug.Log($"Approval updated to: {appro}");
    }

    private void SendApprovalToVVVV(int value)
    {
        if (udpClient != null && endPoint != null)
        {
            int vvvvValue = 3 - value;
            byte[] messageBytes = BitConverter.GetBytes(vvvvValue);

            try
            {
                udpClient.Send(messageBytes, messageBytes.Length, endPoint);
                Debug.Log($"Sent approval value to vvvv: {vvvvValue}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to send UDP message: {e.Message}");
            }
        }
        else
        {
            Debug.LogError("UDP client or endpoint is not initialized.");
        }
    }


    private void UpdateTouchState()
    {
        bool touchL = serialManager.GetTouchState(BODY_PART.TOUCH_L);
        bool touchR = serialManager.GetTouchState(BODY_PART.TOUCH_R);

        if (touchL && !isUpdating)
        {
            Debug.Log("Touched L");
            HandleTouch("Fasz");
        }
        else if (touchR && !isUpdating)
        {
            Debug.Log("Touched R");
            HandleTouch("Trim");
        }
        else if (!touchL && !touchR)
        {
            HandleTouchEnd();
        }
    }

    private void HandleTouch(string touchTag)
    {
        Debug.Log($"Touch detected: {touchTag}");

        tag = touchTag; // Dynamically assign the tag
        if (DialogueLua.GetVariable("touched").AsBool == false)
        {
            StartTouchHandler();
        }
        else if (touchAllowed)
        {
            StartCoroutine(SendAnswer());
        }
    }

    private void HandleTouchEnd()
    {
        //Debug.Log("No touch detected. Stopping coroutines and resetting.");
        if (touchHandlerCoroutine != null)
        {
            StopCoroutine(touchHandlerCoroutine);
            touchHandlerCoroutine = null;
        }
        isUpdating = false;
    }

    private void Update()
    {
        if (usePhysicalTouch)
        {
            UpdateTouchState(); // Continuously check for physical touch inputs
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            (DialogueManager.dialogueUI as StandardDialogueUI).OnContinue(); // Debug advancing dialogue
        }
        killChoice = DialogueLua.GetVariable("killChoice").AsBool;
    }

    private void StartTouchHandler()
    {
        if (isUpdating) return; // Prevent overlapping coroutines

        isUpdating = true; // Lock to prevent multiple updates
        touchHandlerCoroutine = StartCoroutine(TouchDelayCoroutine());
    }

    private IEnumerator TouchDelayCoroutine()
    {
        yield return new WaitForSeconds(2f); // Wait for 2 seconds

        // Check if touch is still active before proceeding
        if (!serialManager.GetTouchState(BODY_PART.TOUCH_L) && !serialManager.GetTouchState(BODY_PART.TOUCH_R))
        {
            Debug.Log("Touch removed before execution. Cancelling.");
            isUpdating = false;
            yield break; // Stop execution
        }

        // Execute dialogue logic
        DialogueLua.SetVariable("touched", true);
        (DialogueManager.dialogueUI as StandardDialogueUI).OnContinue();
        var entry = DialogueManager.masterDatabase.GetDialogueEntry(2, 261);
        var state = DialogueManager.conversationModel.GetState(entry);
        DialogueManager.conversationController.GotoState(state);

        isUpdating = false; // Unlock after completion
    }

    public void resetAppro()
    {
        appro = 0;
        DialogueLua.SetVariable("Approval", appro);
    }

    private void HandleKillChoice()
    {
        // Special logic when killChoice is true
        Debug.Log("killChoice is true. Executing special behavior.");

        
    }

    public void SendGlitchToVVVV(int glitchState)
    {
        if (udpClient != null && glitchMessageEndPoint != null)
        {
            
            // Convert the integer to a string before encoding it
            
            byte[] messageBytes = BitConverter.GetBytes(glitchState);

            // Send the integer as a UTF-8 string message to vvvv on port 5556
            udpClient.Send(messageBytes, messageBytes.Length, glitchMessageEndPoint);
            Debug.Log($"Sent glitch state to vvvv: {glitchState}");
        }
        else
        {
            Debug.LogError("UDP client or glitch endpoint is not initialized.");
        }
    }
    
    public void stopMessageToVVVV(int messageState)
    {
        if (udpClient != null && StopMessageEndPoint != null)
        {
            
            // Convert the integer to a string before encoding it
            
            byte[] messageBytes = BitConverter.GetBytes(messageState);

            // Send the integer as a UTF-8 string message to vvvv on port 5557
            udpClient.Send(messageBytes, messageBytes.Length, StopMessageEndPoint);
            Debug.Log($"Sent message state to vvvv: {messageState}");
        }
        else
        {
            Debug.LogError("UDP client or glitch endpoint is not initialized.");
        }
    }

    private void OnApplicationQuit()
    {
        if (udpClient != null)
        {
            udpClient.Close(); // Close the UDP client
            Debug.Log("UDP client closed.");
        }
    }
}
