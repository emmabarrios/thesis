using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamHolder : MonoBehaviour
{
    [SerializeField] private Transform playerTransform = null;
    [SerializeField] private Transform lookTargetTransform = null;
    public float yOffset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Get the player's position and add the offset
        Vector3 targetPosition = playerTransform.position + Vector3.up * yOffset;

        // Set the camera's position to the target position
        transform.position = targetPosition;
    }
}
