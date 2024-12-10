using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchCheckScript : MonoBehaviour
{
    
    private void OnTriggerEnter(Collider other)
    {
        
            Debug.Log("Ball has entered the trigger!");
            StartCoroutine(sendAnswer());
    }

    private void OnTriggerExit(Collider other)
    {
        StopAllCoroutines();
    }


    public IEnumerator sendAnswer()
    {
        yield return new WaitForSecondsRealtime(3f);
        //xxx.DialogueAnswer()
        Debug.Log(gameObject.tag);
    }
}
