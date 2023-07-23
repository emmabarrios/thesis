using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStatsManager: MonoBehaviour {

    [SerializeField] private int vitality;
    [SerializeField] private int endurance;
    [SerializeField] private int strenght;
    [SerializeField] private int dexterity;

    public void InitializePlayerStats(Player player) {

        player.Health = 100f + (vitality * 10f);
        player.Stamina = 100f + (endurance * 10f);
        player.Strenght = 100f + (strenght * 10f);
        player.Dexterity = 100f + (dexterity * 10f);

    }
}
