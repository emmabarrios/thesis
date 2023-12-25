﻿using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private float skillPointCost;
    [SerializeField] private int skillPoints;
    //[SerializeField] private int percentageModifier;

    [Header("Dummy JSON fields")]
    //public int _health;
    //public int _stamina;
    //public int _attack;
    //public int _defense;
    public int health;
    public int stamina;
    public int attack;
    public int defense;

    public int Vitality { get { return vitality; } set { vitality = value; } }
    public int Endurance { get { return endurance; } set { endurance = value; } }
    public int Strenght { get { return strenght; } set { strenght = value; } }
    public int SkillPoints { get { return skillPoints; } set { skillPoints = value; } }
    public int SkillPointCost { get { return (int)skillPointCost; } set { skillPointCost = value; } }
    public int EarnedExperience { get { return (int)earnedExp; } set { earnedExp = value; } }
    public int Health { get { return (int)health; } set { health= value; } }
    public int Stamina { get { return (int)stamina; } set { stamina = value; } }
    public int Attack { get { return (int)attack; } set { attack = value; } }
    public int Defense { get { return (int)defense; } set { defense = value; } }




    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        LoadPlayerDataFile();

        if (skillPointCost == 0) {
            skillPointCost = 5;
        }

        // Show initial values
        UpdateExperience(0);
        UpdateLastStats();
    }

    public void LoadPlayerStats(Player character) {
        weaponAttack = CombatInventory.instance.WeaponItemSO._damage;

        character.Health = 100f + (vitality * 0.10f) + (strenght * 0.25f);
        character.Stamina = 100f + (endurance * 0.50f);
        character.Damage = 10f + (strenght * 0.50f) + weaponAttack;
        character.Defense = 10f + (strenght * 0.50f) + (vitality * 0.1f) + (endurance * 0.1f);

        // Update Player attack cooldown values
        character.AttackCooldown += CombatInventory.instance.WeaponItemSO._attackCooldown;
    }


    public void UpdateExperience(float exp) {
        earnedExp += exp;
        skillPoints = (int)Mathf.Floor(earnedExp / skillPointCost);
    }

    private void LoadPlayerDataFile() {
        // cargar un archivo PlayerDataFile del "servidor" con los datos del jugador y actualizar los stats con eso
        
    }

    // MOVE THIS METHOD LATER TO THE UI MANAGER
    //public void UpdateStatsOnDatabase() {
    //    UpdateExperience(0);
    //    UpdateLastStats();
    //}

    public void UpdateLastStats() {
        //_health = (int)(100f + Mathf.Ceil(vitality * 1.50f) + Mathf.Ceil(endurance * 1.1f));
        //_stamina = (int)(100f + Mathf.Ceil(endurance * 1.50f) + Mathf.Ceil(vitality * 1.10f));
        //_attack = (int)(10f + Mathf.Ceil(strenght * 1.50f) + weaponAttack);
        //_defense = (int)(10f + Mathf.Ceil(strenght * 1.50f) + Mathf.Ceil(vitality * 1.10f) + Mathf.Ceil(endurance * 1.10f));

        Health = (int)(100f + Mathf.Ceil(vitality * 1.50f) + Mathf.Ceil(endurance * 1.1f));
        Stamina = (int)(100f + Mathf.Ceil(endurance * 1.50f) + Mathf.Ceil(vitality * 1.10f));
        Attack = (int)(10f + Mathf.Ceil(strenght * 1.50f) + weaponAttack);
        Defense = (int)(10f + Mathf.Ceil(strenght * 1.50f) + Mathf.Ceil(vitality * 1.10f) + Mathf.Ceil(endurance * 1.10f));
    }

    public int GetLastValue(CharacterStatModifierCard.CardStat cardStat) {

        int lastValue = 0;
        switch (cardStat) {
            case CharacterStatModifierCard.CardStat.Vitality:
                lastValue =  Vitality;
                break;
            case CharacterStatModifierCard.CardStat.Endurance:
                lastValue = Endurance;
                break;
            case CharacterStatModifierCard.CardStat.Strenght:
                lastValue = Strenght;
                break;
            default:
                break;
        }

        return lastValue;
    }

    public void UpdateStat(CharacterStatModifierCard.CardStat cardStat) {
        switch (cardStat) {
            case CharacterStatModifierCard.CardStat.Vitality:
                Vitality++;
                break;
            case CharacterStatModifierCard.CardStat.Endurance:
                Endurance++;
                break;
            case CharacterStatModifierCard.CardStat.Strenght:
                Strenght++;
                break;
            default:
                break;
        }
    }
}
