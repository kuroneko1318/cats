using UnityEngine;

public class InventorySlot {
    public ItemBase item;
    public int amount;

    public InventorySlot() {
        item = null;
        amount = 0;
    }

    // 空かどうかを判定するプロパティ
    public bool IsEmpty => item == null;

    // アイテムを取り出す
    public ItemBase TakeItem() {
        var temp = item;
        item = null;
        amount = 0;
        return temp;
    }

    // アイテムを設定する
    public void SetItem(ItemBase newItem, int newAmount = 1) {
        item = newItem;
        amount = newAmount;
    }

    // アイテムを追加する（既存アイテムと同じ場合は数量加算）
    public void AddItem(ItemBase newItem, int addAmount = 1) {
        if (item == null) {
            item = newItem;
            amount = addAmount;
        }
        else if (item == newItem) {
            amount += addAmount;
        }
        else {
            Debug.LogWarning("違うアイテムを追加しようとしました。SetItemを使ってください。");
        }
    }

    // 指定数量を削除
    public void RemoveItem(int removeAmount = 1) {
        if (amount >= removeAmount) {
            amount -= removeAmount;
            if (amount == 0)
                item = null;
        }
        else {
            Debug.LogWarning("削除量が多すぎます。");
        }
    }
}
