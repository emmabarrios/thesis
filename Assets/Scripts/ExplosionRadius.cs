using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionRadius : MonoBehaviour
{
    [Header("Lifetime Timer Settings")]
    private float timer;
    public float Timer { set { value = timer; } }
    private bool isTiming;

    private float explosionDamage;
    public float ExplosionDamage { get { return explosionDamage; } set {  explosionDamage = value; } }

    private void Start() {
        isTiming = true;
    }

    private void Update() {
        if (isTiming) {
            timer -= Time.deltaTime;
            if (timer < 0.1f) {
                timer = 0;
                isTiming = false;
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        BodyPart bodyPart = other.gameObject.GetComponent<BodyPart>();

        if (bodyPart != null) {
            IDamageable damageable = bodyPart.GetComponentInParent<IDamageable>();
            //Transform collisionTransformRoot = bodyPart.GetComponentInParent<Transform>().root;
            if (damageable != null) {
                if (other.gameObject.name != "Player") {
                    Vector3 contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                    damageable.TakeDamage(explosionDamage);
                    Destroy(gameObject);
                }
            }
        }
    }
}
