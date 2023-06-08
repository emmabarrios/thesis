using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitArea : MonoBehaviour
{
    [SerializeField] private CapsuleCollider collider;

    private bool isTiming = false;

    [SerializeField] private Transform parentTarget;

    [SerializeField] private float areaSpawnTime = 0f;

    [SerializeField] private float damage;


    void Start()
    {
        collider = GetComponent<CapsuleCollider>();
        parentTarget = GetComponent<Transform>().root;

    }

    void Update()
    {
        if (isTiming) {
            areaSpawnTime -= Time.deltaTime;
            if (areaSpawnTime < 0.1f) {
                collider.enabled = false;
                isTiming = false;
            }
        }
    }

    public void ActivateHitArea(float damage, float window) {
        this.damage = damage;
        areaSpawnTime = window;
        isTiming = true;
        collider.enabled = true;
    }

    private void OnTriggerEnter(Collider other) {

        if (!other.gameObject.CompareTag(parentTarget.tag)) {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null) {
                GetComponent<Collider>().enabled = false;
                damageable.TakeDamage(this.damage);
            }
        }
    }

}
