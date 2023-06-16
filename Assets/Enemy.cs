using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Character, IDamageable {

    public Transform camPole;
    public Transform camTarget;
    public float yOffset;


    private float currentHealth;
    private Animator animator;

    [SerializeField] private float timer;
    [SerializeField] private float limitedTime = 5f;
    [SerializeField] private bool isTiming;
    [SerializeField] private float attackWindow;

    private Weapon enemyWeapon;
    public Weapon EnemyWeapon { get { return enemyWeapon; } set { enemyWeapon = value; } }

    public HitArea hitArea;

    [SerializeField] private Image image = null;
    [SerializeField] private Text text = null;
    [SerializeField] private string enemyName = null;

    private Transform playerTransform;
    [SerializeField] float rotationSpeed;
    [SerializeField] float walkSpeed;
    [SerializeField] float attackStateTime;

    CharacterController controller;

    [SerializeField] private bool isWalking = true;
    [SerializeField] private bool isDefeated = false;
    [SerializeField] private bool isHit = false;
    [SerializeField] private bool isAttacking = false;

    public float raycastDistance = 1f;
    public LayerMask targetLayer;
    public float raycastOffset = 0.5f;
    public Color rayColor = Color.red;

    public float actionDelay = 1f;

    private void Awake() {
        //EnemyWeapon = GameObject.Find("Enemy Weapon Anchor Point R").GetComponentInChildren<Weapon>();
    }

    // Start is called before the first frame update
    void Start() {
        hitArea = GetComponentInChildren<HitArea>();
        controller = GetComponent<CharacterController>();
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
        animator = GetComponent<Animator>();
        isTiming = true;
        text.text = enemyName;
        currentHealth = Health;
    }

    // Update is called once per frame
    void Update() {

        if (Health <= 0) {
            isWalking = false;
            isDefeated = true;
        }

        if (currentHealth != Health) {
            currentHealth = Health;
            image.fillAmount = currentHealth / Health;
        }


        Vector3 rayStart = transform.position + transform.forward * raycastOffset;

        // Perform the raycast
        RaycastHit hit;

        if (Physics.Raycast(rayStart, transform.forward, out hit, raycastDistance, targetLayer)) {
            // Check if the ray hits a CharacterController
            CharacterController characterController = hit.collider.GetComponent<CharacterController>();

            if (characterController != null && characterController != controller) {
                // Perform the desired action when the ray hits a CharacterController
                isAttacking = true;
                Attack();
                animator.SetTrigger("attack");
            }
        }
        Debug.DrawRay(rayStart, transform.forward * raycastDistance, rayColor);

        animator.SetFloat("walkSpeed", walkSpeed);


        if (isWalking == true && isDefeated == false && isAttacking == false) {
            RotateToTarget(rotationSpeed, Time.deltaTime);
        }

        animator.SetBool("isDefeated", isDefeated);
        animator.SetBool("isWalking", isWalking);
        animator.SetFloat("health", Health);

        if (Health <= 0) {
            text.text = "";
            camTarget.position = camPole.transform.position + Vector3.up * yOffset;
        }

    }

    private void Attack() {
        StartCoroutine(AttackState(attackStateTime));
    }

    public void TakeDamage(float damage) {
        animator.Play("Skeleton@Damage01");
        Health -= damage;
        isHit = true;
        isWalking = false;
        isTiming = false;

        StartCoroutine(Recover(1f));
    }

    private void RotateToTarget(float followRotationSpeed, float timeDelta) {
        Vector3 directionToTarget = playerTransform.position - transform.position;
        directionToTarget.y = 0f;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, timeDelta * followRotationSpeed);
    }

    private IEnumerator Recover(float time) {
        yield return new WaitForSeconds(time);
        isWalking = true;
        isTiming = true;
    }

    private IEnumerator AttackState(float time) {
        yield return new WaitForSeconds(time);
        isWalking = true;
        isTiming = true;
        isAttacking = false;
    }

    private void OnAnimatorMove() {
        Vector3 deltaPosition = animator.deltaPosition;
        Vector3 worldDeltaPosition = deltaPosition;
        controller.Move(worldDeltaPosition);
    }

    public void OnWeaponHit() {
        hitArea.ActivateHitArea(enemyWeapon.damage, enemyWeapon.hitWindow);
    }

}