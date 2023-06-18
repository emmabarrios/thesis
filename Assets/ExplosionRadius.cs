using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionRadius : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        BodyPart bodyPart = other.gameObject.GetComponent<BodyPart>();

        if (bodyPart != null) {
            IDamageable damageable = bodyPart.GetComponentInParent<IDamageable>();
            Transform collisionTransformRoot = bodyPart.GetComponentInParent<Transform>().root;
            if (damageable != null) {
                if (other.gameObject.name != "Player") {
                    Vector3 contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                    damageable.TakeDamage(GetComponentInParent<Bomb>().damage);
                }
            }
        }
    }
}
