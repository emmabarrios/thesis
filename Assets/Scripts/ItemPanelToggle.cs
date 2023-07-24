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
    [SerializeField] private GameObject buttonB = null;
    [SerializeField] private GameObject QuickItemPanel = null;
    private Toggle toggle;

    public Action<bool> OnToggleValueChanged;
    

    // Start is called before the first frame update
    void Start()
    {
        toggle = this.GetComponent<Toggle>();

        toggle.onValueChanged.AddListener(delegate {
            ToggleValueChanged(toggle);
        });

    }

    void ToggleValueChanged(Toggle change) {

        //buttonA.SetActive(!change.isOn);
        //buttonB.SetActive(!change.isOn);
        OnToggleValueChanged?.Invoke(change.isOn);
    }

}
