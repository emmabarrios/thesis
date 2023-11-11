using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Character, IDamageable {

    //public Transform camPole;
    //public Transform camTarget;
    //public float yOffset;

    [SerializeField] private float currentHealth;
    [SerializeField] float maxHealth;
    //private Animator animator;

    [SerializeField] Animator visualAnimator;

    

    //[SerializeField] private float timer;
    //[SerializeField] private float limitedTime = 5f;
    //[SerializeField] private bool isTiming;
    //[SerializeField] private float attackWindow;

    //private Weapon enemyWeapon;
    //public Weapon EnemyWeapon { get { return enemyWeapon; } set { enemyWeapon = value; } }

    [SerializeField] private Image lifebarImage = null;
    //[SerializeField] private Text text = null;
    //[SerializeField] private string enemyName = null;

    //private Transform playerTransform;
    //[SerializeField] float rotationSpeed;
    //[SerializeField] float walkSpeed;
    //[SerializeField] float attackStateTime;

    //CharacterController controller;

    //[SerializeField] private bool isWalking = true;
    [SerializeField] private bool isDefeated = false;
    //[SerializeField] private bool isHit = false;
    //[SerializeField] private bool isAttacking = false;

    //public float raycastDistance = 1f;
    //public LayerMask targetLayer;
    //public float raycastOffset = 0.5f;
    //public Color rayColor = Color.red;

    //public float actionDelay = 1f;

    //[SerializeField] private CharacterSoundFXManager characterSoundFXManager;

    public List<QuickItem> dropList = new List<QuickItem>();

    public Action<float> OnDamageTaken;

    private void Awake() {
        // Target lock the enemy on the player
        GameObject.Find("Player").GetComponent<Controller>().SetLockTarget(this.transform);

        //EnemyWeapon = GameObject.Find("Enemy Weapon Anchor Point R").GetComponentInChildren<Weapon>();
    }

    // Start is called before the first frame update
    void Start() {
        //controller = GetComponent<CharacterController>();
        //playerTransform = GameObject.Find("Player").GetComponent<Transform>();
        //animator = GetComponent<Animator>();
        //isTiming = true;
        //text.text = enemyName;
        currentHealth = Health;
        maxHealth = Health;
        //characterSoundFXManager = GetComponent<CharacterSoundFXManager>();
    }

    private void Update() {

        if (Health <= 0) {
            if (!isDefeated) {
                isDefeated = true;
                GameManager.instance.EndCombatSequence(isDefeated);
            }
        }
    }

    // Update is called once per frame
    //void FixedUpdate() {

    //    if (Health <= 0) {
    //        isWalking = false;
    //        isDefeated = true;
    //    }

    //    Vector3 rayStart = transform.position + transform.forward * raycastOffset;

    //    // Perform the raycast
    //    RaycastHit hit;

    //    if (Physics.Raycast(rayStart, transform.forward, out hit, raycastDistance, targetLayer)) {
    //        // Check if the ray hits a CharacterController
    //        CharacterController characterController = hit.collider.GetComponent<CharacterController>();

    //        if (characterController != null && characterController != controller) {
    //            // Perform the desired action when the ray hits a CharacterController
    //            isAttacking = true;
    //            Attack();
    //            animator.SetTrigger("attack");
    //        }
    //    }
    //    Debug.DrawRay(rayStart, transform.forward * raycastDistance, rayColor);

    //    animator.SetFloat("walkSpeed", walkSpeed);


    //    if (isWalking == true && isDefeated == false && isAttacking == false) {
    //        RotateToTarget(rotationSpeed, Time.fixedDeltaTime);
    //    }

    //    animator.SetBool("isDefeated", isDefeated);
    //    animator.SetBool("isWalking", isWalking);
    //    animator.SetFloat("health", Health);

    //    if (Health <= 0) {
    //        text.text = "";
    //        camTarget.position = camPole.transform.position + Vector3.up * yOffset;
    //    }

    //}

    public void TakeDamage(float damage) {
        //animator.Play("Skeleton@Damage01");
        //characterSoundFXManager.PlayeDamageSoundFX();
        Health -= damage;
        lifebarImage.fillAmount = Health / maxHealth;
        OnDamageTaken?.Invoke(Health);
        //isHit = true;
        //isWalking = false;
        //isTiming = false;

        //StartCoroutine(Recover(1f));

        //Debug.Log("Enemy: ouch!");

        if (Health<=0) {
            //GameManager.instance.SetToOutroState();
            visualAnimator.Play("defeat", -1, 0);
        }
    }

    //private void RotateToTarget(float followRotationSpeed, float timeDelta) {
    //    Vector3 directionToTarget = playerTransform.position - transform.position;
    //    directionToTarget.y = 0f;
    //    Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
    //    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, timeDelta * followRotationSpeed);
    //}

    //private IEnumerator Recover(float time) {
    //    yield return new WaitForSeconds(time);
    //    isWalking = true;
    //    isTiming = true;
    //}

    //private IEnumerator AttackState(float time) {
    //    yield return new WaitForSeconds(time);
    //    isWalking = true;
    //    isTiming = true;
    //    isAttacking = false;
    //}

    //private void OnAnimatorMove() {
    //    Vector3 deltaPosition = animator.deltaPosition;
    //    Vector3 worldDeltaPosition = deltaPosition;
    //    controller.Move(worldDeltaPosition);
    //}

}