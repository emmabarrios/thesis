using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockArea : MonoBehaviour
{
    [SerializeField] private Collider collider;

    private bool isTiming = false;

    [SerializeField] private Transform parentTarget;

    [SerializeField] private float areaSpawnTime = 0f;

    [SerializeField] private float damage;

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<Collider>();
        parentTarget = GetComponent<Transform>().root;
    }

    // Update is called once per frame
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

    public void ActivateBlockArea(float damage, float window) {
        this.damage = damage;
        areaSpawnTime = window;
        isTiming = true;
        collider.enabled = true;
    }


}
