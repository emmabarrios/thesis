using System;
using System.Collections;
using UnityEngine;

public class Player : Character, IDamageable
{
    public enum PlayerState { Normal, Combat, None }
    public PlayerState currentState { get; set; }

    public void SetState(PlayerState newState) {
        if (newState == currentState) return;
        currentState = newState;
    }

    PlayerStatsManager stats;

    [SerializeField] bool canDrainStamina = false;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float beginHealthRecoverDelay;
    [SerializeField] private float staminaRecoverySpeed = 1f;
    [SerializeField] private float staminaRecoveryModifier = 1f;
    [SerializeField] private float staminaCost = 5f;

    public float StaminaRecoverySpeed { get { return staminaRecoverySpeed; } set { staminaRecoverySpeed = value; } }
    public float StaminaRecoveryModifier{ get { return staminaRecoveryModifier; } set { staminaRecoveryModifier = value; } }
    public float StaminaCost { get { return staminaCost; } set { staminaCost = value; } }
    public float BeginHealthRecoverDelay { get { return beginHealthRecoverDelay; } set { beginHealthRecoverDelay = value; } }
    public float MaxHealth { get { return maxHealth; } set { maxHealth = value; } }
    public float MaxStamina { get { return maxStamina; } set { maxStamina = value; } }


    [SerializeField] private bool isBlocking = false;

    public bool IsBlocking { get { return isBlocking; } set { isBlocking = value; } }

    public Action OnDamageTaken;
    public Action OnHitBlocked;
    public Action<float, float> OnHealthValueRestored;
    public Action<float> OnHealthValueReduced;

    public event EventHandler<OnWeaponHitDetectedEventArgs> OnWeaponHitDetected;
    public event EventHandler<OnStaminaValueChanged_EventArgs> OnStaminaValueChanged;
    public class OnHealthValueChanged_EventArgs {
        public float value;
    }
    public class OnStaminaValueChanged_EventArgs {
        public float value;
    }
    public class OnWeaponHitDetectedEventArgs : EventArgs {
        public float damage;
    }

    private void Start() {
        //animator = GetComponentInChildren<PlayerAnimator>();
        stats = GetComponent<PlayerStatsManager>();
        currentState = PlayerState.Combat;
        stats.InitializePlayerStats(this);
        MaxHealth = Health;
        MaxStamina = Stamina;
    }

    private void Update() {
        if (Stamina < MaxStamina && canDrainStamina) {

            if (IsBlocking) {
                Stamina += StaminaRecoverySpeed * staminaRecoveryModifier * Time.deltaTime;
            } else {
                Stamina += StaminaRecoverySpeed * Time.deltaTime;
            }

            OnStaminaValueChanged?.Invoke(this, new OnStaminaValueChanged_EventArgs { value = Stamina });

            if (Stamina > MaxStamina) {
                Stamina = MaxStamina;
            } else if (Stamina < -5) {
                Stamina = -5;
            }
        }
    }

    public void DrainStamina() {
        canDrainStamina = false;
        StartCoroutine(DelayStaminaDrain(.15f));
    }

    public void RecoverHealth(float value) {
        StartCoroutine(RecoverHealthCoroutine(value, BeginHealthRecoverDelay));
    }

    private IEnumerator RecoverHealthCoroutine(float healthAmmount, float delay) {

        yield return new WaitForSeconds(delay);

        float _tempHealth = 0f;

        if (Health + healthAmmount > MaxHealth) {
            _tempHealth = Mathf.Min(Health + healthAmmount, MaxHealth) - Health;
        } else {
            _tempHealth = healthAmmount;
        }

        Health += _tempHealth;

        if (Health > MaxHealth) {
            Health = MaxHealth;
        }

        float targetValue = Health;
        float currentValue = Health - _tempHealth;

        OnHealthValueRestored?.Invoke(currentValue, targetValue);
    }

    private IEnumerator DelayStaminaDrain(float time) {
        yield return new WaitForSeconds(time);
        canDrainStamina = true;
        this.Stamina -= StaminaCost;
        OnStaminaValueChanged?.Invoke(this, new OnStaminaValueChanged_EventArgs { value = Stamina });
    }

    public void TakeDamage(float damage) {
        if (!IsBlocking) {
            Health -= damage;
            if (Health<0f) {
                Health = 0f;
            }
            OnDamageTaken?.Invoke();
            OnHealthValueReduced?.Invoke(Health);
        } else {
            OnHitBlocked?.Invoke();
            DrainStamina();
        }
    }

}
