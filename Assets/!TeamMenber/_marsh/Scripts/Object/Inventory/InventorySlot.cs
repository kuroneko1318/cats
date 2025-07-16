using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySlot {
    public string name;
    public int amount;
    public ItemBase item;

    public bool IsEmpty => item == null;

    public void AddItem(ItemBase newItem, int amount) {
        if (newItem == null || amount <= 0) {
            Debug.LogWarning("AddItem: –³Œø‚ÈƒAƒCƒeƒ€‚Ü‚½‚Í”—Ê‚Å‚·");
            return;
        }

        if (IsEmpty) {
            item = newItem;
            this.amount = amount;
            name = newItem.itemName;
        }
        else if (item.itemID == newItem.itemID) {
            this.amount += amount;
        }
    }

    public void RemoveItem(int removeAmount) {
        amount -= removeAmount;
        if (amount <= 0) {
            item = null;
            amount = 0;
            name = null;
        }
    }
}
