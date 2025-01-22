using System;
using System.Collections;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using UnityEngine.Events;

public class TouchCheckScript : MonoBehaviour
{
    private static int appro = 0;  // Initialize appro to 0 at the start
    private bool isUpdating = false;  // Flag to prevent multiple updates in quick succession
    public static bool touchAllowed = false;

    public UnityEvent touchConmtinue;

    private void Start()
    {
       
        DialogueLua.SetVariable("Approval", appro);  // Initialize it for the Dialogue System
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            //Debugging/Fast Dialogue advancing
            (DialogueManager.dialogueUI as StandardDialogueUI).OnContinue();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (isUpdating) return;  // Prevent multiple updates within the same frame
        Debug.Log("Ball has entered the trigger!");

        if (DialogueLua.GetVariable("touched").AsBool == false)
        {
            DialogueLua.SetVariable("touched", true);
           // (DialogueManager.dialogueUI as StandardDialogueUI).OnContinue();
            var entry = DialogueManager.masterDatabase.GetDialogueEntry(2, 1);
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
        StopAllCoroutines();  // Stop any running coroutines when the trigger is exited
        isUpdating = false;
    }

    public IEnumerator SendAnswer()
    {
        if (isUpdating) yield break; // Don't start another coroutine if one is already running

        isUpdating = true;  // Lock the update to prevent overlaps

        yield return new WaitForSeconds(3f);  // Wait for 3 seconds before making any change

        // Update the appro value inside the switch case
        switch (tag)
        {
            case "Trim":
                appro -= 1;  // Decrease appro by 1
                Debug.Log("Trim pressed. Decreasing approval.");
                
                break;

            case "Fasz":
                appro += 1;  // Increase appro by 1
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
        
        (DialogueManager.dialogueUI as StandardDialogueUI).OnContinue();
        isUpdating = false;  // Unlock the update after the coroutine is done
    }

    private void UpdateApprovalVariable()
    {
        // Update the approval value for both Dialogue Lua and logging purposes
        DialogueLua.SetVariable("Approval", appro);
        Debug.Log($"Approval updated: {appro}");
    }
    
    
    
}
