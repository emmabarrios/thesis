using UnityEngine;

[CreateAssetMenu(fileName ="Weapon Item", menuName = "Items/Weapon Item")]
public class WeaponItem: EquipmentItem {

    [Header("Wield Requirements")]
    public int _strenghtRequirement;
    public int _dexterityRequirement;

    [Header("Damage")]
    public float _damage;

    [Header("Attack Cooldown")]
    public float _attackCooldown;
}

