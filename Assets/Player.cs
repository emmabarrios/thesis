using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Player : Character, IDamageable 
{
    private PlayerController playerController;
    private Weapon playerWeapon;
    private Weapon playerShield;
    public Weapon PlayerWeapon { get { return playerWeapon; } set { playerWeapon = value; } }
    public Weapon PlayerShield { get { return playerShield; } set { playerShield = value; } }

    [SerializeField] private float timer;
    [SerializeField] private float itemUseDelay = 5f;
    [SerializeField] private bool isTiming;

    public event EventHandler<OnWeaponHitDetectedEventArgs> OnWeaponHitDetected;
    public event EventHandler<OnStaminaValueChanged_EventArgs>  OnStaminaValueChanged;
    public Action OnDamageTaken;
    public class OnHealthValueChanged_EventArgs {
        public float value;
    }
    public class OnStaminaValueChanged_EventArgs {
        public float value;
    }
    public class OnWeaponHitDetectedEventArgs: EventArgs {
        public float damage;
    }

    public Action <float, float> OnHealthValueRestored;
    public Action <float> OnHealthValueReduced;

    private void Awake() {
        PlayerWeapon = GameObject.Find("Player Weapon Anchor Point R").GetComponentInChildren<Weapon>();
        PlayerShield = GameObject.Find("Player Weapon Anchor Point L").GetComponentInChildren<Weapon>();
    }

    private void Start() {
        playerController = GetComponent<PlayerController>();
    }

    public void Update() {

        if (Stamina < 100f && !IsBusy) {

            if (playerController.IsBlocking) {
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
        this.Stamina -= StaminaCost;
        OnStaminaValueChanged?.Invoke(this, new OnStaminaValueChanged_EventArgs { value = Stamina });
    }

    public void TakeDamage(float damage) {

        bool isB = playerController.IsBlocking;
        bool isP = playerController.ParryExecuted;

        if (isB == false && isP == false) {
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

    

}
