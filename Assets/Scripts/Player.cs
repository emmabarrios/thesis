using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {
    public enum PlayerState { Normal, Combat }
    public PlayerState currentState;

    [SerializeField] Transform playerVisual = null;

    [Header("Weapon Cooldown")]
    [SerializeField] private float swipeLeftTimer;
    [SerializeField] private float swipeRightTimer;
    [SerializeField] private float swipeUpTimer;
    [SerializeField] private float swipeDownTimer;
    [SerializeField] private bool isCooling;
    private float coolDownTimer;

    [Header("Dash Settings")]
    [SerializeField] private float dashDistance;
    [SerializeField] private float dashMovementSpeed;
    [SerializeField] private float dirMultiplier = 1f;
    [SerializeField] private float distanceFactor = 100f;
    [SerializeField] private bool canDash = true;
    private Vector2 dashPoint;
    public Vector2 DashPoint { get { return dashPoint; } set { dashPoint = value; } }
    [SerializeField] private float followRotationDashSpeed = 4f;

    [Header("Orbital Settings")]
    public Transform target;
    [SerializeField] private float followRotationSpeed = 4f;

    [Header("Attack Settings")]
    [SerializeField] private bool canAttack = true;
    public bool CanAttack { get { return canAttack; } }

    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float maxMovementSpeed = 10f;
    [SerializeField] private float accelerationTime = 2f;
    [SerializeField] private float deaccelerationTime = 2f;
    [SerializeField] private float turnSpeed = 90f;
    [SerializeField] private bool isWalking;
    public bool IsWalking { get { return isWalking; } set { isWalking = value; } }
    private Vector3 last_movement;
    private float last_speed;


    

    [Header("Limited Settings")]
    [SerializeField] private float limitedSpeed = 2f;
    [SerializeField] private float timer;
    [SerializeField] private float limitedTime = 5f;
    [SerializeField] private float damageCoolDown = 2f;
    [SerializeField] private bool isTiming;
    [SerializeField] private bool isLimited = false;
    public bool IsLimited { get { return isLimited; } }

    [Header("Weapon Anchors")]
    [SerializeField] private Transform pointL;
    [SerializeField] private Transform pointR;

    private CharacterController characterController;

    [Header("Camera bob factor")]
    [SerializeField] private float currentSpeed;
    private bool isRotating;

    public float CurrentSpeed { get { return currentSpeed; } }

    [Header("External Components")]
    public Joystick joystick = null;

    public SwipeDetector swipeDetector = null;

    public Button buttonA = null;

    public event EventHandler OnDamageTaken;
    
    public event EventHandler OnDash;
    public event EventHandler OnBlocking;
    public event EventHandler OnReleaseBlock;
    public event EventHandler OnParry;
    public event EventHandler<OnAttackEventArgs> OnAttack;

    public class OnAttackEventArgs: EventArgs {
        public SwipeDetector.SwipeDir swipeDirection;
    }

    public Shake camHolder = null;

    private PlayerAnimator playerAnimator;

    private bool isBlocking = false;
    public bool IsBlocking { get { return isBlocking; } set { IsBlocking = value; } }

    private void Start() {
        currentState = PlayerState.Combat;

        playerAnimator = this.GetComponentInChildren<PlayerAnimator>();
        playerAnimator.OnUsingItem += EnterBusytate;
        playerAnimator.OnFinishedUsingItem += ExitBusyState;
        playerAnimator.OnAnimating += InheritMovementFromAnimation;
        playerAnimator.OnFinishedAction += EnableActions;

        buttonA.OnBlocking += Block;
        buttonA.OnHandleDroped += ReleaseBlock;
        buttonA.OnParry += Parry;

        characterController = GetComponent<CharacterController>();
        currentSpeed = 0f;
        isRotating = false;

        swipeDetector.SwipeDirectionChanged += ProcessSwipeDetection;
        joystick.OnDoubleTap += Dash;

        pointL = GameObject.Find("Attach Point L").GetComponent<Transform>();
        pointR = GameObject.Find("Attach Point R").GetComponent<Transform>();
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

        // Timer 
        if (isTiming) {
            timer -= Time.deltaTime;
            if (timer < 0.1f) {
                timer = 0;
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
                movement = movementDirection.normalized * currentSpeed * Time.deltaTime;
                last_movement = movement;
                RotateToTarget(followRotationSpeed, Time.deltaTime);
            } else {

                if (currentSpeed > 1) {
                    currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * deaccelerationTime);
                } else {
                    currentSpeed = 0;
                }
                movement = last_movement.normalized * currentSpeed * Time.deltaTime;
                RotateToTarget(followRotationSpeed, Time.deltaTime);
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

    private IEnumerator ResetRotationDegrees() {
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

            RotateToTarget(followRotationDashSpeed, elapsedTime);

            elapsedTime += Time.fixedDeltaTime;

            yield return null;
        }
    }

    private void SwitchState() {
        if (currentState == PlayerState.Normal) {
            currentState = PlayerState.Combat;
        } else {
            currentState = PlayerState.Normal;
        }
    }

    private void RotateToTarget(float followRotationSpeed, float timeDelta) {
        // Calculate the direction from player to target
        Vector3 directionToTarget = target.position - transform.position;

        // Ignore the vertical component of the direction
        directionToTarget.y = 0f;

        // Rotate the player towards the target direction
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, timeDelta * followRotationSpeed);

    }

    private void ProcessSwipeDetection(object sender, SwipeDetector.SwipeDirectionChangedEventArgs e) {

        if (!isRotating) {

            if (e.swipeDirection == SwipeDetector.SwipeDir.Left && currentState == PlayerState.Normal) {

                StartCoroutine(RotateDegrees(1));

            } else if (e.swipeDirection == SwipeDetector.SwipeDir.Right && currentState == PlayerState.Normal) {

                StartCoroutine(RotateDegrees(-1));
            }
        }


        if (canAttack == true && isLimited == false) {
            //StartCoroutine(AttackCoroutine(Vector3.forward));
            canAttack = false;
            canDash = false;
            OnAttack?.Invoke(this, new OnAttackEventArgs { swipeDirection = e.swipeDirection });
            //StarWeaponCooldownTimer(e.swipeDirection);
        }

        //if (isAttacking == false) {

        //    if ((e.swipeDirection == SwipeDetector.SwipeDir.Left ||
        //           e.swipeDirection == SwipeDetector.SwipeDir.UpLeft ||
        //           e.swipeDirection == SwipeDetector.SwipeDir.Up ||
        //           e.swipeDirection == SwipeDetector.SwipeDir.UpRight ||
        //           e.swipeDirection == SwipeDetector.SwipeDir.Right ||
        //           e.swipeDirection == SwipeDetector.SwipeDir.DownRight ||
        //           e.swipeDirection == SwipeDetector.SwipeDir.Down ||
        //           e.swipeDirection == SwipeDetector.SwipeDir.DownLeft) &&
        //           currentState == PlayerState.Combat) {

        //        if (isDashing == false && isAttacking == false && isLimited == false) {
        //            StartCoroutine(AttackCoroutine(Vector3.forward, e.swipeDirection));
        //        }
        //    }
        //}

    }

    private void Dash(object sender, Joystick.OnDoubleTapEventArgs e) {

        if (canDash == true && isCooling == false && isLimited == false) {
            canDash = false;
            OnDash?.Invoke(this, EventArgs.Empty);
            DashPoint = e.point.normalized;
            StartCoroutine(DashRoutine(e.point));
        }
    }

    public void ToggleWeaponsVisuals(bool toggle) {
        if (toggle) {
            pointL.GetChild(0).gameObject.SetActive(false);
            pointR.GetChild(0).gameObject.SetActive(false);
        } else {
            pointL.GetChild(0).gameObject.SetActive(true);
            pointR.GetChild(0).gameObject.SetActive(true);
        }
        
    }

    private void EnterBusytate(object sender, EventArgs e) {
        pointL.GetChild(0).gameObject.SetActive(false);
        pointR.GetChild(0).gameObject.SetActive(false);
    }
    
    private void ExitBusyState(object sender, EventArgs e) {
        pointL.GetChild(0).gameObject.SetActive(true);
        pointR.GetChild(0).gameObject.SetActive(true);
    }

    private void EnableActions(object sender, EventArgs e) {
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

    private void InheritMovementFromAnimation(object sender, EventArgs e) {
        Animator animator = playerAnimator.GetComponent<Animator>();
        Vector3 deltaPosition = animator.deltaPosition;

        Vector3 worldDeltaPosition = deltaPosition;

        characterController.Move(worldDeltaPosition);
    }
}
