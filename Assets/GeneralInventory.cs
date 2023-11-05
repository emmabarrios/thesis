using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralInventory : MonoBehaviour
{
    public static GeneralInventory instance;

    [Header("Stored Items")]
    public List<QuickItem> storedItemList = new List<QuickItem>();

    private void Awake() {
        //DontDestroyOnLoad(transform.gameObject);
        if (instance == null) {
            // If no instance exists, set the instance to this object
            instance = this;

            // Mark this object to not be destroyed when loading a new scene
            DontDestroyOnLoad(gameObject);
        } else {
            // If an instance already exists, destroy this object
            Destroy(gameObject);
        }
        //DontDestroyOnLoad(transform.gameObject);

        //if (instance != null) { return; }
        //instance = this;
    }

}
