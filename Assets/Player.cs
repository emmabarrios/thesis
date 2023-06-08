using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class Player : Character, IDamageable 
{
    [SerializeField] private float health = 100f;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float healthVisualRecoveryFactor = 1f;
    [SerializeField] private float healthRecoveryDelay;

    [SerializeField] private float stamina = 100f;
    [SerializeField] private float staminaRecoveryFactor = 1f;
    [SerializeField] private float staminaCostFactor = 5f;

    private PlayerVisuals playerVisuals;
    [SerializeField] private HitArea hitArea;
    private PlayerController playerController;
    private Weapon playerWeapon;
    public Weapon PlayerWeapon { get { return playerWeapon; } set { playerWeapon = value; } }

    [SerializeField] private float timer;
    [SerializeField] private float itemUseDelay = 5f;
    [SerializeField] private bool isTiming;

    public float Health { get { return health; } set {  health = value; } }
    public float Stamina { get { return stamina; } set {  stamina = value; } }

    public event EventHandler<OnWeaponHitDetectedEventArgs> OnWeaponHitDetected;
    public event EventHandler<OnHealthValueChanged_EventArgs> OnHealthValueChanged;
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

    private void Awake() {
        PlayerWeapon = GameObject.Find("Player Weapon Anchor Point R").GetComponentInChildren<Weapon>();
    }

    private void Start() {
        health = maxHealth;
        playerController = GetComponent<PlayerController>();
        playerVisuals = GetComponentInChildren<PlayerVisuals>();
        hitArea = GetComponentInChildren<HitArea>();

        playerVisuals.OnWeaponHit += WeaponHitDetected_OnWeaponHit;

        //weapon = GameObject.Find("Player Weapon Anchor Point R").GetComponentInChildren<Weapon>();
    }

    public void Update() {
        if (stamina < 100f && playerController.IsBusy == false) {
            stamina += staminaRecoveryFactor * Time.deltaTime;
            OnStaminaValueChanged?.Invoke(this, new OnStaminaValueChanged_EventArgs { value = stamina });

            if (stamina > 100f) {
                stamina = 100f;
            }

        }
    }

    public void DrainStamina() {
        this.stamina -= staminaCostFactor;
        OnStaminaValueChanged?.Invoke(this, new OnStaminaValueChanged_EventArgs { value = stamina });
    }

    public void TakeDamage(float damage) {
        this.health -= damage;
        playerController.LimitActions(1f);
        OnDamageTaken?.Invoke(this, EventArgs.Empty);
        OnHealthValueChanged?.Invoke(this, new OnHealthValueChanged_EventArgs { value = health });
    }

    public void FillHealth(float value) {
        StartCoroutine(FillHealthVisual(value, healthRecoveryDelay));
    }

    private IEnumerator FillHealthVisual(float healthAmmount, float delay) {

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
        float startTime = Time.time;
        float endTime = startTime + healthVisualRecoveryFactor;

        while (Time.time < endTime) {
            float elapsedTime = Time.time - startTime;
            float progress = elapsedTime / healthVisualRecoveryFactor;

            currentValue = Mathf.Lerp(currentValue, targetValue, progress);
            OnHealthValueChanged?.Invoke(this, new OnHealthValueChanged_EventArgs { value = currentValue });
            yield return null;
        }
    }

    public void WeaponHitDetected_OnWeaponHit(object sender, EventArgs e) {
        hitArea.ActivateHitArea(playerWeapon.damage, playerWeapon.hitWindow);
    }

}
