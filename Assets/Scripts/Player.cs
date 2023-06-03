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
    [SerializeField] private bool canDash = false;
    //public bool IsDashing { get { return canDash; } }
    [SerializeField] private float followRotationDashSpeed = 4f;

    [Header("Attack Settings")]
    [SerializeField] private float attackDistance;
    [SerializeField] private float attackMovementSpeed;
    [SerializeField] private bool canAttack = true;
    public bool IsAttacking { get { return canAttack; } }

    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float maxMovementSpeed = 10f;
    [SerializeField] private float accelerationTime = 2f;
    [SerializeField] private float turnSpeed = 90f;
    [SerializeField] private bool isWalking;
    public bool IsWalking { get { return isWalking; } set { isWalking = value; } }


    [Header("Orbital Settings")]
    public Transform target;
    [SerializeField] private float followRotationSpeed = 4f;

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
    //public event EventHandler OnAttacking;

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
        // Set the initial state to Normal
        currentState = PlayerState.Combat;

        playerAnimator = this.GetComponentInChildren<PlayerAnimator>();
        playerAnimator.OnUsingItem += EnterBusytate;
        playerAnimator.OnFinishedUsingItem += ExitBusyState;
        playerAnimator.OnAnimating += InheritMovementFromAnimation;
        buttonA.OnBlocking += Block;
        buttonA.OnHandleDroped += ReleaseBlock;
        buttonA.OnParry += Parry;

        playerAnimator.OnFinishedAction += EnableActions;
        //playerAnimator.OnEnterAttack += EnterAttackState;

        characterController = GetComponent<CharacterController>();
        currentSpeed = 0f;
        isRotating = false;

        // Subscribe to events
        swipeDetector.SwipeDirectionChanged += ProcessSwipeDetection;
        joystick.OnDoubleTap += Dash;


        pointL = GameObject.Find("Attach Point L").GetComponent<Transform>();
        pointR = GameObject.Find("Attach Point R").GetComponent<Transform>();

        
    }

    private void ButtonA_OnBlocking(object sender, Button.OnBlockingEventArgs e) {
        throw new NotImplementedException();
    }

    private void Update() {

        Vector2 inputMovement = joystick.Direction;

        // Update logic based on the current state

        if (!isRotating) {

            switch (currentState) {
                case PlayerState.Normal:
                    MovePlayerNormal(inputMovement);
                    break;

                case PlayerState.Combat:
                    MovePlayerNormal(inputMovement);
                    RotateToTarget(followRotationSpeed);
                    break;

                default:
                    break;

            }
        }

        // Rotate 90° in the passed direction.
        if (Input.GetKeyDown(KeyCode.E) && !isRotating) {
            StartCoroutine(RotatePlayer(1));
        } else if (Input.GetKeyDown(KeyCode.Q) && !isRotating) {
            StartCoroutine(RotatePlayer(-1));
        }

        // Toggle current state
        //if (Input.GetKeyDown(KeyCode.Space)) {

        //    if (currentState == PlayerState.Combat) {
        //        StartCoroutine("ResetRotation");
        //    }

        //    SwitchState();

        //}

        // Timer 
        if (isTiming) {
            timer -= Time.deltaTime;
            if (timer < 0.1f) {
                timer = 0;
                isLimited = false;
                isTiming = false;
            }
        } 
        
        // Weapon Cooldown Timer 
        //if (isCooling) {
        //    coolDownTimer -= Time.deltaTime;
        //    if (coolDownTimer < 0.1f) {
        //        coolDownTimer = 0;
        //        isCooling = false;
        //    }
        //}
    }

    private void MovePlayerNormal(Vector2 inputMovement) {

        if (canAttack == true) {

            Vector3 joystickDirection = new Vector3(inputMovement.x, 0f, inputMovement.y);
            Vector3 movementDirection = transform.TransformDirection((joystickDirection));

            if (movementDirection.magnitude > 0.1) {
                IsWalking = true;
            } else {
                IsWalking = false;
            }

            // Calculate the target speed based on input direction
            float targetSpeed = movementDirection.magnitude * maxMovementSpeed;

            // Lerp the current speed towards the target speed for smooth acceleration
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * accelerationTime);

            Vector3 movement = new Vector3(0, 0, 0);

            if (isLimited == false) {
                // Apply the movement to the player
                movement = movementDirection * movementSpeed * Time.deltaTime;
            } else {
                // Apply the movement to the player
                movement = movementDirection * limitedSpeed * Time.deltaTime;
            }

            characterController.Move(movement);
        }
    }

    private IEnumerator RotatePlayer(int dir) {

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

    private IEnumerator ResetRotation() {
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

    private void SwitchState() {
        if (currentState == PlayerState.Normal) {
            currentState = PlayerState.Combat;
        } else {
            currentState = PlayerState.Normal;
        }
    }

    private void RotateToTarget(float followRotationSpeed) {
        // Calculate the direction from player to target
        Vector3 directionToTarget = target.position - transform.position;

        // Ignore the vertical component of the direction
        directionToTarget.y = 0f;

        // Rotate the player towards the target direction
        if (directionToTarget != Vector3.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * followRotationSpeed);
            //transform.rotation = targetRotation;
        }

    }

    private void _RotateToTarget(float followRotationSpeed) {
        // Calculate the direction from player to target
        Vector3 directionToTarget = target.position - transform.position;

        // Ignore the vertical component of the direction
        directionToTarget.y = 0f;

        // Rotate the player towards the target direction
        if (directionToTarget != Vector3.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.fixedDeltaTime * followRotationSpeed);
            //transform.rotation = targetRotation;
        }

    }

    private void ProcessSwipeDetection(object sender, SwipeDetector.SwipeDirectionChangedEventArgs e) {

        if (!isRotating) {

            if (e.swipeDirection == SwipeDetector.SwipeDir.Left && currentState == PlayerState.Normal) {

                StartCoroutine(RotatePlayer(1));

            } else if (e.swipeDirection == SwipeDetector.SwipeDir.Right && currentState == PlayerState.Normal) {

                StartCoroutine(RotatePlayer(-1));
            }
        }


        if (canAttack == true && isLimited == false) {
            //StartCoroutine(AttackCoroutine(Vector3.forward));
            canAttack = false;
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
            StartCoroutine(DashRoutine(e.point));
        }
    }

    //private IEnumerator InitiateBusyCounter(float time) {
    //    isLimited = true;
    //    yield return new WaitForSeconds(limitedTime);
    //    isLimited = false;
    //} 

    private IEnumerator Attack(SwipeDetector.SwipeDir direction) {

        yield return new WaitForSeconds(3f);

    }

    // Coroutine to move the object
    //private IEnumerator DashRoutine(Vector2 point) {
    //    isDashing = true;

    //    Vector3 pointNormalized = point.normalized;
    //    Vector3 direction = new Vector3(pointNormalized.x, 0f, pointNormalized.y);  // Ignore Y-axis

    //    Vector3 startPosition = characterController.transform.position;
    //    Vector3 targetPosition = startPosition + (characterController.transform.forward * direction.z + characterController.transform.right * direction.x).normalized * dashDistance;

    //    float elapsedTime = 0f;

    //    while (elapsedTime < 1f) {
    //        // Calculate the current position based on the interpolation between start and target positions
    //        Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime);

    //        // Move the character controller towards the current position (ignoring Y-axis)
    //        characterController.Move(new Vector3(currentPosition.x - startPosition.x, 0f, currentPosition.z - startPosition.z));

    //        // Update the elapsed time
    //        elapsedTime += Time.deltaTime * dashMovementSpeed;

    //        yield return null;
    //    }

    //    // Ensure the character controller reaches the exact target position
    //    characterController.Move(new Vector3(targetPosition.x - startPosition.x, 0f, targetPosition.z - startPosition.z));

    //    isDashing = false;
    //}

    private IEnumerator DashRoutine(Vector2 point) {

        Vector3 pointNormalized = point.normalized;
        Vector3 direction = new Vector3(pointNormalized.x, 0f, pointNormalized.y); // Ignore Y-axis

        Vector3 startPosition = characterController.transform.position;
        Vector3 targetPosition = startPosition + (characterController.transform.forward * direction.z + characterController.transform.right * direction.x).normalized * dashDistance * ProcessDirMultiplier(point);

        float elapsedTime = 0f;
        float step = 0.02f; // Fixed time step for movement calculation

        while (elapsedTime < 1f) {

            // Calculate the current position based on the interpolation between start and target positions
            Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime);

            // Move the character controller towards the current position (ignoring Y-axis)
            characterController.Move(new Vector3(currentPosition.x - startPosition.x, 0f, currentPosition.z - startPosition.z));

            // Update the elapsed time
            elapsedTime += step * dashMovementSpeed;

            _RotateToTarget(followRotationDashSpeed);

            yield return new WaitForSeconds(step);
        }
        // Ensure the character controller reaches the exact target position
        //characterController.Move(new Vector3(targetPosition.x - startPosition.x, 0f, targetPosition.z - startPosition.z));

    }

    //private IEnumerator AttackCoroutine(Vector3 direction, SwipeDetector.SwipeDir swipeDirection) {

    //    isAttacking = true;

    //    Debug.Log("Attacked in direction: " + swipeDirection);

    //    Vector3 startPosition = characterController.transform.position;
    //    Vector3 targetPosition = startPosition + (characterController.transform.forward * direction.z + characterController.transform.right * direction.x).normalized * attackDistance;

    //    float elapsedTime = 0f;

    //    while (elapsedTime < 1f) {
    //        // Calculate the current position based on the interpolation between start and target positions
    //        Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime);

    //        // Move the character controller towards the current position (ignoring Y-axis)
    //        characterController.Move(new Vector3(currentPosition.x - startPosition.x, 0f, currentPosition.z - startPosition.z));

    //        // Update the elapsed time
    //        elapsedTime += Time.deltaTime * attackMovementSpeed;

    //        yield return null;
    //    }

    //    // Ensure the character controller reaches the exact target position
    //    characterController.Move(new Vector3(targetPosition.x - startPosition.x, 0f, targetPosition.z - startPosition.z));

    //    isAttacking = false;
    //}

    private IEnumerator AttackCoroutine(Vector3 direction, SwipeDetector.SwipeDir e) {
        //isAttacking = true;

        Vector3 startPosition = characterController.transform.position;
        Vector3 targetPosition = startPosition + (characterController.transform.forward * direction.z + characterController.transform.right * direction.x).normalized * attackDistance;

        float elapsedTime = 0f;
        float step = 0.02f; // Fixed time step for movement calculation

        while (elapsedTime < 1f) {
            // Calculate the current position based on the interpolation between start and target positions
            Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime);

            // Move the character controller towards the current position (ignoring Y-axis)
            characterController.Move(new Vector3(currentPosition.x - startPosition.x, 0f, currentPosition.z - startPosition.z));

            // Update the elapsed time
            elapsedTime += step * attackMovementSpeed;

            yield return new WaitForSeconds(step);
        }

        // Ensure the character controller reaches the exact target position
        characterController.Move(new Vector3(targetPosition.x - startPosition.x, 0f, targetPosition.z - startPosition.z));

        //isAttacking = false;
    }
    
    private IEnumerator AttackCoroutine(Vector3 direction) {
       
        Vector3 startPosition = characterController.transform.position;
        Vector3 targetPosition = startPosition + (characterController.transform.forward * direction.z + characterController.transform.right * direction.x).normalized * attackDistance;

        float elapsedTime = 0f;
        float step = 0.02f; // Fixed time step for movement calculation

        while (elapsedTime < 1f) {
            // Calculate the current position based on the interpolation between start and target positions
            Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime);

            // Move the character controller towards the current position (ignoring Y-axis)
            characterController.Move(new Vector3(currentPosition.x - startPosition.x, 0f, currentPosition.z - startPosition.z));

            // Update the elapsed time
            elapsedTime += step * attackMovementSpeed;

            yield return new WaitForSeconds(step);
        }

        // Ensure the character controller reaches the exact target position
        characterController.Move(new Vector3(targetPosition.x - startPosition.x, 0f, targetPosition.z - startPosition.z));

    }

    private IEnumerator CoolDown(float timer) {
        if (isLimited == false) {
            isLimited = true; // Reset the boolean value
        }

        yield return new WaitForSeconds(timer);

        isLimited = false; // Set the boolean value
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Damage") {

            //StartCoroutine(CoolDown(limitedTime));

            StartLimitedTimerState(damageCoolDown);

            OnDamageTaken?.Invoke(this, EventArgs.Empty);
        }
    }

    private float ProcessDirMultiplier(Vector2 dir) {
        if(Mathf.Abs(dir.x) >= distanceFactor) {
            return dirMultiplier;
        }
        return 1f;
    }

    public void ToggleWeapons(bool toggle) {
        if (toggle) {
            pointL.GetChild(0).gameObject.SetActive(false);
            pointR.GetChild(0).gameObject.SetActive(false);
        } else {
            pointL.GetChild(0).gameObject.SetActive(true);
            pointR.GetChild(0).gameObject.SetActive(true);
        }
        
    }

    public void StartLimitedTimerState(float time) {
        timer = time;
        isTiming = true;
        isLimited = true;
    }

    public void StarWeaponCooldownTimer(SwipeDetector.SwipeDir swipeDir) {
        
        //isAttacking = true;

        float _time;

        switch (swipeDir) {
            case SwipeDetector.SwipeDir.Left:
                _time = swipeLeftTimer;
                break;

            case SwipeDetector.SwipeDir.Right:
                _time = swipeRightTimer;
                break;

            case SwipeDetector.SwipeDir.Up:
                _time = swipeUpTimer;
                break;

            case SwipeDetector.SwipeDir.Down:
                _time = swipeDownTimer;
                break;

            default:
                _time = 0;
                break;

        }

        coolDownTimer = _time;

        isCooling = true;
    }

    private void EnterBusytate(object sender, EventArgs e) {
        StartLimitedTimerState(limitedTime);
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

        //if (canAttack == true) {
            
        //}
    }
}
