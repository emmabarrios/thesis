using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : Projectile
{
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "BodyPart") {
            DealDamageOnImpact(other);
        }

        if ((other.tag == "BodyPart" || other.tag == "Terrain") && destroyOnImpact == true) {
            if (impactFX != null) {
                Vector3 contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                Instantiate(impactFX, contactPoint, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }

    protected void DealDamageOnImpact(Collider other) {
        BodyPart bodyPart = other.gameObject.GetComponent<BodyPart>();

        if (bodyPart != null) {
            IDamageable damageable = bodyPart.GetComponentInParent<IDamageable>();

            if (damageable != null) {
                if (other.gameObject.name != "Player") {

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
