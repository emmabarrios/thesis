using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeDetector : MonoBehaviour
{
    public enum SwipeDir { None, Left, Up, Down, Right, UpLeft, UpRight, DownLeft, DownRight };

    public float minSwipeLength = 5f;
    Vector2 firstPressPos;
    Vector2 secondPressPos;
    Vector2 currentSwipe;

    public SwipeDir swipeDirection;

    void Update() {

        if (Input.touches.Length > 0) {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began) {
                firstPressPos = new Vector2(t.position.x, t.position.y);
            }

            if (t.phase == TouchPhase.Ended) {
                secondPressPos = new Vector2(t.position.x, t.position.y);
                currentSwipe = new Vector3(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);

                if (currentSwipe.magnitude < minSwipeLength) {
                    swipeDirection = SwipeDir.None;
                    return;
                }

                float swipeAngle = Mathf.Atan2(currentSwipe.y, currentSwipe.x) * Mathf.Rad2Deg;

                if (swipeAngle < 0) {
                    swipeAngle += 360;
                }

                if (swipeAngle > 22.5f && swipeAngle <= 67.5f) {
                    swipeDirection = SwipeDir.UpRight;
                } else if (swipeAngle > 67.5f && swipeAngle <= 112.5f) {
                    swipeDirection = SwipeDir.Up;
                } else if (swipeAngle > 112.5f && swipeAngle <= 157.5f) {
                    swipeDirection = SwipeDir.UpLeft;
                } else if (swipeAngle > 157.5f && swipeAngle <= 202.5f) {
                    swipeDirection = SwipeDir.Left;
                } else if (swipeAngle > 202.5f && swipeAngle <= 247.5f) {
                    swipeDirection = SwipeDir.DownLeft;
                } else if (swipeAngle > 247.5f && swipeAngle <= 292.5f) {
                    swipeDirection = SwipeDir.Down;
                } else if (swipeAngle > 292.5f && swipeAngle <= 337.5f) {
                    swipeDirection = SwipeDir.DownRight;
                } else {
                    swipeDirection = SwipeDir.Right;
                }
            }
        }
    }
}

