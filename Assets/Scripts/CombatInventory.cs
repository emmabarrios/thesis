using System.Collections.Generic;
using UnityEngine;

public class CombatInventory : MonoBehaviour
{
    // Make it "singleton"
    public static CombatInventory instance;

    [Header("Usable Items")]
    public List<Item> itemList1 = new List<Item>();
    public List<Item> itemList2 = new List<Item>();
    public List<Item> itemList3 = new List<Item>();
    public List<Item> itemList4 = new List<Item>();
    public List<List<Item>> itemLists = new List<List<Item>>();

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
        itemLists.Add(itemList3);
        itemLists.Add(itemList4);

        weaponSlotManager.LoadWeaponOnSlot(leftWeaponItemSO, true);
        weaponSlotManager.LoadWeaponOnSlot(RightWeaponItemSO, false);
    }
}
