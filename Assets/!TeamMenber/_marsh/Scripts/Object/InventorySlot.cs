using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class InventorySlot {
    public ItemBase item;
    public WeaponBase weapon;
    public ArmorBase armor;
    public int amount;

    public bool IsEmpty => item == null;

    public void AddItem(ItemBase newItem, int amount) {

        if (newItem == null || amount <= 0) {
            Debug.LogWarning("AddItem: 無効なアイテムまたは数量です");
            return;
        }

        if (IsEmpty) {
            item = newItem;
            this.amount = amount;
        }
        else if(item.itemID == newItem.itemID){
            this.amount += amount;
        }
    }
    public void AddItem(WeaponBase newWeapon, int amount) {

        if (newWeapon == null || amount <= 0) {
            Debug.LogWarning("AddItem: 無効なアイテムまたは数量です");
            return;
        }

        if (IsEmpty) {
            weapon = newWeapon;
            this.amount = amount;
        }
        else if (weapon.weaponID == newWeapon.weaponID) {
            this.amount += amount;
        }
    }
    public void RemoveItem(int removeAmount) {
        amount -= removeAmount;
        if (amount <= 0) {
            item = null;
            amount = 0;
        }

    }
}

