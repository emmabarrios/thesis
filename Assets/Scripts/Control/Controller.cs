using System;
using System.Collections;
using UnityEngine;

public class Controller : MonoBehaviour {

    private Player player;
    private CharacterController characterController;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerAnimator playerAnimator;

    [Header("Orbital Settings")]
    [SerializeField] private Transform target = null;
    private float lookRotationSpeed = 4f;

    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float maxMovementSpeed = 10f;
    [SerializeField] private float accelerationTime = 2f;
    [SerializeField] private float deaccelerationTime = 2f;
    [SerializeField] private float speedLimitMultiplier = 1f;
    [SerializeField] private float turnSpeed = 90f;
    private bool isWalking;

    private float currentSpeed;
    private Vector3 last_movement;

    [Header("Knock Back Settings")]
    [SerializeField] private float knockBackDistance;
    [SerializeField] private float knockBackTime;
    [SerializeField] private float knockBackTimer;
    [SerializeField] private bool isKnockBacked = false;
    [SerializeField] private bool isHitBlocked = false;

    [Header("Dash Settings")]
    [SerializeField] private float dashDistance;
    [SerializeField] private float dashMovementSpeed;
    [SerializeField] private float dirMultiplier = 1f;
    [SerializeField] private float lookRotationSpeedMultiplier = 4f;


    [SerializeField] private bool canBlock = true;
    [SerializeField] private bool isBlocking = false;
    [SerializeField] private bool dashPerformed = false;
    [SerializeField] private bool attackPerformed = false;
    [SerializeField] private bool isUsingItem = false;

    public bool IsWalking { get { return isWalking; } set { isWalking = value; } }
    public bool CanBlock { get { return canBlock; } set { canBlock = value; } }
    public bool DashPerformed { get { return dashPerformed; } set { dashPerformed = value; } }
    public bool IsBlocking { get { return isBlocking; } set { isBlocking = value; } }
    public bool AttackPerformed { get { return attackPerformed; } set { attackPerformed = value; } }
    public bool IsHitBlocked { get { return isHitBlocked; } set { isHitBlocked = value; } }
    public float CurrentSpeed { get { return currentSpeed; } }
    public bool IsUsingItem { get { return isUsingItem; } set { isUsingItem = value; } }
    public bool IsKnockBacked { get { return isKnockBacked; } set { isKnockBacked = value; } }

    [Header("Input")]
    public Joystick joystick = null;
    public GestureInput gestureInput;
    public Button buttonA;
    public ItemPanelToggle quickItemToggle = null;

    [Header("Attack Details")]
    public float attackCooldown;
    public float attackTimer;

    // Events
    public Action OnBlocking;
    public Action OnReleaseBlock;
    public Action OnParry;
    public Action<int> OnDash;
    public Action<float> OnAttackCooldown;

    private void Start() {

        player = GetComponent<Player>();
        characterController = GetComponent<CharacterController>();
        playerAnimator = GetComponentInChildren<PlayerAnimator>();
        animator  = playerAnimator.GetComponent<Animator>();

        // Input
        buttonA = GameObject.Find("Button").GetComponent<Button>();
        joystick = GameObject.Find("Joystick").GetComponent<Joystick>();
        gestureInput = GameObject.Find("Gesture Input").GetComponent<GestureInput>();
        quickItemToggle = GameObject.Find("Pouch Toggle").GetComponent<ItemPanelToggle>();

        // Input Event Subscribers
        buttonA.OnBlocking += Block;

        player.OnHitBlocked += HitBlocked;
        playerAnimator.OnAnimating += InheritPositionFromAnimation;
        playerAnimator.OnQuickItemAction += ToggleIsUsingItem;

        gestureInput.SwipeDirectionChanged += ProcessGestureSwipes;
        joystick.OnDoubleTap += Dash;
        quickItemToggle.OnToggleValueChanged += ToggleWeaponActivation;

        // Initial Values
        currentSpeed = 0f;
        StartCoroutine(LoadWeaponSettings());
    } 

    private void Update() {

        Vector2 inputMovement = joystick.Direction;

        // Evaluate attack performed
        if (AttackPerformed) {
            attackTimer -= Time.deltaTime;

            float fillAmount = Mathf.Clamp01(1f - (attackTimer / attackCooldown));
            OnAttackCooldown?.Invoke(fillAmount);

            if (attackTimer < 0.1f) {
                attackTimer = 0;
                AttackPerformed = false;
                OnAttackCooldown?.Invoke(1f);
            }
        }

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        IsHitBlocked = stateInfo.IsName("Shield_Block");

        if (!DashPerformed && !IsKnockBacked && !AttackPerformed) {
            Orbitate(inputMovement);
        }

        if (player.IsBlocking != IsBlocking) {
            player.IsBlocking = IsBlocking;
        }

        // KnockBack timer
        if (IsKnockBacked) {
            Vector3 currentPosition = characterController.transform.position;
            Vector3 targetPosition = currentPosition + (-characterController.transform.forward).normalized * knockBackDistance;
            characterController.Move(targetPosition - currentPosition);
            knockBackTimer -= Time.deltaTime;
            if (knockBackTimer < 0.1f) {
                knockBackTimer = 0;
                IsKnockBacked = false;
            }
        }

        CanBlock = player.Stamina > 0;
        if (!CanBlock) {
            IsBlocking = false;
        }

        animator.SetBool("isWalking", IsWalking);
        animator.SetFloat("WalkSpeed", Mathf.Clamp(CurrentSpeed, 0.1f, 1));
        animator.SetBool("isBlocking", IsBlocking);

    }

