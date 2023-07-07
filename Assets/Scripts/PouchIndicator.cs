using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class PouchIndicator : MonoBehaviour
{
    //public RectTransform[] indicators;
    public List<RectTransform> rowIndicators = new List<RectTransform>();
    public float increasedSize = 2f;
    private Vector2 originalSize;

    public Carousel carousel = null;
    public RectTransform rowIndicatorPrefab;
    private RectTransform pouchRectTransform;

    private void Start() {
        pouchRectTransform = GetComponent<RectTransform>();
        carousel.OnSwipe += ActivateIndicatorByIndex;
    }

    public void ActivateIndicatorByIndex(object sender, Carousel.OnSwipeEventArgs e) {

        for (int i = 0; i < rowIndicators.Count; i++) {
            if (i == e.index) {
                //rowIndicators[i].sizeDelta = originalSize * increasedSize;
                rowIndicators[i].gameObject.transform.localScale = originalSize * increasedSize;
            } else {
                if (rowIndicators[i].sizeDelta != originalSize) {
                    //rowIndicators[i].sizeDelta = originalSize;
                    rowIndicators[i].gameObject.transform.localScale = originalSize;
                }
            }
        }
    }

    public void InitializeRowIndicators(int rowCount) {
        for (int i = 0; i < rowCount; i++) {
            RectTransform rowIndicator = Instantiate(rowIndicatorPrefab, this.transform);
            rowIndicators.Add(rowIndicator);
        }

        originalSize = rowIndicators[0].transform.localScale;
        rowIndicators[0].transform.localScale = originalSize * increasedSize;
        Vector2 currentPos = pouchRectTransform.position;
        Vector2 targetPosition = currentPos + new Vector2(-1*((pouchRectTransform.sizeDelta.x/2) * rowCount/2),0f);
        pouchRectTransform.position = targetPosition;
    }
    
}
