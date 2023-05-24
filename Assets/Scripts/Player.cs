using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements.Experimental;

public class Player : MonoBehaviour {
    public enum PlayerState { Normal, Combat }
    public PlayerState currentState;

    [Header("Dash Settings")]
    [SerializeField] private float dashDistance;
    [SerializeField] private float dashMovementSpeed;
    [SerializeField] private bool isDashing = false;
    [SerializeField] private float followRotationDashSpeed = 4f;

    [Header("Attack Settings")]
    [SerializeField] private float attackDistance;
    [SerializeField] private float attackMovementSpeed;
    [SerializeField] private bool isAttacking = false;

    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float maxMovementSpeed = 10f;
    [SerializeField] private float accelerationTime = 2f;
    [SerializeField] private float turnSpeed = 90f;

    [Header("Orbital Settings")]
    public Transform target;
    [SerializeField] private float followRotationSpeed = 4f;

    [Header("Limited Settings")]
    [SerializeField] private float limitedSpeed = 2f;
    [SerializeField] private bool isLimited = false;
    [SerializeField] private float limitedTime = 5f;

    private CharacterController characterController;

    [Header("Camera bob factor")]
    [SerializeField] private float currentSpeed;
    private bool isRotating;

    private bool isMoving = false;

    public bool IsMoving { get { return isMoving; } }
    public float CurrentSpeed { get { return currentSpeed; } }

    [Header("External Components")]
    public Joystick joystick = null;

    public SwipeDetector swipeDetector = null;

    public event EventHandler OnDamageTaken;

    public Shake camHolder = null;

    private void Start() {
        // Set the initial state to Normal
        currentState = PlayerState.Combat;

        characterController = GetComponent<CharacterController>();
        currentSpeed = 0f;
        isRotating = false;

        // Subscribe to events
        swipeDetector.SwipeDirectionChanged += ProcessSwipeDetection;
        joystick.OnDoubleTap += Dash;
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
                    if (isDashing == false && isAttacking == false) {
                        MovePlayerNormal(inputMovement);
                        RotateToTarget(followRotationSpeed);
                    }
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
    }

    private void MovePlayerNormal(Vector2 inputMovement) {

        if (isAttacking == false) {

            Vector3 joystickDirection = new Vector3(inputMovement.x, 0f, inputMovement.y);
            Vector3 movementDirection = transform.TransformDirection((joystickDirection));

            if (movementDirection.magnitude > 0) {
                isMoving = true;
            } else {
                isMoving = false;
            }


            // Calculate the target speed based on input direction
            float targetSpeed = movementDirection.magnitude * maxMovementSpeed;

            // Lerp the current speed towards the target speed for smooth acceleration
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * accelerationTime);

            Vector3 movement = new Vector3(0, 0, 0);

            if (!isLimited) {
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

        //if (e.swipeDirection == SwipeDetector.SwipeDir.Up) {

        //    if (currentState == PlayerState.Combat) {
        //        StartCoroutine("ResetRotation");
        //    }

        //    SwitchState();
        //}

        if (isAttacking == false) {

            if ((e.swipeDirection == SwipeDetector.SwipeDir.Left ||
                   e.swipeDirection == SwipeDetector.SwipeDir.UpLeft ||
                   e.swipeDirection == SwipeDetector.SwipeDir.Up ||
                   e.swipeDirection == SwipeDetector.SwipeDir.UpRight ||
                   e.swipeDirection == SwipeDetector.SwipeDir.Right ||
                   e.swipeDirection == SwipeDetector.SwipeDir.DownRight ||
                   e.swipeDirection == SwipeDetector.SwipeDir.Down ||
                   e.swipeDirection == SwipeDetector.SwipeDir.DownLeft) &&
                   currentState == PlayerState.Combat) {

                if (isAttacking == false) {
                    StartCoroutine(AttackCoroutine(Vector3.forward, e.swipeDirection));
                }
            }
        }

            //} else if (e.swipeDirection == SwipeDetector.SwipeDir.Down) {
            //    StartCoroutine(InitiateBusyCounter(5f));
            //}

    }

    private void Dash(object sender, Joystick.OnDoubleTapEventArgs e) {

        if (isDashing == false && isAttacking == false && isLimited == false) {
            StartCoroutine(DashRoutine(e.point));
        }
    }

    private IEnumerator InitiateBusyCounter(float time) {
        isLimited = true;
        yield return new WaitForSeconds(limitedTime);
        isLimited = false;
    } 

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

        isDashing = true;

        Vector3 pointNormalized = point.normalized;
        Vector3 direction = new Vector3(pointNormalized.x, 0f, pointNormalized.y); // Ignore Y-axis

        Vector3 startPosition = characterController.transform.position;
        Vector3 targetPosition = startPosition + (characterController.transform.forward * direction.z + characterController.transform.right * direction.x).normalized * dashDistance;

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

        isDashing = false;
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

    private IEnumerator AttackCoroutine(Vector3 direction, SwipeDetector.SwipeDir swipeDirection) {
        isAttacking = true;

        Debug.Log("Attacked in direction: " + swipeDirection);

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

        isAttacking = false;
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

            StartCoroutine(CoolDown(limitedTime));

            OnDamageTaken?.Invoke(this, EventArgs.Empty);
        }
    }


}
