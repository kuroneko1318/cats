using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour {
    public Image icon;
    public TextMeshProUGUI amountText;
    public Image highlight;

    public void SetSlot(ItemBase item, int amount) {
        if (item != null) {
            icon.sprite = item.icon;
            icon.enabled = true;
            amountText.text = amount.ToString();
        }
        else {
            icon.enabled = false;
            amountText.text = "";
        }
    }

    public void SetHighlight(bool isSelected) {
        if (highlight != null)
            highlight.enabled = isSelected;
    }
}