    private void Orbitate(Vector2 inputMovement) {

        Vector3 joystickDirection = new Vector3(inputMovement.x, 0f, inputMovement.y);
        Vector3 movementDirection = transform.TransformDirection((joystickDirection));

        Vector3 movement = new Vector3(0, 0, 0);

        float targetSpeed = movementDirection.magnitude * maxMovementSpeed;

        if (movementDirection.magnitude > 0) {
            isWalking = true;
        } else {
            isWalking = false;
        }

        if (targetSpeed > 0) {
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * accelerationTime);
            movement = movementDirection.normalized * currentSpeed * speedLimitMultiplier * Time.deltaTime;
            last_movement = movement;
            RotateToTarget(lookRotationSpeed, Time.deltaTime);
        } else {

            if (currentSpeed > 1) {
                currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * deaccelerationTime);
            } else {
                currentSpeed = 0;
            }
            movement = last_movement.normalized * currentSpeed * speedLimitMultiplier * Time.deltaTime;
            RotateToTarget(lookRotationSpeed, Time.deltaTime);
        }
        characterController.Move(movement);
    }
   
    private void RotateToTarget(float followRotationSpeed, float timeDelta) {
        Vector3 directionToTarget = target.position - transform.position;
        directionToTarget.y = 0f;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, timeDelta * followRotationSpeed);
    }

    private void ProcessGestureSwipes(object sender, GestureInput.SwipeDirectionChangedEventArgs e) {

        if (player.currentState == Player.PlayerState.Combat) {

            if (player.Stamina > 0 && !AttackPerformed && !IsUsingItem && !DashPerformed && !IsHitBlocked) {

                switch (e.swipeDirection) {

                    case GestureInput.SwipeDir.Left:
                        Attack("Swing_Left");
                        break;
                    case GestureInput.SwipeDir.Up:
                        Attack("Swing_Stab");
                        break;
                    case GestureInput.SwipeDir.Down:
                        Attack("Swing_Down");
                        break;
                    case GestureInput.SwipeDir.Right:
                        Attack("Swing_Right");
                        break; 
                    case GestureInput.SwipeDir.UpRight:
                        Attack("Swing_Right");
                        break;
                    case GestureInput.SwipeDir.UpLeft:
                        Attack("Swing_Stab");
                        break;
                    case GestureInput.SwipeDir.DownRight:
                        Attack("Swing_Down");
                        break;
                    case GestureInput.SwipeDir.DownLeft:
                        Attack("Swing_Left");
                        break;

                    default:
                        break;
                }

               // IsBlocking = false;

            }
        }
    }

    private void Attack(string attackAnimation) {
        animator.Play(attackAnimation,-1,0);
        player.DrainStamina();
        AttackPerformed = true;
        attackTimer = attackCooldown;
    }

    private void Dash(object sender, Joystick.OnDoubleTapEventArgs e) {

        if (!DashPerformed && player.Stamina > 0 && !IsBlocking &&!AttackPerformed) {
            DashPerformed = true;
            if (!IsUsingItem) {
                animator.Play("Dash");
            }
            OnDash?.Invoke((int)e.point.x);
            player.DrainStamina();
            StartCoroutine(DashRoutine(e.point));
        }
    }

    private IEnumerator DashRoutine(Vector2 point) {

        float duration = dashDistance / dashMovementSpeed; // Duration in seconds

        Vector3 startPosition = characterController.transform.position;
        Vector3 targetPosition = startPosition + (characterController.transform.forward * point.y + characterController.transform.right * point.x).normalized * dashDistance;

        float elapsedTime = 0f;

        while (elapsedTime < duration) {
            float t = elapsedTime / duration;
            Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, t);

            characterController.Move(currentPosition - characterController.transform.position);

            RotateToTarget(lookRotationSpeed * lookRotationSpeedMultiplier, t);

            elapsedTime += Time.fixedDeltaTime;

            yield return null;
        }

        DashPerformed = false;
    }

    private void Block(object sender, EventArgs e) {

        if (CanBlock) {
            if (!IsBlocking) {
                IsBlocking = true;
            } else {
                IsBlocking = false;
            }
        }

    }


    private void InheritPositionFromAnimation() {
        Vector3 deltaPosition = animator.deltaPosition;
        Vector3 worldDeltaPosition = deltaPosition;
        characterController.Move(worldDeltaPosition);
    }

    private void ToggleIsUsingItem(bool value) {
        IsUsingItem = value;
    }

    private void HitBlocked() {
        isKnockBacked = true;
        knockBackTimer = knockBackTime;
        animator.GetComponent<Animator>().Play("Shield_Block", -1, 0f);
    }

    private IEnumerator LoadWeaponSettings() {
        yield return attackCooldown = CombatInventory.instance.RightWeaponItemSO._attackCooldown;
    }

    private void ToggleWeaponActivation(bool value) {

    }

}
