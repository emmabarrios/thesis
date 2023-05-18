using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class PouchIndicator : MonoBehaviour
{
    public RectTransform[] indicators;
    public float increasedSize = 2f;
    private Vector2 originalSize;

    public Carousel carousel = null;

    private void Start() {

        carousel.OnSwipe += ActivateIndicatorByIndex;

        originalSize = indicators[0].sizeDelta;

        indicators[0].sizeDelta = originalSize * increasedSize;
    }

    public void ActivateIndicatorByIndex(object sender, Carousel.OnSwipeEventArgs e) {

        for (int i = 0; i < indicators.Length; i++) {
            if (i == e.index) {
                indicators[i].sizeDelta = originalSize * increasedSize;
            } else {
                if (indicators[i].sizeDelta != originalSize) {
                    indicators[i].sizeDelta = originalSize;

                }
            }
        }
    }


    
}
