using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PouchItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    private float value = 30f;

    private enum ItemState { Ready, Holstered }

    private ItemState _state = ItemState.Holstered;

    Player player;

    private Canvas canvas;

    public Sprite fillSprite; // The new sprite to assign to the Image component
    public Sprite hintSprite;

    public Image fillImage = null;

    public GameObject objectPrefab;

    [SerializeField] private Toggle toggle;

    // The duration threshold for a long press in seconds
    public float longPressDuration = 3f;

    public float longPressDurationDelay = 1f;

    // Boolean to indicate if a long press has been detected
    public bool isLongPressDetected = false;

    public bool isPressed = false;

    public Vector2 initialPosition;

    public Vector2 initialFillImagePosition;
    [SerializeField] private Vector2 scale;

    public GameObject image;

    private Coroutine longPressCoroutine;

    public bool canDraw = false;

    Vector2 initialTouchPosition;

    float eventSwipeThreshold = 160f;

    public PlayerAnimator playerAnimator;

    public void Start() {
        scale = fillImage.GetComponent<RectTransform>().localScale;
        player = GameObject.Find("Player").GetComponent<Player>();
        canvas = GetComponentInParent<Canvas>();
        toggle = GameObject.Find("Pouch Toggle").GetComponent<Toggle>();
        playerAnimator = GameObject.Find("Player Visual").GetComponent<PlayerAnimator>();
    }

    public void OnPointerDown(PointerEventData eventData) {
        // Start the long press detection coroutine
        initialTouchPosition = eventData.position;
        isPressed = true;
        initialPosition = image.transform.position;
        initialFillImagePosition = fillImage.transform.position;
        longPressCoroutine = StartCoroutine(LongPressDetectionDelayed(eventData));

        ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.pointerDownHandler);
    }

    public void OnPointerUp(PointerEventData eventData) {

        if (_state == ItemState.Ready) {
            float swipeDistance = eventData.position.y - initialTouchPosition.y;
            if (Mathf.Abs(swipeDistance) > eventSwipeThreshold) {
                UseItem();
                return;
            }
        }

        // Stop the long press detection coroutine
        if (longPressCoroutine != null) {
            StopCoroutine(longPressCoroutine);
            longPressCoroutine = null;
            //_state = ItemState.Holstered;
        }

        _state = ItemState.Holstered;

        //Reset image to its original position
        image.transform.position = initialPosition;

        // Reset the long press detection flag
        isLongPressDetected = false;
        isPressed = false;

        // Reset the fill amount of the Image component
        ResetFillAmount();

        ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.pointerUpHandler);

    }

    private void ResetFillAmount() {
        if (fillImage != null) {
            fillImage.fillAmount = 0f;
            fillImage.sprite = fillSprite;
            fillImage.transform.position = initialFillImagePosition;
            fillImage.GetComponent<RectTransform>().localScale = scale;
            fillImage.color = Color.white;
        }
    }

    private void UseItem() {
        // Refactor

        toggle.isOn = !toggle.isOn;

        Instantiate(objectPrefab, Vector3.zero, Quaternion.identity);

        playerAnimator.Trigger_UsingItem_AnimState();
        player.FillHealth(value);

        Destroy(this.gameObject);
    }

    private IEnumerator LongPressDetectionDelayed(PointerEventData eventData) {

        // Wait for 1 second before starting the long press detection
        yield return new WaitForSeconds(longPressDurationDelay);

        longPressCoroutine = StartCoroutine(LongPressDetection(eventData));
    }

    private IEnumerator LongPressDetection(PointerEventData eventData) {
        float timer = 0f;

        while (timer < longPressDuration) {

            timer += Time.deltaTime;

            // Update the fill amount of the Image component
            if (fillImage != null) {
                float fillAmount = timer / longPressDuration;
                fillImage.fillAmount = fillAmount;
            }

            //float swipeDistance = eventData.position.x - initialTouchPosition.x;
            //if (Mathf.Abs(swipeDistance) > eventSwipeThreshold) {

            //    ResetFillAmount();
            //    yield break;
            //}

            Vector2  swipeDistance = new Vector2(eventData.position.x, eventData.position.y) - initialTouchPosition;
            if (Mathf.Abs(swipeDistance.magnitude) > eventSwipeThreshold) {

                ResetFillAmount();
                yield break;
            }

            yield return null;
        }

        _state = ItemState.Ready;

        // Long press duration reached, set the flag to true
        isLongPressDetected = true;

        // Reset the image fill when completed
        //fillImage.fillAmount = 0;
        fillImage.sprite = hintSprite;
        fillImage.GetComponent<RectTransform>().localScale = fillImage.GetComponent<RectTransform>().localScale / 3f;
        fillImage.color = Color.red;

        //Slightly elevate the image
        Vector2 offset = new Vector2(0f, 50f);
        Vector2 currentPos = image.GetComponent<RectTransform>().transform.position;
        Vector2 currentPos2 = fillImage.GetComponent<RectTransform>().transform.position;
        image.transform.position = currentPos + offset;
        fillImage.transform.position = currentPos2 + offset;

    }

    // Method to check if a long press has been detected
    public bool IsLongPressDetected() {
        return isLongPressDetected;
    }

    ///Makes the image position to be draggable only on the Y-Axis from its anchor point///
    //public void OnDrag(PointerEventData eventData) {

    //    if (isLongPressDetected) {
    //        Vector2 position;
    //        RectTransformUtility.ScreenPointToLocalPointInRectangle(
    //            (RectTransform)canvas.transform,
    //            eventData.position,
    //            canvas.worldCamera,
    //            out position);

    //        Vector3 newPosition = canvas.transform.TransformPoint(position);
    //        newPosition.x = initialPosition.x; // Lock the X position

    //        if (newPosition.y >= initialPosition.y) {
    //            image.transform.position = newPosition;
    //        } 
    //    }
    //}
}
