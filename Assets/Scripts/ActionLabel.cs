using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ActionLabel : MonoBehaviour
{
    public Button aButton = null;
    public Joystick joystick = null;
    public Text text;

    // Start is called before the first frame update
    void Start()
    {
        text = this.GetComponent<Text>();
        aButton.OnBlocking += ChangeLabelValue;
        aButton.OnParry += ChangeLabelValue;
        aButton.OnTensionReleased += ChangeLabelValue;
        joystick.OnDashPerformed += ChangeLabelValue;
        joystick.OnHandleDroped += ChangeLabelValue;
    }

    public void ChangeLabelValue(object sender, Button.OnBlockingEventArgs e) {
        text.text = e.a;
    }
    
    public void ChangeLabelValue(object sender, Button.OnParryEventArgs e) {
        text.text = e.a;
    }
    
    public void ChangeLabelValue(object sender, Joystick.OnDashPerformedEventArgs e) {
        text.text = "Dashed to point: \n" + e.point;
    }
    
    public void ChangeLabelValue(object sender, System.EventArgs e) {
        text.text = "";
    }
    
    public void ChangeLabelValue(object sender, Button.OnTensionReleasedEventArgs e) {

        if (e.magnitude > .5f) {
            text.text = "Arrow Fired:\nMagnitude: " + e.magnitude;
        } else if(e.magnitude <.5f) {
            text.text = "";
        }
    }


}
