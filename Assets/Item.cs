using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _Item : MonoBehaviour
{
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

        // Attach to hand timer, when time is out, attach to hand.
        if (isTiming && isAttached == false) {
            timer -= Time.deltaTime;
            if (timer < 0.1f) {
                animator.SetTrigger("isAttached");
                AttachToPoint(attachPointB);
                transform.position = transform.position+ offset;
                timer = destroyTime;
                isAttached = true;
            }
        }

        // Destroy Timer, when time is out, destroy Usable.
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
