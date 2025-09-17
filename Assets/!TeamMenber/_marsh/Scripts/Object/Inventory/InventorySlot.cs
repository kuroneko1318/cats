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
            Debug.LogWarning("AddItem: 無効なアイテムまたは数量です");
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

    /// <summary> スロットからアイテムを取り出す（取得してスロットを空にする）
    public ItemBase TakeItem() {
        ItemBase temp = item;
        item = null;
        amount = 0;
        return temp;
    }

    /// <summary> スロットにアイテムをセットする（上書き）
    public void SetItem(ItemBase newItem, int newAmount = 1) {
        item = newItem;
        amount = newItem != null ? newAmount : 0;
    }
}
