using System.Collections;
using UnityEngine;

public class Consumable : MonoBehaviour
{
    [Header("Attribute modifiers")]
    public float health;
    public float stamina;
    public float equipLoad;
    public float poise;
    public float poisonDefense;
    public float fireDefense;
    public float lightningDefense;
    public float magicDefense;
    public float frostbiteDefense;
    public float strikeDefense;
    public float slashDefense;
    public float thrustDefense;

    public float delayTime;
    public Character user;

    public void Use() {

        user.Health += health;
        //user.Stamina += stamina;
        //user.EquipLoad += equipLoad;
        //user.Poise += poise;
        //user.PoisonDefense += poisonDefense;
        //user.FireDefense += fireDefense;
        //user.LightningDefense += lightningDefense;
        //user.MagicDefense += magicDefense;
        //user.FrostbiteDefense += frostbiteDefense;
        //user.StrikeDefense += strikeDefense;
        //user.SlashDefense += slashDefense;
        //user.ThrustDefense += thrustDefense;
    }

    public void Start() {
        StartCoroutine(DelayedUse());
    }

    private IEnumerator DelayedUse() {
        yield return new WaitForSeconds(delayTime);
        Use();
    }
}
