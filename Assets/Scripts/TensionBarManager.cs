using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TensionBarManager : MonoBehaviour
{
    public Image image;
    public float tension = 100f;
    public Button abutton;

    void Start() {
        Reset();
        abutton = GameObject.Find("AButton").GetComponent<Button>();
        abutton.OnHandleDraged += UpdateFill;
        abutton.OnHandleDroped += Reset;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reset(object sender, EventArgs e) {
        image.fillAmount = 0;
    }

    public void Reset() {
        image.fillAmount = 0;
    }

    public void UpdateFill(object sender, Button.OnHandleDragedEventArgs e) {

        if (e.position > 0) {
            image.fillAmount = 0;
        } else {
            float mag = e.magnitude;
            image.fillAmount = mag / 700;
            Color spedBarColor = Color.Lerp(Color.green, Color.red, (mag / 700));
            image.color = spedBarColor;


            if (image.fillAmount > 1) {
                image.fillAmount = 1;
            } else if (image.fillAmount < 0) {
                image.fillAmount = 0;
            }
        }
       
    }

}
