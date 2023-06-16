using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStatsManager: MonoBehaviour {

    Character character;

    [SerializeField] private int vitality;
    [SerializeField] private int endurance;
    [SerializeField] private int strenght;
    [SerializeField] private int dexterity;
    [SerializeField] private int agility;
    [SerializeField] private int intelligence;
    [SerializeField] private int vigor;
    [SerializeField] private int luck;

    public int Vitality { get { return vitality; } set { vitality = value; } }
    public int Endurance { get { return endurance; } set { endurance = value; } }
    public int Strenght { get { return strenght; } set { strenght = value; } }
    public int Dexterity { get { return dexterity; } set { dexterity = value; } }
    public int Agility { get { return agility; } set { agility = value; } }
    public int Intelligence { get { return intelligence; } set { intelligence = value; } }
    public int Vigor { get { return vigor; } set { vigor = value; } }
    public int Luck { get { return luck; } set { luck = value; } }


    private void Start() {
        character = GetComponent<Character>();
    }

    public void InitializePlayerStats() {

        // Calculate health based on Vitality
        character.Health = 100f + (vitality * 10f);
        character.Stamina = 100f + (vitality * 10f);

        // Calculate Slash Damage and Thrust Damage based on Dexterity
        //float dexterityDamageBonus = dexterity * 10f;
        //character.SlashDamage = dexterityDamageBonus;
        //character.ThrustDamage = dexterityDamageBonus;
    }
}
