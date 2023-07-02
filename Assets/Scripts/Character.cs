using System;
using System.Collections;
using UnityEngine;

public class Character : MonoBehaviour, IAttributes
{

    [SerializeField] private float health = 100f;
    [SerializeField] private float stamina = 100f;
    [SerializeField] private float poise;
    
    public float Health { get { return health; } set { health = value; } }
    public float Stamina { get { return stamina; } set { stamina = value; } }
    public float Poise { get { return poise; } set { poise = value; } }
    public float EquipLoad { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public float PoisonDefense { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public float FireDefense { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public float LightningDefense { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public float MagicDefense { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public float FrostbiteDefense { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public float StrikeDefense { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public float SlashDefense { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public float ThrustDefense { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public float CriticalDamage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public float StrikeDamage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public float SlashDamage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public float ThrustDamage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public float PoisonDamage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public float FireDamage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public float LightningDamage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public float MagicDamage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public float FrostbiteDamage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    [SerializeField] private bool isBlocking = false;
    [SerializeField] private bool isBusy = false;
    [SerializeField] private bool isParryPerformed = false;
    [SerializeField] private bool isAttackPerformed = false;

    public bool IsBusy { get { return isBusy; } set { isBusy = value; } }
    public bool IsBlocking { get { return isBlocking; } set { isBlocking = value; } }
    public bool IsParryPerformed { get { return isParryPerformed; } set { isParryPerformed = value; } }
    public bool IsAttackPerformed { get { return isAttackPerformed; } set { isAttackPerformed = value; } }

    public void ApplyItemAttributes(Item item) {
        throw new NotImplementedException();
    }

    public void RemoveItemAttributes(Item item) {
        throw new NotImplementedException();
    }
}
