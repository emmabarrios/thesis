using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Item : MonoBehaviour
{
    private enum State {Spawned, Attached }
    private State state;

    [SerializeField] private Vector3 offset;
    [SerializeField] private Transform attachPointA;
    [SerializeField] private Transform attachPointB;
    [SerializeField] private Animator animator;

    [Header("Destroy Timer Settings")]
    [SerializeField] private float timer;
    [SerializeField] private float destroyTime;
    [SerializeField] private float spawnTime;
    [SerializeField] private bool isTiming;
    private bool isAttached;

    // Start is called before the first frame update
    void Start()
    {
        state = State.Spawned;

        attachPointA = GameObject.Find("Player Attach Point").GetComponent<Transform>();

        attachPointB = GameObject.Find("Item Anchor").GetComponent<Transform>();

        animator = GetComponentInChildren<Animator>();

        timer = spawnTime;

        AttachToPoint(attachPointA);

        isAttached = false;
        isTiming = true;
    }

    public void AttachToPoint(Transform point) {

        this.transform.localPosition = Vector3.zero;

        this.transform.SetParent(point);

        this.transform.localPosition = Vector3.zero;

    }

    private void Update() {

        //if (isTiming && state == State.Spawned) {
        //    if (animator != null) {

        //        timer -= Time.deltaTime;
        //        if (timer < 0.1f) {
        //            timer = 0;
        //            Destroy(gameObject);
        //        }

        //        isAttached = true;
        //        AttachToPoint(attachPointB);
        //        this.transform.localPosition = transform.localPosition + offset;
        //        //transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        //        isTiming = true;

        //    }
        //}

        //if (isAttached == false) {
        //    if (animator != null) {
        //        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        //        if (stateInfo.IsName("Idle")) {
        //            AttachToPoint(attachPointB);
        //            isAttached = true;

        //            this.transform.localPosition = transform.localPosition + offset;
        //            isTiming = true;
        //        }
        //    }
        //}


        if (isTiming && isAttached == false) {
            timer -= Time.deltaTime;
            if (timer < 0.1f) {
                animator.SetTrigger("isAttached");
                AttachToPoint(attachPointB);
                //this.transform.localPosition = transform.localPosition + offset;
                this.transform.position = transform.position+ offset;
                timer = destroyTime;
                isAttached = true;
            }
        }

        if (isTiming && isAttached == true) {
            timer -= Time.deltaTime;
            if (timer < 0.1f) {
                timer = 0;
                Destroy(gameObject);
            }
        }
    }

    public void AttachToHand() {

        this.transform.localPosition = Vector3.zero;

        this.transform.SetParent(attachPointB);

        isTiming = true;

        this.transform.localPosition = Vector3.zero;

    }

}
