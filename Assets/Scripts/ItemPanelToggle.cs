using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemPanelToggle : MonoBehaviour
{
    [SerializeField] private GameObject buttonA = null;
    //[SerializeField] private GameObject buttonB = null;
    //[SerializeField] private GameObject leftIndicator = null;
    //[SerializeField] private GameObject rightIndicator = null;
    private Toggle toggle;

    // Start is called before the first frame update
    void Start()
    {
        toggle = this.GetComponent<Toggle>();

        toggle.onValueChanged.AddListener(delegate {
            ToggleValueChanged(toggle);
        });

    }

    void ToggleValueChanged(Toggle change) {
        if (change.isOn) {
            buttonA.SetActive(false);
            //buttonB.SetActive(false);

            //leftIndicator.GetComponent<MoveLinear>().IsActive = true;
            //rightIndicator.GetComponent<MoveLinear>().IsActive = true;

        } else {
            buttonA.SetActive(true);
            //buttonB.SetActive(true);

            //leftIndicator.GetComponent<MoveLinear>().IsActive = false;
            //rightIndicator.GetComponent<MoveLinear>().IsActive = false;
        }
    }

}
