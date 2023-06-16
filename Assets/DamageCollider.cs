using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    [SerializeField] private Collider damageCollider;
    [SerializeField] private float weaponDamage;
    [SerializeField] private Transform parentTransform;
    [SerializeField] private Character characterUser;

    private void Awake() {
        damageCollider = GetComponent<Collider>();
        damageCollider.gameObject.SetActive(true);
        damageCollider.isTrigger = true;
        damageCollider.enabled = false;
    }

    private void Start() {
        parentTransform = GetComponent<Transform>().root;
        characterUser = parentTransform.GetComponent<Character>();
        if (characterUser.CompareTag("Player")) {
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
            Debug.Log(bodyPart.bodyPart);
            if (damageable != null) {
                if (!collision.CompareTag(parentTransform.tag)) {
                    damageable.TakeDamage(weaponDamage);
                    DisableDamageCollider();
                }
            }
        }
    }
}
