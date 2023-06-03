using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seal : MonoBehaviour
{
    [SerializeField] private float impulseForce = 10f;
    [SerializeField] private Vector3 impulseDirection;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ApplyForceWithOffset();
    }

    // Update is called once per frame
    private void ApplyForceWithOffset() {

        if (rb != null) {
            Vector3 forceVector = this.transform.forward + impulseDirection;
            rb.AddForce(forceVector.normalized * impulseForce, ForceMode.Impulse);
        }
    }
}
