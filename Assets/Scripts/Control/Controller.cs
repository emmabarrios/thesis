using System;
using System.Collections;
using System.IO.Pipes;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class Controller : MonoBehaviour {

    private Player player;
    private CharacterController characterController;
    [SerializeField] private Animator animator = null;
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
    private float currentSpeed;
    private Vector3 last_movement;

    [Header("Dash Settings")]
    [SerializeField] private float dashDistance;
    [SerializeField] private float dashMovementSpeed;
    [SerializeField] private float dirMultiplier = 1f;
    [SerializeField] private float lookRotationSpeedMultiplier = 4f;

    [Header("Boolean parameters")]
    private bool isWalking;
    [SerializeField] private bool parryPerformed = false;

    [SerializeField] private bool isBlocking = false;
    [SerializeField] private bool dashPerformed = false;
    [SerializeField] private bool attackPerformed = false;
    [SerializeField] private bool isUsingItem = false;

    public bool IsWalking { get { return isWalking; } set { isWalking = value; } }
    public bool ParryPerformed { get { return parryPerformed; } set { parryPerformed = value; } }
    public bool DashPerformed { get { return dashPerformed; } set { dashPerformed = value; } }
    public bool IsBlocking { get { return isBlocking; } set { isBlocking = value; } }
    public bool AttackPerformed { get { return attackPerformed; } set { attackPerformed = value; } }
    public float CurrentSpeed { get { return currentSpeed; } }
    public bool IsUsingItem { get { return isUsingItem; } set { isUsingItem = value; } }

    [Header("Input")]
    public Joystick joystick = null;
    public GestureInput gestureInput;
    public Button buttonA;

    // Events
    public Action OnBlocking;
    public Action OnReleaseBlock;
    public Action OnParry;
    public Action<int> OnDash;
    public Action<GestureInput.SwipeDir> OnAttack;

    private void Start() {

        player = GetComponent<Player>();
        characterController = GetComponent<CharacterController>();

        // Input
        buttonA = GameObject.Find("Button").GetComponent<Button>();
        joystick = GameObject.Find("Joystick").GetComponent<Joystick>();
        gestureInput = GameObject.Find("Gesture Input").GetComponent<GestureInput>();

        // Input Event Subscribers
        buttonA.OnBlocking += Block;
        buttonA.OnToggleValueChanged += Block;
        playerAnimator.OnAnimating += InheritPositionFromAnimation;
        playerAnimator.OnQuickItemAction += ToggleIsUsingItem;

        gestureInput.SwipeDirectionChanged += ProcessGestureSwipes;
        joystick.OnDoubleTap += Dash;

        // Initial Values
        currentSpeed = 0f;
    }

    private void Update() {

        Vector2 inputMovement = joystick.Direction;

        // Evaluate attack performed
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        AttackPerformed = stateInfo.IsName("Swing_Left") || stateInfo.IsName("Swing_Right") || 
            stateInfo.IsName("Swing_Stab") || 
            stateInfo.IsName("Swing_Down");

        if (!dashPerformed) {
            Orbitate(inputMovement);
        }

        if (player.IsBlocking != IsBlocking) {
            player.IsBlocking = IsBlocking;
        }
        if (player.IsBusy != dashPerformed) {
            player.IsBusy = dashPerformed;
        }
        if (player.IsAttackPerformed != AttackPerformed) {
            player.IsAttackPerformed = AttackPerformed;
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

            if (player.Stamina > 0 && !AttackPerformed && !IsUsingItem && !dashPerformed) {

                switch (e.swipeDirection) {

                    case GestureInput.SwipeDir.Left:
                        animator.Play("Swing_Left");
                        player.DrainStamina();
                        break;
                    case GestureInput.SwipeDir.Up:
                        animator.Play("Swing_Stab");
                        player.DrainStamina();
                        break;
                    case GestureInput.SwipeDir.Down:
                        animator.Play("Swing_Down");
                        player.DrainStamina();
                        break;
                    case GestureInput.SwipeDir.Right:
                        animator.Play("Swing_Right");
                        player.DrainStamina();
                        break;

                    default:
                        break;
                }
               
            }
        }
    }

    private void Dash(object sender, Joystick.OnDoubleTapEventArgs e) {

        if (!dashPerformed && player.Stamina > 0) {
            dashPerformed = true;
            if (!IsBlocking && !IsUsingItem && !AttackPerformed) {
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

        dashPerformed = false;
    }

    private void Block(object sender, EventArgs e) {
        if (!IsBlocking) {
            IsBlocking = true;
        }
    }  
    
    private void Block(bool value) {
        IsBlocking = value;
    }

    private void ReleaseBlock(object sender, EventArgs e) {
        if (IsBlocking) {
            IsBlocking = false;
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

}
