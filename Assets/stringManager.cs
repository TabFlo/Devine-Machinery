using System;
using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;

public class stringManager : MonoBehaviour
{
    private string entryString;
    // Start is called before the first frame update
    void Start()
    {
        
    }

   
  

    public void getStringEntry()
    {
        
        entryString = DialogueManager.CurrentConversationState.subtitle.dialogueEntry.currentDialogueText;
        if (entryString.Contains("-"))
        {
            Debug.Log(entryString);
        }
        
       
        
    }

   
}
