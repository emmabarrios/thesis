using UnityEngine;

[CreateAssetMenu(fileName ="Weapon Item", menuName = "Items/Weapon Item")]
public class WeaponItem: EquipmentItem {

    [Header("Wield Requirements")]
    public int _strenghtRequirement;
    public int _dexterityRequirement;

    [Header("Damage")]
    public float _strikeDamage;
    public float _slashDamage;
    public float _thrustDamage;

    [Header("Attack Cooldown")]
    public float _attackCooldown;
}

