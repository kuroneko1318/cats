using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingSlot : MonoBehaviour {
    [Header("UI参照")]
    public Image itemIcon;
    public TextMeshProUGUI amountText;

    [Header("データ")]
    public string currentItem;
    public int amount;

    /// <summary>
    /// アイテムをセット
    /// </summary>
    public void SetItem(string item, int count = 1, Sprite icon = null) {
        currentItem = item;
        amount = count;

        if (itemIcon != null) {
            itemIcon.enabled = true;
            if (icon != null) itemIcon.sprite = icon;
        }

        UpdateAmountUI();
    }

    /// <summary>
    /// スロットを空にする
    /// </summary>
    public void Clear() {
        currentItem = null;
        amount = 0;

        if (itemIcon != null) itemIcon.enabled = false;
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
        return string.IsNullOrEmpty(currentItem) || amount <= 0;
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
