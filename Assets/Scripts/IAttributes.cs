using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttributes
{
    #region Base
    float Health { get; set; }
    float Stamina { get; set; }
    float EquipLoad { get; set; }
    float Poise { get; set; }
    #endregion

    #region Defense
    float PoisonDefense { get; set; }
    float FireDefense { get; set; }
    float LightningDefense { get; set; }
    float MagicDefense { get; set; }
    float FrostbiteDefense { get; set; }
    float StrikeDefense { get; set; }
    float SlashDefense { get; set; }
    float ThrustDefense { get; set; }
    #endregion

    #region Attack
    float CriticalDamage { get; set; }
    float StrikeDamage { get; set; }
    float SlashDamage { get; set; }
    float ThrustDamage { get; set; }
    float PoisonDamage { get; set; }
    float FireDamage { get; set; }
    float LightningDamage { get; set; }
    float MagicDamage { get; set; }
    float FrostbiteDamage { get; set; }
    #endregion

    void ApplyItemAttributes(Item item);
    void RemoveItemAttributes(Item item);
}
