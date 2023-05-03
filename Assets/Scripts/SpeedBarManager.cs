using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SpeedBarManager : MonoBehaviour
{

    public Image image;
    public float speed = 100f;
    public Joystick joystick;

    public void UpdateFill(object sender, Joystick.OnHandleDragedEventArgs e) {
        float mag = e.magnitude;
        image.fillAmount = mag / 100f; 
    }

    public void Reset(object sender, EventArgs e) {
        image.fillAmount = 0;
    }

    void Start()
    {
        joystick = GameObject.Find("Dynamic Joystick").GetComponent<Joystick>();
        joystick.OnHandleDraged += UpdateFill;
        joystick.OnHandleDroped += Reset;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
