using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldUIManager : MonoBehaviour
{
    [SerializeField] private GameObject eventUiPanel;
    //[SerializeField] private GameObject eventPanelUserNotInRange;
    public bool isEventUiPanelActive;


    // Start is called before the first frame update
    public void DisplayStartEventPanel() {
        if (isEventUiPanelActive == false) {
            eventUiPanel.SetActive(true);
            isEventUiPanelActive = true;
        }
    }

    //public void DisplayUserNotInRangeEventPanel() {
    //    if (isEventUiPanelActive == false) {
    //        eventPanelUserNotInRange.SetActive(true);
    //        isEventUiPanelActive = true;
    //    }
    //}

    public void CloseButtonClick() {
        eventUiPanel.SetActive(false);
        //eventPanelUserNotInRange.SetActive(false);
        isEventUiPanelActive = false;

    }



}
