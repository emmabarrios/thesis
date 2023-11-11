using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralInventory : MonoBehaviour
{
    public static GeneralInventory instance;

    [Header("Stored Items")]
    public List<QuickItem> storedQuickItemList = new List<QuickItem>();
    public List<WeaponItem> storedWeaponItemList = new List<WeaponItem>();

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void StoreItems(List<QuickItem> items, List<WeaponItem> weapons = null) {
        foreach (QuickItem item in items) {
            storedQuickItemList.Add(item);
        }

        foreach (WeaponItem item in weapons) {
            storedWeaponItemList.Add(item);
        }
    }

}
