using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class InventorySlot {
    public ItemBase item;
    public int quantity;

    public bool IsEmpty => item == null || quantity <= 0;

    public void AddItem(ItemBase newItem, int amount) {
        if (item == null) {
            item = newItem;
            quantity = amount;
        }
        else if (item.itemID == newItem.itemID) {
            quantity += amount;
        }
    }
}

