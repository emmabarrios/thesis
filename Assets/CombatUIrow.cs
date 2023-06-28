using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatUIrow : MonoBehaviour
{
    public List<GameObject> slots = new List<GameObject>();
    public PouchItem pouchItemPrefab;

    public void LoadSlotItems(List<Item> itemList) {

        for (int i = 0; i < slots.Count; i++) {

            if (itemList[i] != null) {
                PouchItem currentPouchItem = Instantiate(pouchItemPrefab);
                currentPouchItem.itemSO = itemList[i];
                currentPouchItem.SetPouchItemImageSprite();
                currentPouchItem.transform.SetParent(slots[i].transform, false);
            }
        }
    }
}
