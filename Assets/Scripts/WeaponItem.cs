using UnityEngine;

[CreateAssetMenu(fileName ="Weapon Item", menuName = "Items/Weapon Item")]
public class WeaponItem: Item 
{
    public GameObject modelPrefab;
    public float durability;
    public bool isUnarmed;
}

