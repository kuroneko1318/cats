using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {
    public int slotCount = 20;
    public InventorySlot[] slots;

    private void Awake() {
        slots = new InventorySlot[slotCount];
        for (int i = 0; i < slotCount; i++) {
            slots[i] = new InventorySlot();
        }
    }

    public bool AddItem(ItemBase item, int amount = 1) {
        // 既存スタックに追加
        foreach (var slot in slots) {
            if (!slot.IsEmpty && slot.item.itemID == item.itemID) {
                slot.AddItem(item, amount);
                return true;
            }
        }

        // 空きスロットに追加
        foreach (var slot in slots) {
            if (slot.IsEmpty) {
                slot.AddItem(item, amount);
                return true;
            }
        }

        Debug.Log("インベントリがいっぱいです");
        return false;
    }
}
