using System;
using UnityEngine;
using UnityEngine.Analytics;

public class HitArea : MonoBehaviour
{
    [SerializeField] private Collider collider;

    private bool isTiming = false;

    [SerializeField] private Transform parentTarget;

    [SerializeField] private float areaSpawnTime = 0f;

    [SerializeField] private float damage;

    public event EventHandler OnHitDeflected;


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
                areaSpawnTime = 0;
                //areaType = AreaType.None;
            }
        }
    }

    public void ActivateHitArea(float damage, float window) {
        //areaType = AreaType.Attack;
        this.damage = damage;
        areaSpawnTime = window;
        isTiming = true;
        collider.enabled = true;
    }

    public void DeactivateHitArea() {
        isTiming = false;
        collider.enabled = false;
        areaSpawnTime = 0f;
    }

    private void OnTriggerEnter(Collider other) {

        if (!other.gameObject.CompareTag(parentTarget.tag)) {

            PlayerController pc = other.gameObject.GetComponent<PlayerController>();

            if (pc != null && pc.ParryExecuted) {
                parentTarget.GetComponent<Animator>().SetTrigger("staggered");
            }

            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null) {
                DeactivateHitArea();
                damageable.TakeDamage(this.damage);
            }
        }
    }

}
