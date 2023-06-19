using System;
using System.Collections;
using UnityEngine;

public class Shake : MonoBehaviour
{
    private Quaternion originalRotation;
    private Transform cameraTransform;
    [SerializeField] private Player player = null;
    Controller controller = null;

    [SerializeField] private float poiseModifier;
    [SerializeField] private float duration;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float magnitude;
    [SerializeField] private int direction = 1;
    [SerializeField] private bool isrotating = false;

    [Header("Bobb Settings")]
    [SerializeField] private float bobDuration = 1f;
    [SerializeField] private float bobHeight = 1f;
    private bool isBobbing = false;

    [Header("Bobb Settings")]
    [SerializeField] private float rotationDuration = 1f;
    [SerializeField] private float rotationMagnitude = 1f;


    private Vector3 originalPosition;


    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        controller = player.GetComponent<Controller>();

        cameraTransform = GetComponent<Transform>();
        originalRotation = cameraTransform.localRotation;

        // Player Controller Event Subscribers
        player.OnDamageTaken += StartCameraShake;
        controller.OnDash += StartCameraBob;
        //controller.OnAttack += StartCameraRotation;

        originalPosition = transform.localPosition;

    }

    public void StartCameraBob(Vector2 placeholder) {
        StartCoroutine(BobCamera());
    }

    public void StartCameraShake() {
        if (isrotating == false) {
            StartCoroutine(ShakeCamera(duration, magnitude, direction));
        }
    }
    
    public void StartCameraRotation(GestureInput.SwipeDir swipeDirection) {
        //if (isrotating == false) {
        //    int dir = -1;

        //    if (swipeDirection == GestureInput.SwipeDir.Left) {
        //        dir = 1;
        //    }

        //    StartCoroutine(RotateCamera(duration, magnitude, dir));
        //}

        //gameObject.GetComponent<Animator>().Play("cam_swing_rotation", -1, 0f);

    }

    private IEnumerator ShakeCamera(float duration, float magnitude, int direction) {
        Quaternion rotation = originalRotation * Quaternion.Euler(0f, 0f, magnitude * direction);

        //Reset the camera's rotation to the original rotation
        cameraTransform.localRotation = originalRotation;

        float _poise = player.Poise;

        if (_poise > 1) {
            rotation = originalRotation * Quaternion.Euler(0f, 0f, magnitude * poiseModifier * direction);
        }
        cameraTransform.localRotation = rotation;

        while (cameraTransform.localRotation != originalRotation) {
            // Apply the rotation towards the original rotation
            cameraTransform.localRotation = Quaternion.Lerp(cameraTransform.localRotation, originalRotation, Time.fixedDeltaTime * rotationSpeed);

            if (Mathf.Abs(cameraTransform.localRotation.z - originalRotation.z) < 0.01) {
                cameraTransform.localRotation = originalRotation;
            }
        }
        yield return null;

    }
    
    private IEnumerator BobCamera() {
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

    private IEnumerator RotateCamera(float duration, float magnitude, int direction) {
        Quaternion startRotation = cameraTransform.localRotation;
        Quaternion targetRotation = originalRotation * Quaternion.Euler(0f, 0f, magnitude * direction);

        float elapsedTime = 0f;
        float halfDuration = duration / 2f;

        while (elapsedTime < duration) {
            // Calculate the interpolation factor based on the elapsed time and duration
            float t = elapsedTime / halfDuration;

            // Interpolate between startRotation and targetRotation for the first half
            if (elapsedTime < halfDuration) {
                Quaternion rotation = Quaternion.Lerp(startRotation, targetRotation, t);
                cameraTransform.localRotation = rotation;
            }
            // Interpolate between targetRotation and startRotation for the second half
            else {
                t = (elapsedTime - halfDuration) / halfDuration;
                Quaternion rotation = Quaternion.Lerp(targetRotation, startRotation, t);
                cameraTransform.localRotation = rotation;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the camera ends up at the original rotation
       // cameraTransform.localRotation = originalRotation;
    }
}
