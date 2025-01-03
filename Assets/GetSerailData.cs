using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

using TMPro;

public class GetSerailData : MonoBehaviour
{
    SerialPort stream = new SerialPort("COM15", 9600);
    [SerializeField] private int buttonState = 0;
    [SerializeField] private string value; 
    private TextMeshProUGUI text;
    
    // Start is called before the first frame update
    void Start()
    {
        stream.Open();
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        value = stream.ReadLine();

        int.TryParse(value, out buttonState);
        

        text.text = buttonState.ToString();
    }
}
