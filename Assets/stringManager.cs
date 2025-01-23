using System.Collections;
using PixelCrushers.DialogueSystem;
using UnityEngine;

public class stringManager : MonoBehaviour
{
    private string entryString;
   [SerializeField] private float timerDuration = 180f; // Timer duration in seconds (default: 3 minutes)
    private Coroutine timerCoroutine; // Reference to the running timer coroutine
    private bool isTimerRunning = false;
    public TextSender textSender;

    void Start()
    {
        // You can adjust the timer duration here or via other scripts
    }

    public void getStringEntry()
    {
        entryString = DialogueManager.CurrentConversationState.subtitle.dialogueEntry.currentDialogueText;

        if (entryString.Contains("-"))
        {
            Debug.Log(entryString);
            TouchCheckScript.touchAllowed = true;
            string trimmedEntry = entryString.Replace(" ", "").Replace(".", "");
            textSender.SetProjectionText(trimmedEntry);
            
            if (!isTimerRunning)
            {
                timerCoroutine = StartCoroutine(StartTimer());
            }
        }
        else
        {
            TouchCheckScript.touchAllowed = false;

           
            if (isTimerRunning)
            {
                StopCoroutine(timerCoroutine);
                isTimerRunning = false;
                Debug.Log("Timer stopped as condition is no longer true.");
            }
        }
    }

    private IEnumerator StartTimer()
    {
        isTimerRunning = true;
        float elapsedTime = 0f;

        Debug.Log("Timer started.");

        
        while (elapsedTime < timerDuration)
        {
            if (!entryString.Contains("-"))
            {
                
                Debug.Log("Condition invalidated during the timer. Stopping.");
                isTimerRunning = false;
                yield break;
            }

            elapsedTime += Time.deltaTime;
            yield return null; // Wait until the next frame
        }

        // Timer finished and condition remained true
        Debug.Log("Timer completed. Executing action.");
        ExecuteDialogueAction();
        isTimerRunning = false;
    }

    private void ExecuteDialogueAction()
    {
        DialogueLua.SetVariable("touched", false);  
        var entry = DialogueManager.masterDatabase.GetDialogueEntry(2, 257);
        var state = DialogueManager.conversationModel.GetState(entry);
        DialogueManager.conversationController.GotoState(state);

        Debug.Log("Dialogue action executed.");
    }
}
