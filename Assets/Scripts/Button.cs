using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {

    public enum AxisOptions { Both, Horizontal, Vertical, None, Shield }
    public float Horizontal { get { return (snapX) ? SnapFloat(input.x, AxisOptions.Horizontal) : input.x; } }
    public float Vertical { get { return (snapY) ? SnapFloat(input.y, AxisOptions.Vertical) : input.y; } }
    
    [SerializeField] float lastTimeTap;
    [SerializeField] float tapThreshold = 0.75f;
    public float storedMagnitude = 0f;

    [SerializeField] private AxisOptions axisOptions = AxisOptions.Both;
    [SerializeField] private bool snapX = false;
    [SerializeField] private bool snapY = false;
    [SerializeField] protected RectTransform background = null;
    [SerializeField] private RectTransform handle = null;
    [SerializeField] private bool isActive = false;
    [SerializeField] private GameObject deactiveButton = null;

    public RectTransform activeAnchor = null;
    public RectTransform inactiveAnchor = null;

    private Vector2 activeAnchorPosition;
    private Vector2 inactiveAnchorPosition;

    private Canvas canvas;

    private Vector2 input = Vector2.zero;
    private Camera cam;
    
    public event EventHandler<OnHandleDragedEventArgs> OnHandleDraged;
    public event EventHandler<OnBlockingEventArgs> OnBlocking;
    //public event EventHandler<OnParryEventArgs> OnParry;
    public event EventHandler OnParry;
    public event EventHandler<OnTensionReleasedEventArgs> OnTensionReleased;
    public event EventHandler OnHandleDroped;

    Vector2 initialTouchPosition;
    float eventSwipeThreshold = 160f;


    [SerializeField] private Vector2 scale;

    #region Shield
    private bool isToggle;
    public bool toggleValue = false;
    public Action<bool> OnToggleValueChanged;

    private Controller playerController;
    #endregion

    public Vector2 Scale {
        get { return scale; }
        set { scale = value; }
    }

    public bool IsActive {
        get { return isActive; }
        set { isActive = value; }
    }

    public class OnTensionReleasedEventArgs : EventArgs {
        public float magnitude;
    }
    public class OnHandleDragedEventArgs : EventArgs {
        public float magnitude;
        public float position;
    }
    public class OnBlockingEventArgs: EventArgs {
        public string a;
    }
    public class OnParryEventArgs: EventArgs {
        public string a;
    }



    private void Start() {
        playerController = GameObject.Find("Player").GetComponent<Controller>();
        playerController.OnParry += ChangeToggleValue;

        Scale = this.GetComponent<RectTransform>().localScale;
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null) {
            Debug.LogError("The Joystick is not placed inside a canvas");
        }
        Vector2 center = new Vector2(0.5f, 0.5f);
        background.pivot = center;
        handle.anchorMin = center;
        handle.anchorMax = center;
        handle.pivot = center;
        handle.anchoredPosition = Vector2.zero;

        activeAnchorPosition = activeAnchor.anchoredPosition;
        inactiveAnchorPosition = inactiveAnchor.anchoredPosition;
    }

    public void OnPointerDown(PointerEventData eventData) {

        initialTouchPosition = eventData.position;

        // Button swapping 
        if (!IsActive) {
            IsActive = true;
            this.GetComponent<RectTransform>().localScale = deactiveButton.GetComponent<Button>().Scale;
            deactiveButton.GetComponent<RectTransform>().localScale = Scale;

            Scale = this.GetComponent<RectTransform>().localScale;
            deactiveButton.GetComponent<Button>().Scale = deactiveButton.GetComponent<RectTransform>().localScale;

            SetAnchorPosition(activeAnchorPosition);
            deactiveButton.GetComponent<Button>().IsActive = false;
            deactiveButton.GetComponent<Button>().SetAnchorPosition(inactiveAnchorPosition);
        }

        // Axis Option Both state
        if (axisOptions == AxisOptions.Both) {
            OnDrag(eventData);
        }

        // Axis Option None state
        if (axisOptions == AxisOptions.Shield) {
            OnBlocking?.Invoke(this, new OnBlockingEventArgs { a = "Blocking" });
        }

        
    }

    public void OnPointerUp(PointerEventData eventData) {
        input = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;

        if (axisOptions == AxisOptions.None) {
            CheckForDoubleTap(eventData);
        }

        if (axisOptions == AxisOptions.Vertical) {
            OnTensionReleased?.Invoke(this, new OnTensionReleasedEventArgs { magnitude = storedMagnitude });
        }

        if (axisOptions == AxisOptions.Shield) {

            // This is for the parry
            //float swipeDistance = eventData.position.x - initialTouchPosition.x;
            //if (Mathf.Abs(swipeDistance) > eventSwipeThreshold) {
            //    OnParry?.Invoke(this, new OnParryEventArgs { a = "Parry!" });
            //}
            toggleValue = !toggleValue;
            OnToggleValueChanged?.Invoke(toggleValue);

        }

        OnHandleDroped?.Invoke(this, EventArgs.Empty);
    }

    private void CheckForDoubleTap(PointerEventData eventData) {
        float currentTimeClick = eventData.clickTime;

        if (Mathf.Abs(currentTimeClick - lastTimeTap) < tapThreshold) {
            //OnParry?.Invoke(this, new OnParryEventArgs { a = "Parry!"});
        }
        lastTimeTap = currentTimeClick;
    }

    public void OnDrag(PointerEventData eventData) {

        cam = null;
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            cam = canvas.worldCamera;

        Vector2 position = RectTransformUtility.WorldToScreenPoint(cam, background.position);
        Vector2 radius = background.sizeDelta;
        input = (eventData.position - position) / (radius * canvas.scaleFactor);
        FormatInput();
        handle.anchoredPosition = input * radius * 1;

        // Bow tension control
        if (axisOptions == AxisOptions.Vertical) {

            float mag = (handle.anchoredPosition - background.rect.center).magnitude;
            storedMagnitude = mag;

            OnHandleDraged?.Invoke(this, new OnHandleDragedEventArgs {
                magnitude = mag,
                position = (handle.anchoredPosition - background.rect.center).y
            });
           
        }

    }

    private float SnapFloat(float value, AxisOptions snapAxis) {
        if (value == 0)
            return value;

        if (axisOptions == AxisOptions.Both) {
            float angle = Vector2.Angle(input, Vector2.up);
            if (snapAxis == AxisOptions.Horizontal) {
                if (angle < 22.5f || angle > 157.5f)
                    return 0;
                else
                    return (value > 0) ? 1 : -1;
            } else if (snapAxis == AxisOptions.Vertical) {
                if (angle > 67.5f && angle < 112.5f)
                    return 0;
                else
                    return (value > 0) ? 1 : -1;
            }
            return value;
        } else {
            if (value > 0)
                return 1;
            if (value < 0)
                return -1;
        }
        return 0;
    }

    private void FormatInput() {
        if (axisOptions == AxisOptions.Horizontal)
            input = new Vector2(input.x, 0f);
        else if (axisOptions == AxisOptions.Vertical)
            input = new Vector2(0f, input.y);
        else if (axisOptions == AxisOptions.None || axisOptions == AxisOptions.Shield) {
            input = Vector2.zero;
        }
    }

    public void SetAxisOption(AxisOptions axis) {
        this.axisOptions = axis;
    }

    public void SetAnchorPosition(Vector2 anchor) {
        this.GetComponent<RectTransform>().anchoredPosition = anchor;
    }

    private void ChangeToggleValue() {
        toggleValue = !toggleValue;
    }
}