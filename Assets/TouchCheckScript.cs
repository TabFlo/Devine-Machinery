using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using UnityEngine.Events;

public class TouchCheckScript : MonoBehaviour
{
    private static int appro = 0; // Initialize appro to 0 at the start
    private bool isUpdating = false; // Flag to prevent multiple updates in quick succession
    public static bool touchAllowed = false;

    public UnityEvent touchConmtinue;

    private UdpClient udpClient; // UDP client for sending data
    private IPEndPoint endPoint; // Target endpoint for vvvv

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            // Debugging/Fast Dialogue advancing
            (DialogueManager.dialogueUI as StandardDialogueUI).OnContinue();
        }
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
        StopAllCoroutines(); // Stop any running coroutines when the trigger is exited
        isUpdating = false;
    }

    public IEnumerator SendAnswer()
    {
        if (isUpdating) yield break; // Don't start another coroutine if one is already running

        isUpdating = true; // Lock the update to prevent overlaps

        yield return new WaitForSeconds(3f); // Wait for 3 seconds before making any change

        // Update the appro value inside the switch case
        switch (tag)
        {
            case "Trim":
                appro -= 1; // Decrease appro by 1
                Debug.Log("Trim pressed. Decreasing approval.");
                break;

            case "Fasz":
                appro += 1; // Increase appro by 1
                Debug.Log("Fasz pressed. Increasing approval.");
                break;

            default:
                Debug.Log("Tag does not match any case. No changes to approval.");
                break;
        }

        // Clamp the appro value to be between -3 and 3
        appro = Mathf.Clamp(appro, -3, 3);

        // Ensure that the updated approval is saved in the dialogue system immediately after the case
        UpdateApprovalVariable();

        // Send the updated appro value to vvvv
        SendApprovalToVVVV(appro);

        (DialogueManager.dialogueUI as StandardDialogueUI).OnContinue();
        isUpdating = false; // Unlock the update after the coroutine is done
    }

    private void UpdateApprovalVariable()
    {
        // Update the approval value for both Dialogue Lua and logging purposes
        DialogueLua.SetVariable("Approval", appro);
        Debug.Log($"Approval updated: {appro}");
    }

    private void SendApprovalToVVVV(int value)
    {
        if (udpClient != null && endPoint != null)
        {
            // Transform the Unity `appro` value to vvvv's format
            int vvvvValue = 3 - value;

            // Convert the transformed value to a byte array
            byte[] messageBytes = BitConverter.GetBytes(vvvvValue);

            // Send the transformed value as a byte array to vvvv
            udpClient.Send(messageBytes, messageBytes.Length, endPoint);

            Debug.Log($"Sent approval value to vvvv: {vvvvValue}");
        }
        else
        {
            Debug.LogError("UDP client or endpoint is not initialized.");
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
