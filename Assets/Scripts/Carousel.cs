using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Carousel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    [SerializeField] int pouchCount;
    [SerializeField] int carouselPosition = 0;
    [SerializeField] float pouchSize = 755f;
    private RectTransform rectTransform;

    public bool isMoving = false;
    public bool isSwipeInProgress = false;

    [SerializeField] private float swipeThreshold = 50f;
    [SerializeField] private Vector2 initialTouchPosition;

    [SerializeField] private float interpolationTime;

    public GameObject blocker = null;

    private Coroutine slideCoroutine;

    //[SerializeField] private float interpolationTime = 5f;
    //[SerializeField] private float lenght = 100f;
    //private Vector2 fromPosition; 
    //private Vector2 targetPosition;

    //private Coroutine moveCoroutine;

    private void Start() {
        rectTransform = this.GetComponent<RectTransform>();
        pouchCount = this.transform.childCount - 1;
        //ModifyImageAlpha(leftHint.GetComponent<Image>(),0f);
    }

    private void ModifyImageAlpha(Image image, float alpha) {
        Color newColor = image.color;
        newColor.a = alpha;
        image.color = newColor;
    }

    public void OnPointerDown(PointerEventData eventData) {
            isSwipeInProgress = true;
            initialTouchPosition = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData) {

        if (isSwipeInProgress) {

            if (!isMoving) {
                float swipeDistance = eventData.position.x - initialTouchPosition.x;

                if (Mathf.Abs(swipeDistance) > swipeThreshold) {

                    Vector2 normalDirection = new Vector2(swipeDistance, 0).normalized;
                    float swipeDirection = normalDirection.x;

                    if ((carouselPosition == 0 && swipeDirection > 0)) {
                        isSwipeInProgress = false;
                        isMoving = false;
                        return;
                    }

                    if ((carouselPosition == pouchCount && swipeDirection < 0)) {
                        isSwipeInProgress = false;
                        isMoving = false;
                        return;
                    }

                    isMoving = true;

                    blocker.GetComponent<Image>().raycastTarget = true;

                    slideCoroutine = StartCoroutine(Slide(interpolationTime, swipeDirection));
                    OnSwipe?.Invoke(this, new OnSwipeEventArgs { index = carouselPosition });
                }
            }
        }
        isSwipeInProgress = false;
    }

    //private void Slide(float swipeDirection) {

    //    if ((carouselPosition == 0 && swipeDirection > 0)) {
    //        return;
    //    }

    //    if ((carouselPosition == pouchCount && swipeDirection < 0)) {
    //        return;
    //    }

    //    Vector2 offset = new Vector2(Mathf.Floor(pouchSize * swipeDirection), 0f);
    //    Vector2 currentPos = rectTransform.position;

    //    rectTransform.position = currentPos + offset;
    //    carouselPosition -= (int)swipeDirection;

    //    if (carouselPosition == 0 && leftHint.GetComponent<Image>().color.a != 0) {
    //        ModifyImageAlpha(leftHint.GetComponent<Image>(), 0f);

    //    } else if (leftHint.GetComponent<Image>().color.a == 0 && carouselPosition != 0) {
    //        ModifyImageAlpha(leftHint.GetComponent<Image>(), 1f);
    //    }

    //    if (carouselPosition == pouchCount && rightHint.GetComponent<Image>().color.a != 0) {
    //        ModifyImageAlpha(rightHint.GetComponent<Image>(), 0f);

    //    } else if (rightHint.GetComponent<Image>().color.a == 0 && carouselPosition != pouchCount) {
    //        ModifyImageAlpha(rightHint.GetComponent<Image>(), 1f);
    //    }
    //}

    //private IEnumerator Slide(float lenght, float interpolationTime) {

    //    targetPosition = fromPosition + new Vector2(0f, lenght);
    //    Vector2 velocity = Vector2.zero;

    //    float t = 0;

    //    while (Vector2.Distance(transform.position, targetPosition) > 0.01f) {
    //        t = interpolationTime * Time.deltaTime;

    //        rectTransform.position = Vector2.SmoothDamp(rectTransform.position, targetPosition, ref velocity, t);
    //        yield return null;
    //    }

    //    fromPosition = targetPosition;
    //    isMoving = false;
    //}

    private IEnumerator Slide(float interpolationTime, float swipeDirection) {

        Vector2 offset = new Vector2(Mathf.Floor(pouchSize * swipeDirection), 0f);
        Vector2 currentPos = rectTransform.position;

        Vector2 targetPosition = currentPos + offset;
        carouselPosition -= (int)swipeDirection;

        Vector2 velocity = Vector2.zero;

        float t = 0;

        while (Vector2.Distance(rectTransform.position, targetPosition) >20f) {
            t = interpolationTime * Time.deltaTime;

            rectTransform.position = Vector2.SmoothDamp(rectTransform.position, targetPosition, ref velocity, t);
            yield return null;
        }


        rectTransform.position = targetPosition;

        isMoving = false;
        blocker.GetComponent<Image>().raycastTarget = false;
        
    }

    public event EventHandler<OnSwipeEventArgs> OnSwipe;
    public class OnSwipeEventArgs {
        public int index;
    }


}
