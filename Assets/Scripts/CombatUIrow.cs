using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatUIrow : MonoBehaviour
{
    public List<GameObject> slots = new List<GameObject>();
    public PouchItem pouchItemPrefab;

    public void LoadSlotItems(List<QuickItem> itemList) {

        for (int i = 0; i < slots.Count; i++) {

            if (itemList[i] != null) {
                PouchItem currentPouchItem = Instantiate(pouchItemPrefab);
                currentPouchItem.itemSO = itemList[i];
                currentPouchItem.InitializeQuickItemGraphics();
                currentPouchItem.transform.SetParent(slots[i].transform, false);

                GameObject slot_child = slots[i];
                slot_child.transform.GetChild(0).GetComponent<Image>().sprite = currentPouchItem.itemSO._slot_empty_sprite;
            }
        }
    }
}
