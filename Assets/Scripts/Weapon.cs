using System.Collections.Generic;
using UnityEngine;
using static BodyPart;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Collider damageCollider;
    [SerializeField] private float weaponDamage;

    [SerializeField] private Transform parentTransform;

    [Header("Localized Damage bonus")]
    [SerializeField] private float Head;
    [SerializeField] private float Arm;
    [SerializeField] private float Leg;
    [SerializeField] private float Chest;
    [SerializeField] private float Hip;

    public Dictionary<Part, float> partDictionary;

    private void Awake() {
        damageCollider = GetComponent<Collider>();
        damageCollider.gameObject.SetActive(true);
        damageCollider.isTrigger = true;
        damageCollider.enabled = false;
    }

    private void Start() {
        parentTransform = GetComponent<Transform>().root;

        partDictionary = new Dictionary<Part, float>();
        SetPartValue(Part.Head, Head);
        SetPartValue(Part.Arm, Arm);
        SetPartValue(Part.Leg, Leg);
        SetPartValue(Part.Chest, Chest);
        SetPartValue(Part.Hip, Hip);

    }

    public void EnableDamageCollider() {
        damageCollider.enabled = true;
    }

    public void DisableDamageCollider() {
        damageCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider collision) {
        BodyPart detectedPart = collision.GetComponent<BodyPart>();

        if (detectedPart != null) {
            Part part = detectedPart.bodyPart;
            IDamageable damageable = detectedPart.GetComponentInParent<IDamageable>();
            Transform collisionTransformRoot = detectedPart.GetComponentInParent<Transform>().root;
            if (damageable != null) {
                if (!collision.CompareTag(parentTransform.tag)) {

                    CharacterEffectsManager fx = collisionTransformRoot.GetComponent<CharacterEffectsManager>();

                    Vector3 contactPoint = collision.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                    fx.PlayDamageEffect(contactPoint);

                    float bonusPart = partDictionary[part];
                    float totalDamage = weaponDamage + bonusPart;
                    damageable.TakeDamage(totalDamage);
                    DisableDamageCollider();
                }
            }
        }
    }

    private void SetPartValue(Part part, float value) {
        if (partDictionary.ContainsKey(part)) {
            partDictionary[part] = value;
        } else {
            partDictionary.Add(part, value);
        }
    }
}
