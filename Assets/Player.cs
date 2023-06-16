using System;
using System.Collections;
using UnityEngine;

public class Player : Character, IDamageable
{
    PlayerStats stats;

    private void Start() {
        stats = GetComponent<PlayerStats>();
        currentState = State.Combat;
    }

    //private Weapon playerWeapon;
    //private Weapon playerShield;
    //public Weapon PlayerWeapon { get { return playerWeapon; } set { playerWeapon = value; } }
    //public Weapon PlayerShield { get { return playerShield; } set { playerShield = value; } }

    private void Awake() {
        //PlayerWeapon = GameObject.Find("Player Weapon Anchor Point R").GetComponentInChildren<Weapon>();
        //PlayerShield = GameObject.Find("Player Weapon Anchor Point L").GetComponentInChildren<Weapon>();
    }

    public void TakeDamage(float damage) {
        Health -= damage;
        OnDamageTaken?.Invoke();
        OnHealthValueReduced?.Invoke(Health);
    }
}
