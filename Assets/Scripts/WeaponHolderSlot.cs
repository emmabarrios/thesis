using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolderSlot : MonoBehaviour
{
    Character characterUser;
    public Transform parentOverride;
    public GameObject currentWeaponModel;
    public bool isLeftHandSlot;
    public bool isRightHandSlot;

    public void UnloadWeaponModel() {
        if (currentWeaponModel != null) {
            currentWeaponModel.SetActive(false);
        }
    }

    public void UnloadWeaponAndDestroy() {
        if (currentWeaponModel != null) {
            Destroy(currentWeaponModel);
        }
    }

    public void LoadWeaponModel(WeaponItem weaponItem) {

        UnloadWeaponAndDestroy();

        if (weaponItem == null) {
            UnloadWeaponModel();
            return;
        }

        GameObject model = Instantiate(weaponItem.modelPrefab) as GameObject;

        if (model != null) {
            if (parentOverride != null) {
                model.transform.parent = parentOverride;
            } else {
                model.transform.parent = this.transform;
            }
            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;
            model.transform.localScale = Vector3.one;
        }
        characterUser = GetComponentInParent<Character>();
        if (characterUser.CompareTag("Player")) {
            int _layer = LayerMask.NameToLayer("Player");
            gameObject.layer = _layer;
        }
        currentWeaponModel = model;
    }
}
