using System;
using System.Collections;
using UnityEngine;

public class CamHolderMotion : MonoBehaviour
{
    private Animator animator;
    private Quaternion originalRotation;
    private Transform cameraTransform;
    [SerializeField] private Player player = null;
    [SerializeField] private PlayerAnimator playerAnimator = null;
    Controller controller = null;

    [Header("Shake Settings")]
    [SerializeField] private float poiseModifier;
    [SerializeField] private float duration;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float magnitude;
    [SerializeField] private int direction = 1;
    [SerializeField] private bool isrotating = false;

    [Header("Bobb Settings")]
    [SerializeField] private float bobDuration = 1f;
    [SerializeField] private float bobHeight = 1f;
    [SerializeField] private float pointXThreshold = 1f;
    private bool isBobbing = false;

    private Vector3 originalPosition;


    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        controller = player.GetComponent<Controller>();
        animator = GetComponent<Animator>();
        
        cameraTransform = GetComponent<Transform>();
        originalRotation = cameraTransform.localRotation;

        // Player Controller Event Subscribers
        player.OnDamageTaken += StartCameraShake;
        controller.OnDash += StartCameraBob;
        playerAnimator.OnSwingLeft += PlayRotateLeftAnimation;
        playerAnimator.OnSwingRight += PlayRotateRightAnimation;
        originalPosition = transform.localPosition;

    }

    public void StartCameraBob(int direction) {

        if (Mathf.Abs(direction) > pointXThreshold) {
            if (direction > 0) {
                animator.Play("rotate_right");
            } else {
                animator.Play("rotate_left");
            }
        }
        StartCoroutine(BobCamera(direction));
    }

    private void PlayRotateLeftAnimation() {
        animator.Play("rotate_left");
    }
    
    private void PlayRotateRightAnimation() {
        animator.Play("rotate_right");
    }

    public void StartCameraShake() {
        if (isrotating == false) {
            StartCoroutine(ShakeCamera( magnitude, direction));
        }
    }

    private IEnumerator ShakeCamera(float magnitude, int direction) {

        // Take away controll from the animator controller
        GetComponent<Animator>().enabled = false;

        Quaternion rotation = originalRotation * Quaternion.Euler(0f, 0f, magnitude * direction);

        //Reset the camera's rotation to the original rotation
        cameraTransform.localRotation = originalRotation;

        rotation = originalRotation * Quaternion.Euler(0f, 0f, magnitude * poiseModifier * direction);

        cameraTransform.localRotation = rotation;

        while (cameraTransform.localRotation != originalRotation) {
            // Apply the rotation towards the original rotation
            cameraTransform.localRotation = Quaternion.Lerp(cameraTransform.localRotation, originalRotation, Time.fixedDeltaTime * rotationSpeed);

            if (Mathf.Abs(cameraTransform.localRotation.z - originalRotation.z) < 0.01) {
                cameraTransform.localRotation = originalRotation;
            }
            yield return null;
        }
        // Return controll from the animator controller
        GetComponent<Animator>().enabled = true;
    }
    
    private IEnumerator BobCamera(int dir) {
        isBobbing = true;
        float elapsedTime = 0f;

        while (elapsedTime < bobDuration) {
            float progress = elapsedTime / bobDuration;
            float yOffset = Mathf.Sin(progress * Mathf.PI) * bobHeight;

            transform.localPosition = originalPosition - new Vector3(0f, yOffset, 0f);

            elapsedTime += Time.fixedDeltaTime;
            yield return null;
        }

        //transform.localPosition = originalPosition - new Vector3(0f, bobHeight, 0f);
        transform.localPosition = originalPosition;
        isBobbing = false;
    }

}
