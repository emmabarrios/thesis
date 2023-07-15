using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class Projectile : MonoBehaviour {
    [Header("Throw Settings")]
    [SerializeField] private float throwForce;
    [SerializeField] private float throwUpwardForce;
    [SerializeField] private float rotationForce = 5f;
    [SerializeField] private float cooldown = 1f;
    [SerializeField] private Vector3 offset;

    public float ThrowForce { get { return throwForce; } }
    public float ThrowUpwardForce { get { return throwUpwardForce; } }
    public float RotationForce { get { return rotationForce; } }
    public float Cooldown { get { return cooldown; } }
    public Vector3 Offset { get { return offset; } }

    [Header("Impact Settigs")]
    [SerializeField] private float impactDamage;
    [SerializeField] private bool destroyOnImpact = false;
    [SerializeField] private GameObject impactFX;
    
    private void OnCollisionEnter(Collision other) {
        DealDamageOnImpact(other);
        if (destroyOnImpact == true) {
            Destroy(gameObject);
        }
    }

    protected void DealDamageOnImpact(Collision other) {
        BodyPart bodyPart = other.gameObject.GetComponent<BodyPart>();

        if (bodyPart != null) {
            IDamageable damageable = bodyPart.GetComponentInParent<IDamageable>();

            if (damageable != null) {
                if (other.gameObject.name != "Player") {
                    Vector3 contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

                    if (impactFX!=null) {
                        Instantiate(impactFX, contactPoint, Quaternion.identity);
                    }

                    float finalDamage;

                    // Localized damage
                    switch (bodyPart.tag) {
                        case "Head":
                            finalDamage = impactDamage + 100;
                            break;
                        case "Chest":
                            finalDamage = impactDamage + 30;
                            break;
                        default:
                            finalDamage = impactDamage;
                            break;
                    }

                    damageable.TakeDamage(finalDamage);
                    
                }
            }
        }
    }

}