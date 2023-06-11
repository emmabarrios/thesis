using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Player : Character 
{
    private Weapon playerWeapon;
    private Weapon playerShield;
    public Weapon PlayerWeapon { get { return playerWeapon; } set { playerWeapon = value; } }
    public Weapon PlayerShield { get { return playerShield; } set { playerShield = value; } }

    private void Awake() {
        PlayerWeapon = GameObject.Find("Player Weapon Anchor Point R").GetComponentInChildren<Weapon>();
        PlayerShield = GameObject.Find("Player Weapon Anchor Point L").GetComponentInChildren<Weapon>();
    }

}
