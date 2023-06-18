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
    [SerializeField] private float enterTime;

    private void Start() {
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
        yield return new WaitForSeconds(enterTime);
        AttachToPoint(attachPointB);
        this.transform.localPosition = positionOffset;
        this.transform.localRotation = rotationOffset;
    }

}
