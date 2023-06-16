using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class DamageCollider : MonoBehaviour
{
    [SerializeField] private Collider damageCollider;
    [SerializeField] private float weaponDamage;
    [SerializeField] private Transform parentTransform;

    private void Awake() {
        damageCollider = GetComponent<Collider>();
        damageCollider.gameObject.SetActive(true);
        damageCollider.isTrigger = true;
        damageCollider.enabled = false;
    }

    private void Start() {
        parentTransform = GetComponent<Transform>().root;

        if (parentTransform.CompareTag("Player")) {
            int _layer = LayerMask.NameToLayer("Player");
            this.gameObject.layer = _layer;
        }
    }

    public void EnableDamageCollider() {
        damageCollider.enabled = true;
    }

    public void DisableDamageCollider() {
        damageCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider collision) {
        //Character character = collision.GetComponent<Character>();
        BodyPart bodyPart = collision.GetComponent<BodyPart>();

        if (bodyPart!=null) {
            IDamageable damageable = bodyPart.GetComponentInParent<IDamageable>();
            Transform collisionTransformRoot = bodyPart.GetComponentInParent<Transform>().root;
            if (damageable != null) {
                if (!collision.CompareTag(parentTransform.tag)) {

                    // If the attack was deflected by a parry
                    //if (collisionTransformRoot.GetComponent<Character>().IsParryPerformed) {
                    //    parentTransform.GetComponent<Animator>().SetTrigger("staggered");
                    //    DisableDamageCollider();
                    //    return;
                    //}

                    damageable.TakeDamage(weaponDamage);
                    DisableDamageCollider();
                }
            }
        }
    }
}
