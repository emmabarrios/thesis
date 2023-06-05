using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private Transform attachPointA;
    [SerializeField] private Transform attachPointB;
    [SerializeField] private Animator animator;

    [Header("Destroy Timer Settings")]
    [SerializeField] private float timer;
    [SerializeField] private float destroyTime;
    [SerializeField] private bool isTiming;
    private bool isAttached = false;

    // Start is called before the first frame update
    void Start()
    {
        attachPointA = GameObject.Find("Player Attach Point").GetComponent<Transform>();

        attachPointB = GameObject.Find("Anchor Point R").GetComponent<Transform>();

        animator = GetComponentInChildren<Animator>();

        timer = destroyTime;

        AttachToPoint(attachPointA);
    }

    public void AttachToPoint(Transform point) {

        this.transform.localPosition = Vector3.zero;

        this.transform.SetParent(point);

        this.transform.localPosition = Vector3.zero;

    }

    private void Update() {
        if (isAttached == false) {
            if (animator != null) {
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

                if (stateInfo.IsName("Idle")) {
                    isAttached = true;
                    AttachToPoint(attachPointB);
                    this.transform.localPosition = transform.localPosition + offset;
                    isTiming = true;
                }
            }
        }
       

        if (isTiming) {
            timer -= Time.deltaTime;
            if (timer < 0.1f) {
                timer = 0;
                Destroy(gameObject);
            }
        }
    }

}
