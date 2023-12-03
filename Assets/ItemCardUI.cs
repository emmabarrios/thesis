using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCardUI : MonoBehaviour
{
    [SerializeField] private int rowNumber = 0;
    [SerializeField] private int colNumber = 0;

    public int RowNumber { get { return rowNumber; } }
    public int ColNumber { get { return colNumber; } }

    public UnityEngine.UI.Button button;
    public UnityEngine.UI.Button removeButton;
    public Item item;
    public GameObject iconGameObject;
    public Sprite emptySlotSprite;



    public enum CardState {
        Clicked,
        NotClicked
    }

    private void Start() {
        button.onClick.AddListener(() => SetCardStateToClicked());
        removeButton.onClick.AddListener(() => SendItemToGeneralInventory());
        iconGameObject = this.GetComponent<RectTransform>().GetChild(1).gameObject;
    }

    [Header("State settings")]
    public CardState cardCurrentState = CardState.NotClicked;

    public void ResetSlotCardState() {
        this.cardCurrentState = CardState.NotClicked;
    }
    
    public void SetCardStateToClicked() {
        this.cardCurrentState = CardState.Clicked;
    }

    public bool IsCardClicked() {
        return this.cardCurrentState == CardState.Clicked;
    }

    private void ToggleIconGameObject() {
        iconGameObject.SetActive(!iconGameObject.activeSelf);
    }
    public void UpdateItemOnSlot(Item item) {
        this.item = item;
        Image iconImage = iconGameObject.GetComponent<Image>();
        iconImage.sprite = item._menuSprite;
    }

    public bool HasItem() {
        return item != null;
    }

    public bool HasQuickItem() {
        return item is QuickItem;
    }   

    public bool HasWeaponItem() {
        return item is WeaponItem;
    }

    public Item GetItem() {
        return item;
    }

    public void ResetCard() {
        ResetSlotCardState();
        Image iconImage = iconGameObject.GetComponent<Image>();
        iconImage.sprite = emptySlotSprite;
        item = null;
    }

    public void SendItemToGeneralInventory() {
        if (item!=null) {
            GeneralInventory.instance.AddItem(GetItem());
            ResetCard();
        }
    }
}
