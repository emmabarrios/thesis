using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Bomb : MonoBehaviour
{
    [Header("Parent")]
    [SerializeField] Transform parent;

    [Header("Explosion Settings")]
    [SerializeField] private float explosionDamage;
    [SerializeField] private GameObject explosionFX;

    [Header("Timer Settings")]
    [Range(0f, 10f)] public float timer;
    [Range(0f, 10f)] public float activeRadiusThreshold;
    private bool isTiming;
    private bool isRadiusActive;

    

    private void Start() {
        isTiming = true;
        isRadiusActive = false;
        parent = this.transform.parent;
    }

    private void Update() {

        // Bomb timer
        if (isTiming) {
            timer -= Time.deltaTime;

            if (timer <= activeRadiusThreshold) {
                if (isRadiusActive==false) {
                    isRadiusActive = true;
                }
            }

            if (timer < 0.1f) {
                timer = 0;
                isTiming = false;
                Explode();
            }
        }
    }

    private void Explode() {
        Instantiate(explosionFX, transform.position, Quaternion.identity);
        Destroy(parent.gameObject);
    }

    private void OnTriggerStay(Collider other) {
        if (isRadiusActive == true) {

            BodyPart bodyPart = other.gameObject.GetComponent<BodyPart>();

            if (bodyPart != null) {
                IDamageable damageable = bodyPart.GetComponentInParent<IDamageable>();
                //Transform collisionTransformRoot = bodyPart.GetComponentInParent<Transform>().root;
                if (damageable != null) {
                    Vector3 contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                    damageable.TakeDamage(explosionDamage);
                }
            }
            Explode();
        }
    }
}
