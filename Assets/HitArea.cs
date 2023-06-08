using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class HitArea : MonoBehaviour
{
    public enum AreaType {None, Attack, Defense, Parry }

    public AreaType areaType;

    [SerializeField] private Collider collider;

    private bool isTiming = false;

    [SerializeField] private Transform parentTarget;

    [SerializeField] private float areaSpawnTime = 0f;

    [SerializeField] private float damage;


    void Start()
    {
        collider = GetComponent<Collider>();
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
        areaType = AreaType.Attack;
        this.damage = damage;
        areaSpawnTime = window;
        isTiming = true;
        collider.enabled = true;
    }

    public void ActivateBlockArea() {
        areaType = AreaType.Defense;
        collider.enabled = true;
        isTiming = false;
    }
    
    public void DeactivateBlockArea() {
        areaType = AreaType.None;
        collider.enabled = false;
    }
    
    public void ActivateParryArea(float window) {
        areaType = AreaType.Parry;
        areaSpawnTime = window;
        isTiming = true;
        collider.enabled = true;
    }

    //private void OnTriggerEnter(Collider other) {

    //    if (!other.gameObject.CompareTag(parentTarget.tag)) {
    //        IDamageable damageable = other.GetComponent<IDamageable>();
    //        if (damageable != null) {
    //            GetComponent<Collider>().enabled = false;
    //            damageable.TakeDamage(this.damage);
    //        }
    //    }
    //}

    private void OnTriggerStay(Collider other) {

        // If a shield is blocking my attack

        HitArea opositeArea = other.GetComponent<HitArea>();
        IDamageable damageable = other.GetComponent<IDamageable>();


        if (opositeArea != null ) {

            // Attackers HitBox is disable agaisnt Defense Hitbox
            if (this.areaType == AreaType.Attack && opositeArea.areaType == AreaType.Defense) {
                collider.enabled = false;
                return;
            }
        }

        // If reached here, no Defense Hitbox was detected and will deal damage to the other collider
        if (areaType == AreaType.Attack && damageable != null && !other.gameObject.CompareTag(parentTarget.tag)) {
            collider.enabled = false;
            damageable.TakeDamage(this.damage);
        }

    }
}
