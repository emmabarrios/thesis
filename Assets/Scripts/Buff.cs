using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : MonoBehaviour
{
    public float criticalDamage;
    public float strikeDamage;
    public float slashDamage;
    public float thrustDamage;
    public float poisonDamage;
    public float fireDamage;
    public float lightningDamage;
    public float magicDamage;
    public float frostbiteDamage;

    public void Use(Weapon weapon) {
        weapon.CriticalDamage += criticalDamage;
        weapon.StrikeDamage += strikeDamage;
        weapon.SlashDamage += slashDamage;
        weapon.ThrustDamage += thrustDamage;
        weapon.PoisonDamage += poisonDamage;
        weapon.FireDamage += fireDamage;
        weapon.LightningDamage += lightningDamage;
        weapon.MagicDamage += magicDamage;
        weapon.FrostbiteDamage += frostbiteDamage;
    }
}
