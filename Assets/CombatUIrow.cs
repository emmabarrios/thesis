using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatUIrow : MonoBehaviour
{
    public List<GameObject> slots = new List<GameObject>();

    public void LoadSlotItems(List<Item> itemList) {

        for (int i = 0; i < slots.Count; i++) {
            PouchItem currentSlotPouchItem = slots[i].GetComponentInChildren<PouchItem>();
            currentSlotPouchItem.itemSO = itemList[i];
            currentSlotPouchItem.SetPouchItemImageSprite();
        }
    }
}
