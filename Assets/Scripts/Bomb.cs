using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bomb : Throwable
{
    public float time;
    public GameObject explosionRadius;
    public bool isTimed = false;

    private void Start() {
        if (isTimed) {
            StartCoroutine(CountDown());
        }
    }

    private IEnumerator CountDown() {
        yield return new WaitForSeconds(time);
        explosionRadius.GetComponent<SphereCollider>().enabled = true;
        yield return new WaitForSeconds(0.5f);
        Explode();
    }

    private void Explode() {
        Instantiate(explosionFX, transform.localPosition, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        DealDamage(other);
    }
}
