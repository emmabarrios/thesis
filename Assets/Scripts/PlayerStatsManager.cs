using UnityEngine;

public class PlayerStatsManager: MonoBehaviour 
{
    public static PlayerStatsManager instance;

    [Header("Player base stats")]
    [SerializeField] private int vitality;
    [SerializeField] private int endurance;
    [SerializeField] private int strenght;

    [Header("Weapon stats")]
    [SerializeField] private float weaponAttack;

    [Header("Travel stats")]
    [SerializeField] private float traveledDistance;
    [SerializeField] private float maxDistance;
    [SerializeField] private float travelExp;

    [Header("Experience")]
    [SerializeField] private float earnedExp;
    [SerializeField] private float maxExp;
    [SerializeField] private int skillPoints;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void LoadPlayerStats(Player character) {
        weaponAttack = CombatInventory.instance.WeaponItemSO._damage;

        character.Health = 100f + (vitality * 0.10f) + (strenght * 0.25f);
        character.Stamina = 100f + (endurance * 0.50f);
        character.Attack = 10f + (strenght * 0.50f) + weaponAttack;
        character.Defense = 10f + (strenght * 0.50f) + (vitality * 0.1f) + (endurance * 0.1f);

        // Update Player attack cooldown values
        character.AttackCooldown += CombatInventory.instance.WeaponItemSO._attackCooldown;
    }


    public void UpdateExperience(float exp) {
        earnedExp += exp;
        if (earnedExp >= maxExp) {
            skillPoints++;
            float remainder = Mathf.Abs(maxExp - earnedExp);
            earnedExp = remainder;
        }
    }
}
