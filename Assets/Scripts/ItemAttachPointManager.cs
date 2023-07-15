using System.Collections;
using UnityEngine;

public class ItemAttachPointManager : MonoBehaviour {

    [Header("Attach Points")]
    [SerializeField] private Vector3 positionOffset;
    [SerializeField] private Vector3 AOffset;
    [SerializeField] private Quaternion rotationOffset;
    [SerializeField] private Transform attachPointA;
    [SerializeField] private Transform attachPointB;

    [Header("Timer Settings")]
    [SerializeField] private float attackToHandTimer;
    [SerializeField] private float lifeTime;
    private bool isTiming;

    private void Update() {
        if (isTiming) {
            lifeTime -= Time.deltaTime;
            if (lifeTime < 0.1f) {
                lifeTime = 0;
                isTiming = false;
                Destroy(gameObject);
            }
        }
    }
    private void Start() {
        isTiming = true;
        attachPointA = GameObject.Find("Player Attach Point").GetComponent<Transform>();
        attachPointB = GameObject.Find("Item Anchor").GetComponent<Transform>();
        AttachToPoint(attachPointA);
        StartCoroutine(AttachToHand());
    }

    public void AttachToPoint(Transform point) {
        this.transform.SetParent(point);
        this.transform.localPosition = Vector3.zero + AOffset;
        this.transform.localRotation = Quaternion.identity;
    }

    private IEnumerator AttachToHand() {
        yield return new WaitForSeconds(attackToHandTimer);
        AttachToPoint(attachPointB);
        this.transform.localPosition = positionOffset;
        this.transform.localRotation = rotationOffset;
    }

}
