using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float damage;
    public float time;
    public GameObject explosionFX;
    public GameObject explosionRadius;
    
    private void Start() {
        StartCoroutine(CountDown());
    }

    IEnumerator CountDown() {
        yield return new WaitForSeconds(time);
        explosionRadius.GetComponent<SphereCollider>().enabled = true;
        yield return new WaitForSeconds(0.5f);
        Explode();
    }

    private void Explode() {
       
        Instantiate(explosionFX, transform.localPosition, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        BodyPart bodyPart = other.gameObject.GetComponent<BodyPart>();

        if (bodyPart != null) {
            IDamageable damageable = bodyPart.GetComponentInParent<IDamageable>();
            Transform collisionTransformRoot = bodyPart.GetComponentInParent<Transform>().root;
            if (damageable != null) {
                if (other.gameObject.name != "Player") {

                    Vector3 contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

                    Instantiate(explosionFX, contactPoint, Quaternion.identity);

                    damageable.TakeDamage(damage);
                    Destroy(gameObject);
                }
            }
        }
    }
}
