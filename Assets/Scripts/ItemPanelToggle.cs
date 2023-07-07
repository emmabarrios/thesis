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
    [SerializeField] private GameObject QuickItemPanel = null;
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

        buttonA.SetActive(!change.isOn);

        //if (change.isOn) {
        //    buttonA.SetActive(false);
        //    QuickItemPanel.SetActive(false);

        //} else {
        //    buttonA.SetActive(true);
        //    QuickItemPanel.SetActive(false);
        //}
    }

}
