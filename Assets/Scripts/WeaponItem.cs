using UnityEngine;

[CreateAssetMenu(fileName ="Weapon Item", menuName = "Items/Weapon Item")]
public class WeaponItem: EquipmentItem {

    [Header("Weapon Settings")]
    public int _strenghtRequirement;
    public int _dexterityRequirement;
    public float _strikeDamage;
    public float _slashDamage;
    public float _thrustDamage;
}

