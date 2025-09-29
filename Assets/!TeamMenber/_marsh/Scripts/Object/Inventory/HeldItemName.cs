using UnityEngine;
using TMPro;

public class HeldItemName : MonoBehaviour {
    [Header("UI")]
    public TextMeshProUGUI itemNameText; // 表示用のテキスト

    void Update() {
        // InventoryManager から現在の heldItem を取得
        var inv = InventoryManager.Instance;
        if (inv != null && inv.IsHoldingItem) {
            itemNameText.text = inv.IsHoldingItem
                ? inv.GetHeldItemName()
                : "";
        }
        else {
            itemNameText.text = "";
        }
    }
}

