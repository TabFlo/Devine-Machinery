using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Net.Sockets;
using UnityEngine.Serialization;


public class TextSender : MonoBehaviour
{

    [SerializeField] private string projectionText = "Left";


    [SerializeField] private float rate;
    private float timeSinceLastWrite;

    private TcpClient client;
    private NetworkStream stream;
    private int count = 0; 
    
    // Start is called before the first frame update
    void Start()
    {
       ConnectToServer("127.0.0.1", 8080);
    }


 
    
    /// <summary>
    /// sets the text on the arm projection in vvvv 
    /// </summary>
    /// <param name="text">in the format "Left-Right</param>
    
    public void SetProjectionText(string text)
    {
        if (SendMessageToServer(text + "/"))
        {
            projectionText = text; 
            Debug.Log("Text Set scuessfully");
        }
        Debug.LogError("Could not set projection text: " + text);
    }

    public string GetProjectionText()
    {
        return projectionText; 
    }
    
    void ConnectToServer(string ip, int port)
    {
        try
        {
            client = new TcpClient(ip, port);
            stream = client.GetStream(); 
            Debug.Log("Connceted to server");
        }
        catch (SocketException ex)
        {
            Debug.LogError("SocketException: " + ex.Message);
        }
    }

    bool SendMessageToServer(string message)
    {
        try
        {
            if (stream != null && stream.CanWrite)
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                stream.Write(data, 0, data.Length);
                Debug.Log("Sent: " + message);
                return true;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error while sending message: " + ex.Message);
        }

        return false; 
    }
    void OnDestroy()
    {
      if(stream != null) stream.Close();
      if(client != null) client.Close();
      Debug.Log("Connection Closed");
    }
}
