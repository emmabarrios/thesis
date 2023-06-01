using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private Transform attachPointA;
    [SerializeField] private Transform attachPointB;
    [SerializeField] private Animator animator;
    //private Vector3 initialScale;


    // Start is called before the first frame update
    void Start()
    {
        attachPointA = GameObject.Find("Player Attach Point").GetComponent<Transform>();

        attachPointB = GameObject.Find("Attach Point R").GetComponent<Transform>();

        animator = GetComponentInChildren<Animator>();

        //initialScale = transform.localScale;

        AttachToPoint(attachPointA);
    }

    public void AttachToPoint(Transform point) {

        this.transform.localPosition = Vector3.zero;

        this.transform.SetParent(point);

        this.transform.localPosition = Vector3.zero;

        //Vector3 rootScale = GameObject.Find("Player Visual").GetComponent<Transform>().localScale;

        //transform.localScale = new Vector3(
        //    initialScale.x / rootScale.x * 10,
        //    initialScale.y / rootScale.y * 10,
        //    initialScale.z / rootScale.z * 10
        //);

        //Debug.Log(rootScale);
    }

    private void Update() {
        if (animator != null) {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            //if (stateInfo.IsName("Idle") && stateInfo.normalizedTime >= 0.5f) {
            //    AttachToPoint(attachPointB);
            //} 
            if (stateInfo.IsName("Idle")) {
                //transform.position = attachPointB.position;
                AttachToPoint(attachPointB);
            }
        }
    }
}
