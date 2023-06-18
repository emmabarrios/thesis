using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class CombatInventory : MonoBehaviour
{
    WeaponSlotManager weaponSlotManager;

    [Header("Main Weapons")]
    public WeaponItem rightWeapon;
    public WeaponItem leftWeapon;

    [Header("Usable Items")]
    public List<UsableItem> List_1;
    public List<UsableItem> List_2;
    public List<UsableItem> List_3;

    private void Awake() {
        weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
    }

    private void Start() {
        weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
        weaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);
    }
}
