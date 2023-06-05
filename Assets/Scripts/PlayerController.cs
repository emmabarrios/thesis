using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {

    public enum PlayerState { Normal, Combat }
    public PlayerState currentState;

    private Player player;
    private PlayerVisuals playerVisuals;
    private CharacterController characterController;
    private PlayerAnimator playerAnimator;

    [Header("Orbital Settings")]
    [SerializeField] private Transform target = null;
    [SerializeField] private float lookRotationSpeed = 4f;

    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float maxMovementSpeed = 10f;
    [SerializeField] private float accelerationTime = 2f;
    [SerializeField] private float deaccelerationTime = 2f;
    [SerializeField] private float speedLimitMultiplier = 1f;
    [SerializeField] private float turnSpeed = 90f;
    private float currentSpeed;
    public float CurrentSpeed { get { return currentSpeed; } }
    private Vector3 last_movement;

    [Header("Dash Settings")]
    [SerializeField] private float dashDistance;
    [SerializeField] private float dashMovementSpeed;
    [SerializeField] private float dirMultiplier = 1f;
    [SerializeField] private float lookRotationSpeedMultiplier = 4f;

    [Header("Boolean parameters")]
    private bool canAttack = true;
    private bool canDash = true;
    private bool isBlocking = false;
    private bool isRotating;
    private bool isWalking;
    private bool isLimited = false;

    public bool IsLimited { get { return isLimited; } set { isLimited = value; } }
    public bool IsWalking { get { return isWalking; } set { isWalking = value; } }
    public bool CanAttack { get { return canAttack; } }
    public bool CanDash { get { return canDash;} set { canDash = value; } }
    public bool IsBlocking { get { return isBlocking; } set { IsBlocking = value; } }
    public bool IsRotating { get { return isRotating; } set { isRotating = value; } }

    [Header("Timer Settings")]
    [SerializeField] private float timer;
    [SerializeField] private float time = 5f;
    [SerializeField] private bool isTiming;
    
    [Header("Input")]
    public Joystick joystick = null;
    public GestureInput gestureInput;
    public Button buttonA;

    // Events
    public event EventHandler OnDamageTaken;
    public event EventHandler OnBlocking;
    public event EventHandler OnReleaseBlock;
    public event EventHandler OnParry;
    public event EventHandler<OnAttackEventArgs> OnAttack;
    public event EventHandler<OnDashEventArgs> OnDash;
    public class OnDashEventArgs: EventArgs {
        public Vector2 dashPoint;
    }
    public class OnAttackEventArgs: EventArgs {
        public GestureInput.SwipeDir swipeDirection;
    }

    private void Start() {

        currentState = PlayerState.Combat;

        player = GetComponent<Player>();
        characterController = GetComponent<CharacterController>();
        playerAnimator = GetComponentInChildren<PlayerAnimator>();
        playerVisuals = GetComponentInChildren<PlayerVisuals>();

        // Input
        buttonA = GameObject.Find("A Button").GetComponent<Button>();
        joystick = GameObject.Find("Joystick").GetComponent<Joystick>();
        gestureInput = GameObject.Find("Gesture Input").GetComponent<GestureInput>();

        // Animator Event Subscribers
        playerAnimator.OnAnimating += InheritMovementFromAnimation_OnAnimating;
        playerAnimator.OnUsingItem += RunStatusTimer_OnUsingItem;
        playerAnimator.OnFinishedAction += EnableActions_OnFinishedAction;

        // Input Event Subscribers
        buttonA.OnBlocking += Block;
        buttonA.OnHandleDroped += ReleaseBlock;
        buttonA.OnParry += Parry;

        gestureInput.SwipeDirectionChanged += ProcessGestureSwipes;
        joystick.OnDoubleTap += Dash;

        // Initial Values
        currentSpeed = 0f;
        isRotating = false;

    }

    private void Update() {

        Vector2 inputMovement = joystick.Direction;

        if (!isRotating) {

            switch (currentState) {
                case PlayerState.Normal:
                    Orbitate(inputMovement);
                    break;

                case PlayerState.Combat:
                    if (canDash == true) {
                        Orbitate(inputMovement);
                    }
                    break;

                default:
                    break;
            }
        }

        // Limited Status Timer 
        if (isTiming) {
            timer -= Time.deltaTime;
            if (timer < 0.1f) {
                timer = 0;
                speedLimitMultiplier = 1f;
                isLimited = false;
                isTiming = false;
            }
        } 
    }

    private void Orbitate(Vector2 inputMovement) {

        if (canAttack == true) {

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
    }

    private IEnumerator RotateDegrees(int dir) {

        isRotating = true;

        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0f, 90f * dir, 0f));
        float t = 0f;

        while (t < 1f) {
            t += Time.deltaTime * turnSpeed;
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
            yield return null;
        }

        isRotating = false;
    }

    private IEnumerator RestoreFacingDirection() {
        isRotating = true;

        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.identity; // Initialize with no rotation
        float smallestAngleDifference = float.MaxValue;

        // Calculate the closest 90° or closest to 0° angle in world space
        for (int i = 0; i <= 3; i++) {
            Quaternion rotation = Quaternion.Euler(0f, i * 90f, 0f);
            float angleDifference = Quaternion.Angle(startRotation, rotation);

            if (angleDifference < smallestAngleDifference) {
                smallestAngleDifference = angleDifference;
                targetRotation = rotation;
            }
        }

        float t = 0f;

        while (t < 1f) {
            t += Time.deltaTime * turnSpeed;
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
            yield return null;
        }

        isRotating = false;
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

            RotateToTarget(lookRotationSpeed * lookRotationSpeedMultiplier, elapsedTime);

            elapsedTime += Time.fixedDeltaTime;

            yield return null;
        }
    }

    private void SwitchState(PlayerController.PlayerState state) {
        currentState = state;
    }

    private void RotateToTarget(float followRotationSpeed, float timeDelta) {
        Vector3 directionToTarget = target.position - transform.position;
        directionToTarget.y = 0f;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, timeDelta * followRotationSpeed);
    }

    private void ProcessGestureSwipes(object sender, GestureInput.SwipeDirectionChangedEventArgs e) {

        // Turn 90° in normal state
        if (currentState == PlayerState.Normal) {
            if (!isRotating) {
                if (e.swipeDirection == GestureInput.SwipeDir.Left) {

                    StartCoroutine(RotateDegrees(1));

                } else if (e.swipeDirection == GestureInput.SwipeDir.Right) {

                    StartCoroutine(RotateDegrees(-1));
                }
            }
        }

        if (currentState == PlayerState.Combat) {
            if (canAttack == true && isLimited == false) {
                canAttack = false;
                canDash = false;
                OnAttack?.Invoke(this, new OnAttackEventArgs { swipeDirection = e.swipeDirection });
            }
        }
    }

    private void Dash(object sender, Joystick.OnDoubleTapEventArgs e) {

        if (canDash == true && isLimited == false) {
            canAttack = false;
            canDash = false;
            OnDash?.Invoke(this, new OnDashEventArgs { dashPoint = e.point.normalized });
            StartCoroutine(DashRoutine(e.point));
        }
    }

    private void EnableActions_OnFinishedAction(object sender, EventArgs e) {
        canAttack = true;
        canDash = true;
    }

    private void Block(object sender, EventArgs e) {
        OnBlocking?.Invoke(this, EventArgs.Empty);
    }

    private void ReleaseBlock(object sender, EventArgs e) {
        OnReleaseBlock?.Invoke(this, EventArgs.Empty);
    }

    private void Parry(object sender, EventArgs e) {
        OnParry?.Invoke(this, EventArgs.Empty);
        OnReleaseBlock?.Invoke(this, EventArgs.Empty);
    }

    private void InheritMovementFromAnimation_OnAnimating(object sender, EventArgs e) {
        Animator animator = playerAnimator.GetComponent<Animator>();
        Vector3 deltaPosition = animator.deltaPosition;
        Vector3 worldDeltaPosition = deltaPosition;
        characterController.Move(worldDeltaPosition);
    }

    public void RunStatusTimer_OnUsingItem(object sender, PlayerAnimator.OnUsingItemEventArgs e) {
        LimitActions(e.animLength);
    }

    private void LimitActions(float time) {
        isTiming = true;
        IsLimited = true;
        speedLimitMultiplier = .5f;
        timer = time;
    }
}
