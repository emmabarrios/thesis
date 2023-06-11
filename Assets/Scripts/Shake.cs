using System;
using System.Collections;
using UnityEngine;

public class Shake : MonoBehaviour
{
    private Quaternion originalRotation;
    private Transform cameraTransform;
    [SerializeField] private Player player = null;

    [SerializeField] private float poiseModifier;
    [SerializeField] private float duration;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float magnitude;
    [SerializeField] private int direction = 1;
    [SerializeField] private bool isrotating = false;

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        cameraTransform = GetComponent<Transform>();
        originalRotation = cameraTransform.localRotation;

        // Player Controller Event Subscribers
        player.OnDamageTaken += StartCameraShake;
    }

    public void StartCameraShake() {
        if (isrotating == false) {
            StartCoroutine(ShakeCamera(duration, magnitude, direction));
        }
    }


    private IEnumerator ShakeCamera(float duration, float magnitude, int direction) {
        // Reset the camera's rotation to the original rotation
        cameraTransform.localRotation = originalRotation;

        float _poise = player.Poise;

        Quaternion rotation = originalRotation * Quaternion.Euler(0f, 0f, magnitude * direction);

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

            yield return null;
        }

    }
}
