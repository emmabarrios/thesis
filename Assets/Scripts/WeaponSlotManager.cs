using UnityEngine;

public class WeaponSlotManager : MonoBehaviour
{
    [SerializeField] private WeaponHolderSlot leftHandSlot;
    [SerializeField] private WeaponHolderSlot rightHandSlot;

    Weapon leftHandWeapon;
    Weapon rightHandWeapon;

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
            LoadLeftHandWeapon();
        } else {
            rightHandSlot.LoadWeaponModel(weaponItem);
            LoadRightHandWeapon();
        }
    }

    #region Weapon's Collider handling
    public void LoadLeftHandWeapon() {
        leftHandWeapon = leftHandSlot.currentWeaponModel.GetComponentInChildren<Weapon>();
    }

    public void LoadRightHandWeapon() {
        rightHandWeapon = rightHandSlot.currentWeaponModel.GetComponentInChildren<Weapon>();
    }

    public void OpenRightHandWeaponCollider() {
        rightHandWeapon.EnableDamageCollider();
        rightHandWeapon.EnableSwingTrail();
    }

    public void OpenLeftHandWeaponCollider() {
        leftHandWeapon.EnableDamageCollider();
    }

    public void CloseRightHandWeaponCollider() {
        rightHandWeapon.DisableDamageCollider();
    }

    public void CloseLeftHandWeaponCollider() {
        leftHandWeapon.DisableDamageCollider();
    }
    #endregion

    #region Weapon's Visual handling
    public void ToggleRightHandWeaponVisual() {
        rightHandSlot.currentWeaponModel.SetActive(!rightHandSlot.currentWeaponModel.gameObject.activeSelf);
    }
    #endregion
}
