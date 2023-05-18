using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemPanel : MonoBehaviour
{
    [SerializeField] private Toggle toggle = null;
    [SerializeField] private float interpolationTime = 5f;
    [SerializeField] private float lenght = 100f;
    private Vector2 fromPosition; // The original position of the UI element
    private Vector2 targetPosition;

    private RectTransform rectTransform;

    private Coroutine moveCoroutine; // Reference to the active coroutine

    public bool isMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        fromPosition = rectTransform.position;

        toggle.onValueChanged.AddListener(delegate {
            ToggleValueChanged();
        });

        //pointAposition = pointA.anchoredPosition;
        //pointBposition = pointB.anchoredPosition;
    }

    // Update is called once per frame
    void Update() {

        //if (isMoving && toggle.isOn) {
        //    MoveTo(pointBposition);
        //} else if (isMoving && !toggle.isOn) {
        //    MoveTo(pointAposition);
        //}
    }

    //private IEnumerator MoveTo(Vector2 targetPosition) {

    //    float step = moveSpeed * Time.deltaTime;

    //    Vector2 currentPosition = this.GetComponent<RectTransform>().anchoredPosition;

    //    // Move the UI component towards the target position using Lerp
    //    this.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(currentPosition, targetPosition, step);



    //    // Check if the UI component has reached the target position
    //    if (Vector2.Distance(currentPosition, targetPosition) < 0.01f) {
    //        isMoving = false;
    //    }

    //}

    void ToggleValueChanged() {

        toggle.interactable = false;

        if (!isMoving) {

            isMoving = true;

            if (moveCoroutine != null) {
                StopCoroutine(moveCoroutine);
            }

            if (toggle.isOn) {
                
                moveCoroutine = StartCoroutine(MoveToTarget(lenght, interpolationTime));
            } else {

                moveCoroutine = StartCoroutine(MoveToTarget(-lenght, interpolationTime));
            }
        }
    }


    // Code usefull to move the arrow above the items
    //private IEnumerator MoveToTarget(float lenght, float interpolationTime) {

    //    float t = 0f;

    //    targetPosition = fromPosition + new Vector2(0f, lenght);

    //    while (Vector2.Distance(fromPosition, targetPosition) > 0.01f && t < 1f) {

    //        t += Time.deltaTime / interpolationTime; // Increase the interpolation parameter over time

    //        this.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(fromPosition, targetPosition, t); // Interpolate the position
    //        yield return null;
    //    }

    //    isMoving = false;
    //    fromPosition = targetPosition;

    //}

    private IEnumerator MoveToTarget(float lenght, float interpolationTime) {

        targetPosition = fromPosition + new Vector2(0f, lenght);
        Vector2 velocity = Vector2.zero;

        float t = 0;

        while (Vector2.Distance(transform.position, targetPosition) > 20f) {
            t = interpolationTime * Time.deltaTime;

            rectTransform.position = Vector2.SmoothDamp(rectTransform.position, targetPosition, ref velocity, t);

            yield return null;
        }

        rectTransform.position = targetPosition;

        fromPosition = targetPosition;
        toggle.interactable = true;
        isMoving = false;
    }
}
