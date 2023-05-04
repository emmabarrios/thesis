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

    void Start()
    {
        Reset();
        joystick = GameObject.Find("Dynamic Joystick").GetComponent<Joystick>();
        joystick.OnHandleDraged += UpdateFill;
        joystick.OnHandleDroped += Reset;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateFill(object sender, Joystick.OnHandleDragedEventArgs e) {
        float mag = e.magnitude;
        image.fillAmount = mag / 100f;
        Color spedBarColor = Color.Lerp(Color.green, Color.red, (mag / 100f));
        image.color = spedBarColor;

        if (image.fillAmount > 1) {
            image.fillAmount = 1;
        } else if (image.fillAmount < 0) {
            image.fillAmount = 0;
        }
    }

    public void Reset(object sender, EventArgs e) {
        image.fillAmount = 0;
    }

    public void Reset() {
        image.fillAmount = 0;
    }

}
