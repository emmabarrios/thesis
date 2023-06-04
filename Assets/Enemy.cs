using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Animator animator;

    [SerializeField] private float timer;
    [SerializeField] private float limitedTime = 5f;
    [SerializeField] private bool isTiming;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        isTiming = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Timer 
        if (isTiming) {
            timer -= Time.deltaTime;
            if (timer < 0.1f) {
                timer = limitedTime;
                animator.SetTrigger("attack");
            }
        }
    }



}
