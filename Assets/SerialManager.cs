using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using JetBrains.Annotations;


public enum DATA_TYPE {
    COLOR, 
    FLOAT, 
    STATE,
}

public enum BODY_PART {
    EYE, 
    BACK, 
    CHEST, 
    HAND_L, 
    HAND_R, 
    TOUCH_L,
    TOUCH_R,
}

public enum LED_STATE //These NEED to be the same as on the Arduino sonnst bin ich gefickt
{
    ON, 
    OFF,
    BLINK, 
    FLASH,
    WAVE, 
}

public class SerialManager : MonoBehaviour
{
    // Setup
    SerialPort stream = new ("COM15", 9600);
    [SerializeField] private float readRate = 0.5f;
    
    // Debugging
    [SerializeField] private Color color;
    [SerializeField] private float distL;
    [SerializeField] private float touchR;
    [SerializeField] private float touchL; 

    // Serial Read/Write
    private float timeSinceLastRead;
    private List<string> buffer = new List<string>();
    private List<string> currentLines = new List<string>();

    //Data
    private Dictionary<BODY_PART, float> sensorData = new Dictionary<BODY_PART, float>();

    // Unity Functions
    void Start()
    {
        stream.ReadTimeout = 50; 
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
        if (timeSinceLastRead >= (1 / readRate) && stream.IsOpen)
        {
            Debug.Log("in loop");
            TryReadData(stream, ref buffer);

            foreach (string line in buffer)
            {
                ReadSensorData(line);
                currentLines.Add(line);
            }

            touchL = GetSensorData(BODY_PART.TOUCH_L);
            touchR = GetSensorData(BODY_PART.TOUCH_R);
            if (sensorData.ContainsKey(BODY_PART.TOUCH_L) && sensorData.ContainsKey(BODY_PART.HAND_L))
            {
                distL = GetSensorData(BODY_PART.HAND_L);
            }

            timeSinceLastRead = 0;
        }

        foreach (string line in currentLines)
        {
            buffer.Remove(line);
        }
        currentLines.Clear();
        timeSinceLastRead += Time.deltaTime;
        
        if(Input.GetKeyDown(KeyCode.P))
        {
            SetLEDColor(color);
            foreach (KeyValuePair<BODY_PART, float> entry in sensorData)
            {
                Debug.Log($"Body Part: {entry.Key}, Value: {entry.Value}");
                
            }
        }
    }

    public bool canSend()
    {
        return (timeSinceLastRead > 1 / readRate);
    }

   
    // API Methods
    
    /// <summary>
    /// Returns sensor Data from various body Parts, funktioniert jetzt mal nur für hände und distanz
    /// </summary>
    /// <param name="bodyPart"></param>
    /// <returns>Value is always a float, needs to be parsed to other data types if needed, returns -1 if not found</returns>
    public float GetSensorData(BODY_PART bodyPart)
    {
        if(sensorData.TryGetValue(bodyPart, out var data))
        {
            return data;
        }
        return -1; 
    }

    /// <summary>
    /// Changes color of leds on a specific body part, specify no body part to change all the led colors
    /// </summary>
    /// <param name="c">Led color, alpha g</param>
    /// <param name="bodyPart"></param>
    public void SetLEDColor(Color c, BODY_PART? bodyPart)
    {
        sendColorData(c,bodyPart.ToString());
    }

    public void SetLEDColor(Color c)
    {
        BODY_PART[] parts = { BODY_PART.EYE, BODY_PART.CHEST, BODY_PART.BACK};
        foreach (BODY_PART part in parts)
        {
            sendColorData(c, part.ToString());
        }
    }

    public void SetLEDState(LED_STATE ledState)
    {
        BODY_PART[] parts = { BODY_PART.EYE, BODY_PART.CHEST, BODY_PART.BACK};
        foreach (BODY_PART part in parts)
        {
            sendStateData(ledState, part.ToString());
        }
    }
    
    /// <summary>
    /// Returns Touch state as bool, easier to use
    /// </summary>
    /// <param name="bodyPart"></param>
    /// <returns></returns>
    public bool GetTouchState(BODY_PART bodyPart)
    {
        if (!(bodyPart.ToString().Equals(BODY_PART.TOUCH_L.ToString()) ||
            bodyPart.ToString().Equals(BODY_PART.TOUCH_R.ToString())))
        {
            return false; 
        }
        if (GetSensorData(bodyPart) == 1)
        {
            return true;
        }
        return false; 
    }
    
    public void SetLedAnimationSpeed(BODY_PART? bodyPart)
    {
        Debug.LogWarning("not implemented yet");
        if (String.IsNullOrEmpty(bodyPart.ToString()))
        {
            //TODO: change Speed gloablly, implement for other functions as well
        }
    }

    // Private Methods 

    private void ReadSensorData(string line)
    {
        BODY_PART[] parts = { BODY_PART.HAND_L, BODY_PART.HAND_R,  BODY_PART.TOUCH_L ,  BODY_PART.TOUCH_R  };
        var output = -1;
        
        foreach (BODY_PART bodyPart in parts)
        {
            if (!string.IsNullOrEmpty(line) && line.Contains(bodyPart.ToString()))
            {
                TryParseData(DATA_TYPE.FLOAT, ' ', line, ref output);
                Debug.Log("curretnLine " + line);
                sensorData[bodyPart] = output;
                
            }
        }
    }

    void SendData(String msg)
    {
        if(timeSinceLastRead > 1/readRate && stream != null && stream.IsOpen)
        {
            stream.WriteLine(msg + "\n");
            Debug.Log("Sent Message: " + msg);
        }
        else
        {
            Debug.LogWarning("could not set text " + msg + "because sendtime is too short");
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

    void sendStateData(LED_STATE ledState, string prefix)
    {
        string msg = ledState.ToString();

        if (!String.IsNullOrEmpty(prefix))
        {
            msg = prefix + " " + msg;
            msg = "STATE " + msg;
        }
        SendData(msg);
    }

    bool TryReadData(SerialPort stream, ref List<string> currentReadLine)
    {
        string buffer = ""; 
        if (stream.IsOpen && stream.BytesToRead > 0)
        {
            string value = stream.ReadExisting();
            if (!string.IsNullOrEmpty(value))
            {
                buffer += value;
                int newlineIndex;
                while ((newlineIndex = buffer.IndexOf('\n')) != -1)
                {
                    // Extract a single line
                    string message = buffer.Substring(0, newlineIndex);
                    currentReadLine.Add(message);
                    
                    buffer = buffer.Substring(newlineIndex + 1); 
                    
                    Debug.Log("Messages " + currentReadLine.Count);
                }
            }
            
            return true; 
        }
        return false; 
    }

    //TODO: Refactor this bad boi 
    private bool TryParseData<T>(DATA_TYPE dataType, char delimiter, string data, ref T output)
    {
        String[] slices = data.Split(delimiter);
        String prefix = slices[0];
        
        switch (dataType)
        {
            case DATA_TYPE.FLOAT:
                if (int.TryParse(slices[1], out int resultF))
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
