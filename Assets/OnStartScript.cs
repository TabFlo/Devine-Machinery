using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using PixelCrushers.DialogueSystem.Wrappers;
using UnityEngine;

public class OnStartScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    DialogueManager.StartConversation("MainDialogue1");   
    //MainDialogue1
    }

    public void startDialogue()
    {
        DialogueManager.StartConversation("MainDialogue1");   
    }
}
