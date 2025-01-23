using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class TouchCheckScript : MonoBehaviour
{
    private static int appro = 0; // Initialize approval value
    private bool isUpdating = false; // Prevent multiple updates within the same frame
    public static bool touchAllowed = false;

    public bool usePhysicalTouch = true; // Toggle between physical touch and trigger events

    private UdpClient udpClient; // UDP client for sending data
    private IPEndPoint endPoint; // Target endpoint for vvvv
    
    public SerialManager serialManager;

    private void Start()
    {
        // Initialize UDP communication
        udpClient = new UdpClient();
        endPoint = new IPEndPoint(IPAddress.Loopback, 5555); // vvvv listens on port 5555

        // Initialize the approval variable for the Dialogue System
        DialogueLua.SetVariable("Approval", appro);

        // Send the current appro value to vvvv on start
        SendApprovalToVVVV(appro);
    }

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
        StopAllCoroutines(); // Stop any running coroutines related to the trigger
        isUpdating = false; // Reset the update lock
    }

    public IEnumerator SendAnswer()
    {
        if (isUpdating) yield break; // Prevent overlapping coroutines

        isUpdating = true; // Lock to prevent multiple updates
        yield return new WaitForSeconds(2f); // Wait before updating

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

        appro = Mathf.Clamp(appro, -3, 3); // Clamp approval value
        UpdateApprovalVariable(); // Update Lua variable
        SendApprovalToVVVV(appro); // Send updated approval to vvvv

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
            // Transform the Unity `appro` value to vvvv's format
            int vvvvValue = 3 - value;

            // Convert the transformed value to a byte array
            byte[] messageBytes = BitConverter.GetBytes(vvvvValue);

            // Send the transformed value to vvvv
            udpClient.Send(messageBytes, messageBytes.Length, endPoint);
            Debug.Log($"Sent approval value to vvvv: {vvvvValue}");
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
            HandleTouch("Fasz");
        }
        else if (touchR && !isUpdating)
        {
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
            DialogueLua.SetVariable("touched", true);
            (DialogueManager.dialogueUI as StandardDialogueUI).OnContinue();
            var entry = DialogueManager.masterDatabase.GetDialogueEntry(2, 261);
            var state = DialogueManager.conversationModel.GetState(entry);
            DialogueManager.conversationController.GotoState(state);
        }
        else if (touchAllowed)
        {
            StartCoroutine(SendAnswer());
        }
    }

    private void HandleTouchEnd()
    {
        Debug.Log("No touch detected. Stopping coroutines and resetting.");
        StopAllCoroutines(); // Stop any running coroutines
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
