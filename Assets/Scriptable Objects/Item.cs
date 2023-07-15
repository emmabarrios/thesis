using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/Item")]
public class Item : ScriptableObject {
   public enum ItemType { Usable, Thowable};

    [Header("Item Information")]
    public ItemType type;
    public string _name;
    public string _description;
    public Sprite _sprite;
    public Sprite _sprite_pouch;
    public Sprite _sprite_pouch_used;
    public float _cooldown;
    public GameObject _modelPrefab;
    public GameObject _usablePrefab;
    public Projectile _projectilePrefab;
    public float _discoverability;
    public float _price;
}

