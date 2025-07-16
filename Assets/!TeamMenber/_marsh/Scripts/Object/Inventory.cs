using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {
    public int slotCount = 100;
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

    public bool AddItem(WeaponBase weapon, int amount = 1) {
        // 既存スタックに追加
        foreach (var slot in slots) {
            if (!slot.IsEmpty && slot.weapon.weaponID == weapon.weaponID) {
                slot.AddItem(weapon, amount);
                return true;
            }
        }

        // 空きスロットに追加
        foreach (var slot in slots) {
            if (slot.IsEmpty) {
                slot.AddItem(weapon, amount);
                return true;
            }
        }

        Debug.Log("インベントリがいっぱいです");
        return false;
    }

    public bool RemoveItem(ItemBase item, int amount = 1) {
        foreach (var slot in slots) {
            if (!slot.IsEmpty && slot.item.itemID == item.itemID) {
                if (slot.amount >= amount) {
                    slot.RemoveItem(amount);
                    return true;
                }
            }
        }

        Debug.Log("指定されたアイテムが見つからないか、数が足りません");
        return false;
    }

}
