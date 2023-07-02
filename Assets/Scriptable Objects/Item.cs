using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/Item")]
public class Item : ScriptableObject {
    [Header("Item Information")]
    public string _name;
    public string _description;
    public Sprite _sprite;
    public Sprite _sprite_pouch;
    public Sprite _sprite_pouch_used;
    public GameObject _modelPrefab;
    public GameObject _usablePrefab;
    public float _discoverability;
    public float _price;
}

