using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouseFollowForTest : MonoBehaviour
{
    public GameObject mouseBall;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,Mathf.Abs(Camera.main.transform.position.z-transform.position.z)));
        newPos.z = transform.position.z;


        transform.position = newPos;
    }

    
}
