using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

using TMPro;

public class SerialManager : MonoBehaviour
{
    SerialPort stream = new SerialPort("COM15", 9600);
    [SerializeField] private int buttonState = 0;
    [SerializeField] private string value; 
    [SerializeField] private Color color; 
    private TextMeshProUGUI text;
    
    // Start is called before the first frame update
    void Start()
    {
        if (!stream.IsOpen){
            
            try
            {
                stream.Open();
            }
            catch (SystemException e)
            {
                Debug.Log("Failed tp Open Serial port:" + e.Message);
            }
        }

    text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {

        ReadData(stream);
        int.TryParse(value, out buttonState);
        text.text = buttonState.ToString();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SendData(color.ToString());
        }
    }

    void SendData(String msg)
    {
        if(stream != null && stream.IsOpen)
        {
            stream.WriteLine(msg);
            Debug.Log("Sent Message: " + msg);
        }
    }

    void ReadData(SerialPort stream)
    {
        if (stream.IsOpen && stream.BytesToRead > 0)
        {

            value = stream.ReadLine();
            Debug.Log(value);
        }
    }

    private void OnApplicationQuit()
    {
        if (stream != null && stream.IsOpen)
        {
            stream.Close();
            Debug.Log("Stream closed: ");
        }
    }
}
