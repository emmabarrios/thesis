using UnityEngine;

public class Item : ScriptableObject {
    [Header("Item Information")]
    public string _name;
    public string _description;
    public Sprite _sprite;
    public float _discoverability;
    public float _price;
}

