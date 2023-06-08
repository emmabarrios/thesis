using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Character, IDamageable 
{
    [SerializeField] private float health;
    [SerializeField] private float maxHealth;
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

    private void Awake() {
        EnemyWeapon = GameObject.Find("Enemy Weapon Anchor Point R").GetComponentInChildren<Weapon>();
    }

    // Start is called before the first frame update
    void Start()
    {
        hitArea = GetComponentInChildren<HitArea>();
        controller = GetComponent<CharacterController>();
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
        animator = GetComponent<Animator>();
        isTiming = true;
        text.text = enemyName;
        health = maxHealth;
        currentHealth = health;
    }

    // Update is called once per frame
    void Update()
    {
        // Timer
        if (isTiming && health > 0) {

            timer -= Time.deltaTime;
            if (timer < 0.1f) {
                timer = limitedTime;
                isTiming = false;
                isWalking = false;
                Attack();
                animator.SetTrigger("attack");
            }
        }



        if (health <= 0) {
            isWalking = false;
            isDefeated = true;
        }

        if (currentHealth != health) {
            currentHealth = health;
            image.fillAmount = currentHealth / maxHealth;
        }



        animator.SetFloat("walkSpeed", walkSpeed);


        if (isWalking == true && isDefeated == false) {
            //Vector3 movement = transform.forward * walkSpeed * Time.deltaTime;
            //controller.Move(movement);
            RotateToTarget(rotationSpeed, Time.deltaTime);
        }
        animator.SetBool("isDefeated", isDefeated);
        animator.SetBool("isWalking", isWalking);
        animator.SetFloat("health", health);
    }

    private void Attack() {
        StartCoroutine(AttackState(attackStateTime));
    }

    public void TakeDamage(float damage) {
        animator.Play("Skeleton@Damage01");
        health -= damage;
        isHit = true;
        isWalking = false;
        isTiming = false;

        
        if (health <= 0) {
            text.text = "";
            controller.enabled = false;
        } 

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
        //isHit = false;
        isWalking = true;
        isTiming = true;
    }

    private IEnumerator AttackState(float time) {
        yield return new WaitForSeconds(time);
        //isHit = false;
        isWalking = true;
        isTiming = true;
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
