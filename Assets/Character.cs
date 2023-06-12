using System;
using System.Collections;
using UnityEngine;

public class Character : MonoBehaviour, IDamageable 
{
    public enum State { Normal, Combat, None }
    public State currentState { get; private set; }


    public void SetState(State newState) {
        if (newState == currentState) return;
        currentState = newState;
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
    [SerializeField] private bool isBlocking = false;
    [SerializeField] private bool isBusy = false;
    [SerializeField] private bool parryPerformed = false;
    [SerializeField] private bool attackPerformed = false;
    [SerializeField] bool canDrainStamina = false;

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
    public bool IsBlocking { get { return isBlocking; } set { isBlocking = value; } }
    public bool ParryPerformed { get { return parryPerformed; } set { parryPerformed = value; } }
    public bool AttackPerformed { get { return attackPerformed; } set { attackPerformed = value; } }

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

    public Action OnDamageTaken;
    public Action<float, float> OnHealthValueRestored;
    public Action<float> OnHealthValueReduced;

    private void Start() {
        currentState = State.Combat;
    }

    private void Update() {
        if (Stamina < 100f && !IsBusy && !AttackPerformed && !ParryPerformed && canDrainStamina) {

            if (IsBlocking) {
                Stamina += StaminaRecoverySpeed * StaminaRecoveryFactor * Time.deltaTime;
            } else {
                Stamina += StaminaRecoverySpeed * Time.deltaTime;
            }

            OnStaminaValueChanged?.Invoke(this, new OnStaminaValueChanged_EventArgs { value = Stamina });

            if (Stamina > 100f) {
                Stamina = 100f;
            } else if (Stamina < -5) {
                Stamina = -5;
            }

        }
    }

    public void DrainStamina() {
        canDrainStamina = false;
        StartCoroutine(DelayStaminaDrain(.15f));
        
    }

    public void TakeDamage(float damage) {

        if (!IsBlocking && !ParryPerformed) {
            this.Health -= damage;
            OnDamageTaken?.Invoke();
            OnHealthValueReduced?.Invoke(Health);
        }

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

}
