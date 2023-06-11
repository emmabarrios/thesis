using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Player : Character, IDamageable 
{
    #region Revision

   

    #endregion


    [SerializeField] private float health = 100f;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float BeginHealthRecoverDelay;

    [SerializeField] private float stamina = 100f;
    [SerializeField] private float staminaRecoverySpeed = 1f;
    [SerializeField] private float staminaRecoveryFactor = 1f;
    [SerializeField] private float staminaCost = 5f;

    [SerializeField] private float poise;
    [SerializeField] private float hitBlockRecoveryCost;
    
    private PlayerController playerController;
    private Weapon playerWeapon;
    private Weapon playerShield;
    public Weapon PlayerWeapon { get { return playerWeapon; } set { playerWeapon = value; } }
    public Weapon PlayerShield { get { return playerShield; } set { playerShield = value; } }


    [SerializeField] private float timer;
    [SerializeField] private float itemUseDelay = 5f;
    [SerializeField] private bool isTiming;

    public float Health { get { return health; } set {  health = value; } }
    public float Stamina { get { return stamina; } set {  stamina = value; } }
    public float Poise { get { return poise; } set { poise = value; } }
    public float HitBlockRecoveryCost { get { return hitBlockRecoveryCost; } set { hitBlockRecoveryCost = value; } }
    public float StaminaRecoverySpeed { get { return staminaRecoverySpeed; } set { staminaRecoverySpeed = value; } }
    public float StaminaRecoveryFactor { get { return staminaRecoveryFactor; } set { staminaRecoveryFactor = value; } }

    public event EventHandler<OnWeaponHitDetectedEventArgs> OnWeaponHitDetected;
    //public event EventHandler<OnHealthValueChanged_EventArgs> OnHealthValueChanged;
    public event EventHandler<OnStaminaValueChanged_EventArgs>  OnStaminaValueChanged;
    public event EventHandler OnDamageTaken;
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
        health = maxHealth;
        playerController = GetComponent<PlayerController>();
        

        //weapon = GameObject.Find("Player Weapon Anchor Point R").GetComponentInChildren<Weapon>();
    }

    public void Update() {

        bool isPlayerBusy = playerController.IsBusy;

        if (stamina < 100f && isPlayerBusy == false) {

            if (playerController.IsBlocking) {
                stamina += staminaRecoverySpeed * staminaRecoveryFactor * Time.deltaTime;
            } else {
                stamina += staminaRecoverySpeed * Time.deltaTime;
            }
           
            OnStaminaValueChanged?.Invoke(this, new OnStaminaValueChanged_EventArgs { value = stamina });

            if (stamina > 100f) {
                stamina = 100f;
            } else if (stamina < -5) {
                stamina = -5;
            }

        }
    }

    public void DrainStamina() {
        this.stamina -= staminaCost;
        OnStaminaValueChanged?.Invoke(this, new OnStaminaValueChanged_EventArgs { value = stamina });
    }

    public void TakeDamage(float damage) {

        bool isB = playerController.IsBlocking;
        bool isP = playerController.ParryExecuted;

        if (isB == false && isP == false) {
            this.health -= damage;

            OnDamageTaken?.Invoke(this, EventArgs.Empty);
            OnHealthValueReduced?.Invoke(health);
        }

    }

    public void RecoverHealth(float value) {
        StartCoroutine(RecoverHealthCoroutine(value, BeginHealthRecoverDelay));
    }

    private IEnumerator RecoverHealthCoroutine(float healthAmmount, float delay) {

        yield return new WaitForSeconds(delay);

        float _tempHealth = 0f;

        if (health + healthAmmount > maxHealth) {
            _tempHealth = Mathf.Min(health + healthAmmount, maxHealth) - health;
        } else {
            _tempHealth = healthAmmount;
        }

        health += _tempHealth;

        if (health > maxHealth) {
            health = maxHealth;
        }

        float targetValue = health;
        float currentValue = health - _tempHealth;

        OnHealthValueRestored?.Invoke(currentValue, targetValue);
    }

    

}
