using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Bomb : Projectile
{
    [Header("Parent")]
    [SerializeField] Transform parent;

    [Header("Explosion Settings")]
    [SerializeField] private float explosionDamage;
    [SerializeField] private GameObject explosionFX;
    [SerializeField] private List<Collider> inRangeColliders = new List<Collider>();

    [Header("Timer Settings")]
    [Range(0f, 10f)] public float timer;
    private bool isTiming;


    private void Start() {
        isTiming = true;
        parent = this.transform.parent;
    }

    private void Update() {

        // Bomb timer
        if (isTiming) {
            timer -= Time.deltaTime;
            if (timer < 0.1f) {
                timer = 0;
                isTiming = false;
                Explode();
            }
        }
    }

    private void Explode() {
        DealDamageOnRadius();
        Instantiate(explosionFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        BodyPart bodyPart = other.gameObject.GetComponent<BodyPart>();
        if (bodyPart!=null) {
            if(!inRangeColliders.Contains(other)) {
                inRangeColliders.Add(other);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (inRangeColliders.Contains(other)) {
            inRangeColliders.Remove(other);
        }
    }

    private void DealDamageOnRadius() {
        foreach (Collider collider in inRangeColliders) {
            BodyPart bodyPart = collider.gameObject.GetComponent<BodyPart>();

            if (bodyPart != null) {

                Debug.Log(collider.name);
                IDamageable damageable = bodyPart.GetComponentInParent<IDamageable>();

                if (damageable != null) {
                    damageable.TakeDamage(explosionDamage);
                }
            }
        }
    }
   
}
