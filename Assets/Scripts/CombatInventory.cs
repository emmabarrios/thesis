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
    public WeaponItem leftWeaponItemSO;
    public WeaponItem RightWeaponItemSO;
    public WeaponSlotManager weaponSlotManager = null;

    void Awake() {
        if (instance != null) { return; }
        instance = this;
    }

    void Start() {
        itemLists.Add(itemList1);
        itemLists.Add(itemList2);

        weaponSlotManager.LoadWeaponOnSlot(leftWeaponItemSO, true);
        weaponSlotManager.LoadWeaponOnSlot(RightWeaponItemSO, false);
    }
}
