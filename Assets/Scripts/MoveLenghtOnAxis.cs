using UnityEngine;
using UnityEngine.UI;

public class MoveLenghtOnAxis : MonoBehaviour
{
    public enum Axis { Horizontal, Vertical }
    public Axis axis = Axis.Horizontal;
    [SerializeField] private Toggle toggle = null;
    [SerializeField] private float length = 100f;
    public Vector2 fromPosition;
    private Vector2 targetPosition;

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        fromPosition = rectTransform.position;

        toggle.onValueChanged.AddListener(delegate {
            Move();
        });
    }

    void Move() {

        //if (toggle.isOn) {
        //    if (axis == Axis.Vertical) {
        //        targetPosition = fromPosition + new Vector2(0f, lenght);
        //        rectTransform.position = targetPosition;
        //    } else {
        //        targetPosition = fromPosition + new Vector2(lenght, 0f);
        //        rectTransform.position = targetPosition;
        //    }

        //} else {
        //    if (axis == Axis.Vertical) {
        //        targetPosition = fromPosition + new Vector2(0f, -lenght);
        //        rectTransform.position = targetPosition; ;
        //    } else {
        //        targetPosition = fromPosition + new Vector2(-lenght,0f);
        //        rectTransform.position = targetPosition; ;
        //    }

        //}

        Vector2 offset = Vector2.zero;

        if (toggle.isOn) {
            offset = axis == Axis.Vertical ? new Vector2(0f, length) : new Vector2(length, 0f);
        } else {
            offset = Vector2.zero;
        }

        targetPosition = fromPosition + offset;
        rectTransform.position = targetPosition;

    }

}
