using UnityEngine;

[CreateAssetMenu(fileName = "Item")]
public class Item : ScriptableObject {

    [Header("Base Settings")]
    public string _name;
    public string _description;
    public Sprite _sprite;
    public GameObject _modelPrefab;
    public float _discoverability;
    public float _price;

}

