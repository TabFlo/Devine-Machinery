using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using JetBrains.Annotations;
using TMPro;

public enum DATA_TYPE {
    COLOR, 
    FLOAT, 
    STATE,
}

public enum BODY_PART {
    EYE, 
    TORSO, 
    CHEST, 
    HAND_L, 
    HAND_R,
}

public enum LED_STATE //These NEED to be the same as on the Arduino sonnst bin ich gefickt
{
    ON, 
    OFF,
    FADE, 
    FLASH, 
}

public class SerialManager : MonoBehaviour
{
    // Setup
    SerialPort stream = new ("COM15", 9600);
    [SerializeField] private float readRate = 10;
    
    // Testing
    [SerializeField] private Color color;

    // Serial Read/Write
    private float timeSinceLastRead;
    private String currentReadLine;

    //Data
    private Dictionary<BODY_PART, float> sensorData = new Dictionary<BODY_PART, float>();
   // public static event Action OnHandTouched 

    // Unity Functions
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
    }

    // Update is called once per frame
    void Update()
    {
        if (timeSinceLastRead >= (1 / readRate))
        {
            TryReadData(stream, ref currentReadLine);
            ReadToFData();
            sendColorData(color, "EYE");  
            
            timeSinceLastRead = 0;
        }

        timeSinceLastRead += Time.deltaTime;
    }
    
    // API Methods
    
    /// <summary>
    /// Returns sensor Data from various body Parts, funktioniert jetzt mal nur für
    /// </summary>
    /// <param name="bodyPart"></param>
    /// <returns>Value is always a float, needs to be parsed to other data types if needed</returns>
    public float GetSensorData(BODY_PART bodyPart)
    {
        return sensorData[bodyPart];
    }

    public void SetLEDColor(Color c, BODY_PART bodyPart)
    {
        sendColorData(c,bodyPart.ToString());
    }

    public void SetLedState()
    {
        //TODO: Implement this.
    }
    
    public void SetLedAnimationSpeed(BODY_PART? bodyPart)
    {
        if (String.IsNullOrEmpty(bodyPart.ToString()))
        {
            //TODO: change Speed gloablly, implement for other functions as well
        }
    }

    // Private Methods 

    private void ReadToFData()
    {
        BODY_PART[] parts = { BODY_PART.HAND_L, BODY_PART.HAND_R };
        var output = -1;
        
        foreach (BODY_PART bodyPart in parts)
        {
            if (TryReadData(stream, ref currentReadLine) && currentReadLine.Contains(bodyPart.ToString()))
            {
                currentReadLine = currentReadLine.Replace(bodyPart.ToString(), "");
                TryParseData(DATA_TYPE.FLOAT, ' ', currentReadLine, ref output);
            }
            sensorData[bodyPart] = output; 
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
    
    void sendColorData(Color c, [CanBeNull] String prefix)
    {
        int r = Mathf.RoundToInt(c.r * 255); 
        int g = Mathf.RoundToInt(c.g * 255); 
        int b = Mathf.RoundToInt(c.b * 255);
        
        String msg = String.Join(" ", r.ToString(), g.ToString(), b.ToString());
        
        if (!String.IsNullOrEmpty(prefix))
        {
            msg = prefix + " " + msg; 
        }
        SendData(msg);
    }


    bool TryReadData(SerialPort stream, ref String currentReadLine)
    {
        if (stream.IsOpen && stream.BytesToRead > 0)
        {
            String value = stream.ReadLine();
            Debug.Log(value);
            currentReadLine = value;
            return true; 
        }
        return false; 
    }

    //TODO: Refactor this bad boi 
    private bool TryParseData<T>(DATA_TYPE dataType, char delimiter, string data, ref T output)
    {
        String[] slices = data.Split(delimiter);
        String prefix = slices[0];
        
        
        if (!String.Equals(prefix, dataType.ToString())) return false; 
        
        switch (dataType)
        {
            case DATA_TYPE.FLOAT:
                if (float.TryParse(slices[1], out float resultF))
                {
                    output = (T)Convert.ChangeType(resultF, typeof(T));
                    return true; 
                }
                break; 
            
            case DATA_TYPE.COLOR:
                if (slices.Length != 4) break;
                
                float[] col = new float[3];
                for (int i = 1; i < slices.Length; i++)
                {
                    if (!float.TryParse(slices[i], out col[i - 1])) break;
                    col[i - 1] /= 255;
                }

                Color resultC = new Color(col[0], col[1], col[2]); //TODO: add additional checks on the color Value
                output = (T)Convert.ChangeType(resultC, typeof(T));
                return true;

            case DATA_TYPE.STATE: 
                if (bool.TryParse(slices[1], out bool resultB))
                {
                    output = (T)Convert.ChangeType(resultB, typeof(T));
                    return true;
                }
                break;
        }

        Debug.LogWarning($"Could not parse Data: {data} to data Type {dataType}");
        return false; 
    }

    // Housekeeping
    private void OnApplicationQuit()
    {
        if (stream != null && stream.IsOpen)
        {
            stream.Close();
            Debug.Log("Stream closed: ");
        }
    }
}
