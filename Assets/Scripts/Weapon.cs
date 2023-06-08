using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float damage;
    public float damageFactor;

    private bool isTiming = false;
    [SerializeField] private float windowTimer;
    [SerializeField] private float time;
    [SerializeField] private float delayWindowTimer;
    //[SerializeField] float windowFactor = 1;

    private Transform root;
    private string rootTag;

    private CapsuleCollider collider;

    void Start()
    {
        root = this.gameObject.transform.root;
        rootTag = root.tag;

        collider = this.GetComponent<CapsuleCollider>();
        collider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Damage Window Timer 
        if (isTiming) {
            //if (collider.enabled == false) {
            //    collider.enabled = true;
            //}
            time -= Time.deltaTime;
            if (time < 0.1f) {
                time = windowTimer;
                collider.enabled = false;
                isTiming = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.gameObject.CompareTag(rootTag)) {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null) {
                collider.enabled = false;
                time = 0;
                damageable.TakeDamage(damage);
            }
        }
    }

    public void OpenWeaponDamageWindow(float time) {
        StartCoroutine(WeaponWindowDelay(delayWindowTimer, time));
       
    }

    private IEnumerator WeaponWindowDelay(float delay, float time) {
        yield return new WaitForSeconds(delay);
        collider.enabled = true;
        windowTimer = time - delay;
        this.time = windowTimer;
        isTiming = true;
    }
}
