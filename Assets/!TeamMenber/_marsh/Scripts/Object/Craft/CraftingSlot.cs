using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingSlot : InventoryUI {

    [Header("データ")]
    public ItemBase currentItem;
    public int amount;

    /// <summary>
    /// アイテムをセット
    /// </summary>
    public void SetItem(ItemBase item, int count = 1) {
        if (item != null) {
            icon.sprite = item.icon;
            icon.enabled = true;
            UpdateAmountUI();
        }
        else {
            icon.enabled = false;
            amountText.text = "";
        }
    }

    /// <summary>
    /// スロットを空にする
    /// </summary>
    public void Clear() {
        currentItem = null;
        amount = 0;
        icon.sprite = null;
        icon.enabled = false;
        amountText.text = "";
        UpdateAmountUI();
    }

    /// <summary>
    /// 数量を減らす
    /// </summary>
    public void Consume(int count) {
        amount -= count;
        if (amount <= 0) {
            Clear();
        }
        else {
            UpdateAmountUI();
        }
    }

    /// <summary>
    /// 空かどうか
    /// </summary>
    public bool IsEmpty() {
        return string.IsNullOrEmpty(currentItem.itemName) || amount <= 0;
    }

    /// <summary>
    /// 数量テキスト更新
    /// </summary>
    private void UpdateAmountUI() {
        if (amountText != null) {
            amountText.text = amount > 1 ? amount.ToString() : "";
        }
    }
}
