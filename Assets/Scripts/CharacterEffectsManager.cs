using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEffectsManager : MonoBehaviour
{
    [Header("Damage FX")]
    public GameObject bloodSplatterFX;

    public void PlayDamageEffect(Vector3 damageEffectLocation) {
        GameObject blood = Instantiate(bloodSplatterFX, damageEffectLocation, Quaternion.identity);
    }
}
