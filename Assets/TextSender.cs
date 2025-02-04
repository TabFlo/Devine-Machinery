using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using System.Text;
using System.Net.Sockets;
using UnityEngine.Serialization;


public class TextSender : MonoBehaviour
{
    [SerializeField] private string projectionText = "Left";
    
    [SerializeField] private float rate;
    private float timeSinceLastWrite;
    
    private UdpClient udpClient;
    private IPEndPoint textEndPoint;  
    private int count = 0; 
    
    // Start is called before the first frame update
    void Start()
    {
        udpClient = new UdpClient();
        textEndPoint = new IPEndPoint(IPAddress.Broadcast, 5558);
    }

    private void Update()
    {
        if (timeSinceLastWrite > 1 / rate)
        {
            SetProjectionText(projectionText);
            timeSinceLastWrite = 0; 
        }

        timeSinceLastWrite += Time.deltaTime;
    }

    /// <summary>
    /// sets the text on the arm projection in vvvv 
    /// </summary>
    /// <param name="text">in the format "Left-Right</param>
    
    public void SetProjectionText(string text)
    {
        if (SendTextToVVVV(text))
        {
            projectionText = text; 
            Debug.Log("Text Set successfully");
            return; 
        }
        Debug.LogError("Could not set projection text: " + text);
    }

    public string GetProjectionText()
    {
        return projectionText; 
    }
    
  

    bool SendTextToVVVV(string message)
    {
        if (udpClient != null && textEndPoint != null)
        {

            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            try
            {
                udpClient.Send(messageBytes, messageBytes.Length, textEndPoint);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to send UDP message: {e.Message}");
                return false;
            }

            return true;
        }
        else
        {
            Debug.LogError("UDP client or endpoint is not initialized.");
        }

        return false;
    }
    void OnDestroy()
    {
      
      Debug.Log("Connection Closed");
    }
}
