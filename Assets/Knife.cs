using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : Throwable
{
    private void OnTriggerEnter(Collider other) {
        DealDamage(other);
    }
}
