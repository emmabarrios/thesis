using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropDownWidget : MonoBehaviour
{
    public AButton aButton = null;
    public Dropdown dropdown = null;

    // Start is called before the first frame update
    void Start()
    {
       dropdown = this.GetComponent<Dropdown>();
       ChangeButtonAxis();
    }

    public void ChangeButtonAxis() {

        int value = dropdown.value;
        if (value == 0) {
            aButton.SetAxisOption(AButton.AxisOptions.None);
        }else if (value == 1) {
            aButton.SetAxisOption(AButton.AxisOptions.Vertical);
        }
    }
}
