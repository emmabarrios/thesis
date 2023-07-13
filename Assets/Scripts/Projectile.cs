using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour 
{
    [Header("Throw Settings")]
    [SerializeField] private float throwForce;
    [SerializeField] private float throwUpwardForce;
    [SerializeField] private float rotationForce = 5f;
    [SerializeField] private Vector3 offset;

    [Header("Timer Settings")]
    public float timer;
    private bool isTiming;

    [Header("Bullet Settigs")]
    [SerializeField] private float impactDamage;
    [SerializeField] private bool destroyOnImpact = false;
    [SerializeField] private GameObject impactFX;

    [Header("Bomb Settings")]
    [SerializeField] private float explosionDamage;
    [SerializeField] private GameObject explosionRadius;
    [SerializeField] private GameObject explosionFX;

    private void Start() {
        if (timer != 0f) isTiming = true;
    }

    private void Update() {

        // Bomb timer
        if (isTiming) {
            timer -= Time.deltaTime;
            if (timer < 0.1f) {
                timer = 0; ;
                isTiming = false;
                if (explosionRadius!=null) {
                    GameObject e = Instantiate(explosionRadius, transform.position, Quaternion.identity);
                    ExplosionRadius _e = e.GetComponent<ExplosionRadius>();
                    _e.ExplosionDamage = explosionDamage;
                    _e.Timer = timer;
                }
                Instantiate(explosionFX, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        DealDamageOnImpact(other);
    }

    protected void DealDamageOnImpact(Collider other) {
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
                    if (destroyOnImpact==true) {
                        Destroy(gameObject);
                    }
                }
            }
        }
    }



}