using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public enum State { Normal, Combat, None }
    public State CurrentState { get; private set; }


    public void SetState(State newState) {
        if (newState == CurrentState) return;
        CurrentState = newState;
    }

    [SerializeField] private float health = 100f;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float beginHealthRecoverDelay;
    [SerializeField] private float stamina = 100f;
    [SerializeField] private float staminaRecoverySpeed = 1f;
    [SerializeField] private float staminaRecoveryFactor = 1f;
    [SerializeField] private float staminaCost = 5f;
    [SerializeField] private float poise;
    [SerializeField] private float parryRecoveryTime;
    [SerializeField] private bool isBusy = false;

    public float Health { get { return health; } set { health = value; } }
    public float Stamina { get { return stamina; } set { stamina = value; } }
    public float Poise { get { return poise; } set { poise = value; } }
    public float ParryRecoveryTime { get { return parryRecoveryTime; } set { parryRecoveryTime = value; } }
    public float StaminaRecoverySpeed { get { return staminaRecoverySpeed; } set { staminaRecoverySpeed = value; } }
    public float StaminaRecoveryFactor { get { return staminaRecoveryFactor; } set { staminaRecoveryFactor = value; } }
    public float StaminaCost { get { return staminaCost; } set { staminaCost = value; } }
    public float BeginHealthRecoverDelay { get { return beginHealthRecoverDelay; } set { beginHealthRecoverDelay = value; } }
    public float MaxHealth { get { return maxHealth; } set { maxHealth = value; } }
    public bool IsBusy { get { return isBusy; } set { isBusy = value; } }
}
