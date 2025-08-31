using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugManager : MonoBehaviour
{

    [SerializeField] TMP_InputField portInputField;

    [SerializeField] private Button submitButton;

    [SerializeField] private SerialManager _serialManager;

    [SerializeField] private TextMeshProUGUI portNameText;

    [SerializeField] private GameObject debugPanel; 
    // Start is called before the first frame update
    
    void Start()
    {
        submitButton.onClick.AddListener(OnSubmit);
        portNameText.text = $"PortName: {_serialManager.PortName}";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F7))
        {
            debugPanel.SetActive(!debugPanel.activeSelf);
        }
    }

    void OnSubmit()
    {
        _serialManager.stream.Close();
        _serialManager.PortName = portInputField.text; 
        _serialManager.SetupSerialPort();
        portNameText.text = $"PortName: {_serialManager.PortName}";
    }
}
