using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum PlayerState { Normal, Combat}
    public PlayerState currentState;

    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float maxMovementSpeed = 10f;
    [SerializeField] private float accelerationTime = 2f;
    [SerializeField] private float rotationSpeed = 90f;

    private CharacterController characterController;
    private float currentSpeed;
    private bool isRotating;

    private bool isMoving = false;

    public bool IsMoving { get { return isMoving; } }
    public float CurrentSpeed { get { return currentSpeed; } }
    public Joystick joystick = null;

    public SwipeDetector swipeDetector = null;

    [Header("Orbital Settings")]
    public Transform target; // Reference to the transform for rotation

    private void Start() {
        // Set the initial state to Normal
        currentState = PlayerState.Normal;

        characterController = GetComponent<CharacterController>();
        currentSpeed = 0f;
        isRotating = false;
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
                    MovePlayerOrbital(inputMovement);
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
        if (Input.GetKeyDown(KeyCode.Space)) {

            if (currentState == PlayerState.Combat) {
                StartCoroutine("ResetRotation");
            }

            SwitchState();
           
        }
    }

    private void MovePlayerNormal(Vector2 inputMovement) {
        // Rotate the movement direction from local to world space
        Vector3 movementDirection = new Vector3(inputMovement.x, 0f, inputMovement.y);
        Vector3 worldMovementDirection = transform.TransformDirection((movementDirection));

        if (worldMovementDirection.magnitude>0) {
            isMoving = true;
        } else {
            isMoving= false;
        }

        // Calculate the target speed based on input direction
        float targetSpeed = worldMovementDirection.magnitude * maxMovementSpeed;

        // Lerp the current speed towards the target speed for smooth acceleration
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * accelerationTime);

        // Apply the movement to the player
        Vector3 movement = worldMovementDirection * currentSpeed * Time.deltaTime;
        characterController.Move(movement);
    }

    private void MovePlayerOrbital(Vector3 inputMovement) {
        MovePlayerNormal(inputMovement);
        RotatePlayer();
    }

    private IEnumerator RotatePlayer(int dir) {

        isRotating = true;

        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0f, 90f * dir, 0f));
        float t = 0f;

        while (t < 1f) {
            t += Time.deltaTime * rotationSpeed;
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

    private void RotatePlayer() {
        // Calculate the direction from player to target
        Vector3 directionToTarget = target.position - transform.position;

        // Ignore the vertical component of the direction
        directionToTarget.y = 0f;

        // Rotate the player towards the target direction
        if (directionToTarget != Vector3.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            //transform.rotation = targetRotation;

        }
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
            t += Time.deltaTime * rotationSpeed;
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
            yield return null;
        }

        isRotating = false;
    }
}
