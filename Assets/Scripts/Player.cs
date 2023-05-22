using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum PlayerState { Both, Horizontal, Vertical, None }

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

    private void Start() {
        characterController = GetComponent<CharacterController>();
        currentSpeed = 0f;
        isRotating = false;
    }

    private void Update() {

        Vector2 inputMovement = joystick.Direction;
        Debug.Log(inputMovement);

        // Rotate the player on "E" key press
        if (Input.GetKeyDown(KeyCode.E) && !isRotating) {
            StartCoroutine(RotatePlayer());
        }

        // Move the player
        if (!isRotating) {
            MovePlayer(inputMovement);
        }
    }

    private void MovePlayer(Vector2 inputMovement) {
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

    private IEnumerator RotatePlayer() {
        isRotating = true;

        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0f, 90f, 0f));
        float t = 0f;

        while (t < 1f) {
            t += Time.deltaTime * rotationSpeed;
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
            yield return null;
        }

        isRotating = false;
    }

}
