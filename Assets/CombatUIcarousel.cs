using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatUIcarousel : MonoBehaviour
{
    public GameObject rowPrefab;
    // This public list is just to visualize in inspector as debug
    public List<CombatUIrow> rows = new List<CombatUIrow>();

    public void InitializeUIcarousel(List<List<Item>> listOfItems) {
        int rowCount = listOfItems.Count;
        for (int i = 0; i < rowCount; i++) {
            GameObject tempRow = Instantiate(rowPrefab,this.transform);
            CombatUIrow row = tempRow.GetComponent<CombatUIrow>();
            row.LoadSlotItems(listOfItems[i]);
            // Add row to visualize in inspector
            rows.Add(row);
        }
    }
}
