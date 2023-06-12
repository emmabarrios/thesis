using System;
using UnityEngine;
using UnityEngine.UIElements;

public class Weapon : MonoBehaviour
{
    public float damage;
    public float damageFactor;

    public float hitWindow;

    public GameObject sparkEffect;


    //private void OnTriggerEnter(Collider other) {
    //    if (other.CompareTag("hitContactArea")) {
    //        GameObject firework = Instantiate(sparkEffect, other.transform.position, Quaternion.identity);
    //        sparkEffect.GetComponentInChildren<ParticleSystem>().Play();
    //        foreach (ContactPoint contact in collision.contacts)
    //    {
    //        // Instantiate the particle effect at the contact point position
    //        Instantiate(particleEffect, contact.point, Quaternion.identity);
    //    }
    //    }
    //}

    //private void OnCollisionEnter(Collision collision) {
    //    if (collision.gameObject.CompareTag("hitContactArea")) {
    //        foreach (ContactPoint contact in collision.contacts) {
    //            // Instantiate the particle effect at the contact point position
    //            Instantiate(sparkEffect, contact.point, Quaternion.identity);
    //            sparkEffect.GetComponentInChildren<ParticleSystem>().Play();

    //        }
    //    }
    //}

}
