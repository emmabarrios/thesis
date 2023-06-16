using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponSlotManager : MonoBehaviour
{
    [SerializeField] private WeaponHolderSlot leftHandSlot;
    [SerializeField] private WeaponHolderSlot rightHandSlot;

    DamageCollider leftHandDamageCollider;
    DamageCollider rightHandDamageCollider;

    private void Awake() {
        WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();
        foreach (WeaponHolderSlot weaponSlot in weaponHolderSlots) {
            if (weaponSlot.isLeftHandSlot) {
                leftHandSlot = weaponSlot;
            } else if (weaponSlot.isRightHandSlot) {
                rightHandSlot = weaponSlot;
            }
        }
    }

    public void LoadWeaponOnSlot(WeaponItem weaponItem, bool isLeft) {
        if (isLeft) {
            leftHandSlot.LoadWeaponModel(weaponItem);
            LoadLeftHandWeaponCollider();
        } else {
            rightHandSlot.LoadWeaponModel(weaponItem);
            LoadRightHandWeaponCollider();
        }
    }

    #region Weapon's Collider handling
    public void LoadLeftHandWeaponCollider() {
        leftHandDamageCollider = leftHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
    }

    public void LoadRightHandWeaponCollider() {
        rightHandDamageCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
    }

    public void OpenRightHandDamageCollider() {
        rightHandDamageCollider.EnableDamageCollider();
    }

    public void OpenLeftHandDamageCollider() {
        leftHandDamageCollider.EnableDamageCollider();
    }

    public void CloseRightHandDamageCollider() {
        rightHandDamageCollider.DisableDamageCollider();
    }

    public void CloseLeftHandDamageCollider() {
        leftHandDamageCollider.DisableDamageCollider();
    }
    #endregion

}
