using System.Collections.Generic;
using UnityEngine;

public class CombatInventory : MonoBehaviour
{
    // Make it "singleton"
    public static CombatInventory instance;

    [Header("Quick Items")]
    public List<QuickItem> itemList1 = new List<QuickItem>();
    public List<QuickItem> itemList2 = new List<QuickItem>();
    public List<List<QuickItem>> itemLists = new List<List<QuickItem>>();

    [Header("Main Weapoms")]
    public WeaponItem WeaponItemSO;
    public WeaponSlotManager weaponSlotManager = null;

    void Awake() {
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

    void Start() {
        itemLists.Add(itemList1);
        itemLists.Add(itemList2);
    }

    public void LoadPlayerWeapon() {
        weaponSlotManager = GameObject.FindGameObjectWithTag("Player").GetComponent<WeaponSlotManager>();
        if (weaponSlotManager != null) {
            weaponSlotManager.LoadWeaponOnSlot(WeaponItemSO);
        } else {
            Debug.Log("Weapon slot manager not found");
        }
    }
}
