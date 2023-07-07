using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PouchItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    [Header("Holstered Item")]
    //public GameObject itemPrefab;
    //public GameObject objectPrefab;


    [Header("New implementation")]
    public Item itemSO;
    public Sprite item_holder_graphic;
    public Image fillImage = null;

    private float value = 30f;

    private enum ItemState { Ready, Holstered }

    private ItemState _state = ItemState.Holstered;

    Player player;

    private Canvas canvas;

    public Sprite fillSprite; // The new sprite to assign to the Image component
    public Color fillSpriteColor;
    public Sprite hintSprite;
    public Color hintSpriteColor;

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

    public GameObject item_image_gameObject;
    public GameObject item_holder_gameObject;
    public Sprite imageSprite;

    private Coroutine longPressCoroutine;

    public bool canDraw = false;

    Vector2 initialTouchPosition;

    float eventSwipeThreshold = 160f;

    public PlayerAnimator playerAnimator;

    public void Start() {
        //fillImage.color = fillSpriteColor;

        scale = fillImage.GetComponent<RectTransform>().localScale;
        player = GameObject.Find("Player").GetComponent<Player>();
        canvas = GetComponentInParent<Canvas>();
        toggle = GameObject.Find("Pouch Toggle").GetComponent<Toggle>();
        playerAnimator = GameObject.Find("Player Visual").GetComponent<PlayerAnimator>();
    }

    public void OnPointerDown(PointerEventData eventData) {
        // Start the long press detection coroutine
        fillImage.color = fillSpriteColor;

        initialTouchPosition = eventData.position;
        isPressed = true;
        initialPosition = item_image_gameObject.transform.position;
        initialFillImagePosition = fillImage.transform.position;
        longPressCoroutine = StartCoroutine(LongPressDetectionDelayed(eventData));

        ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.pointerDownHandler);
    }

    public void OnPointerUp(PointerEventData eventData) {

        if (_state == ItemState.Ready) {
            float swipeDistance = eventData.position.y - initialTouchPosition.y;
            if (Mathf.Abs(swipeDistance) > eventSwipeThreshold) {
                DrawItem();
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
        item_image_gameObject.transform.position = initialPosition;

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
            //fillImage.color = Color.white;
            //fillImage.color = fillSpriteColor;
        }
    }

    private void DrawItem() {

        // Toggle quick item button
        //toggle.isOn = !toggle.isOn;

        GameObject _item = Instantiate(itemSO._usablePrefab, Vector3.zero, Quaternion.identity) as GameObject;
        IUsable usable = _item.GetComponent<IUsable>();
        usable.Use();

        // Destroy pouch item
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
        //fillImage.color = Color.red;
        fillImage.color = hintSpriteColor;

        //Slightly elevate the image
        Vector2 offset = new Vector2(0f, 50f);
        Vector2 currentPos = item_image_gameObject.GetComponent<RectTransform>().transform.position;
        Vector2 currentPos2 = fillImage.GetComponent<RectTransform>().transform.position;
        item_image_gameObject.transform.position = currentPos + offset;
        fillImage.transform.position = currentPos2 + offset;

    }

    // Method to check if a long press has been detected
    public bool IsLongPressDetected() {
        return isLongPressDetected;
    }

    public void InitializeQuickItemGraphics() {
        item_image_gameObject.GetComponent<Image>().sprite = itemSO._sprite;
        item_holder_gameObject.GetComponent<Image>().sprite = itemSO._sprite_pouch;

    }
}
