using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class AButton : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {

    public enum AxisOptions { Both, Horizontal, Vertical, None }
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

    private Canvas canvas;

    private Vector2 input = Vector2.zero;
    private Camera cam;

    public event EventHandler<OnHandleDragedEventArgs> OnHandleDraged;
    public event EventHandler<OnBlockingEventArgs> OnBlocking;
    public event EventHandler<OnParryEventArgs> OnParry;
    public event EventHandler<OnTensionReleasedEventArgs> OnTensionReleased;
    public event EventHandler OnHandleDroped;

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
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            Debug.LogError("The Joystick is not placed inside a canvas");
        Vector2 center = new Vector2(0.5f, 0.5f);
        background.pivot = center;
        handle.anchorMin = center;
        handle.anchorMax = center;
        handle.pivot = center;
        handle.anchoredPosition = Vector2.zero;
    }

    public void OnPointerDown(PointerEventData eventData) {

        if (axisOptions == AxisOptions.Both) {
            OnDrag(eventData);
        }

        if (axisOptions == AxisOptions.None) {
            OnBlocking?.Invoke(this, new OnBlockingEventArgs { a = "Blocking" });
        }
    }

    public void OnPointerUp(PointerEventData eventData) {
        input = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;

        if (axisOptions == AxisOptions.None) {
            OnBlocking?.Invoke(this, new OnBlockingEventArgs { a = "" });
            CheckForDoubleTap(eventData);
        }

        if (axisOptions == AxisOptions.Vertical) {
            OnTensionReleased?.Invoke(this, new OnTensionReleasedEventArgs { magnitude = storedMagnitude });
        }

        OnHandleDroped?.Invoke(this, EventArgs.Empty);
    }

    private void CheckForDoubleTap(PointerEventData eventData) {
        float currentTimeClick = eventData.clickTime;

        if (Mathf.Abs(currentTimeClick - lastTimeTap) < tapThreshold) {
            OnParry?.Invoke(this, new OnParryEventArgs { a = "Parry!"});
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
        else if (axisOptions == AxisOptions.None) {
            input = Vector2.zero;
        }
    }

    public void SetAxisOption(AxisOptions axis) {
        this.axisOptions = axis;
    }

}