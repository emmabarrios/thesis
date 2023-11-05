using UnityEngine;

public class PlayerStatsManager: MonoBehaviour 
{
    public static PlayerStatsManager instance;

    [Header("Player base stats")]
    [SerializeField] private int vitality;
    [SerializeField] private int endurance;
    [SerializeField] private int strenght;

    [Header("Weapon stats")]
    private float weaponAttack;

    private void Awake() {
        //DontDestroyOnLoad(transform.gameObject);
        if (instance == null) {
            // If no instance exists, set the instance to this object
            instance = this;

            // Mark this object to not be destroyed when loading a new scene
            DontDestroyOnLoad(gameObject);
        } else {
            // If an instance already exists, destroy this object
            Destroy(gameObject);
        }
        //DontDestroyOnLoad(transform.gameObject);

        //if (instance != null) { return; }
        //instance = this;
    }

    public void LoadCharacterStats(Character character) {
        weaponAttack = CombatInventory.instance.WeaponItemSO._damage;

        character.Health = 100f + (vitality * 0.10f) + (strenght * 0.25f);
        character.Stamina = 100f + (endurance * 0.50f);
        character.Attack = 10f + (strenght * 0.50f) + weaponAttack;
        character.Defense = 10f + (strenght * 0.50f) + (vitality * 0.1f) + (endurance * 0.1f);
        
    }
}
