using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thrower : MonoBehaviour
{
    public void Throw(GameObject projectile, float throwForce, float throwUpwardForce, Vector3 offset) {
        GameObject _projectile = Instantiate(projectile, transform.position + offset, transform.rotation);
        Vector3 force = transform.forward * throwForce + (transform.up * throwUpwardForce);
        Rigidbody rb = _projectile.GetComponent<Rigidbody>();
        rb.AddForce(force, ForceMode.Impulse);
    }
}
