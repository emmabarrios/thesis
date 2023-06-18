using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/Item")]
public class Item : ScriptableObject {
    [Header("Item Information")]
    public string _name;
    public string _description;
    public Sprite _sprite;
    public GameObject _prefab;
    public float _discoverability;
    public float _price;
}

