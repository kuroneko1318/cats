using UnityEngine;

public class EquipmentUI : MonoBehaviour {
    public InventorySlot weaponSlot;
    public InventorySlot armorSlot;

    public void Equip(ItemBase item) {
        if (item.type == eItemType.Weapon) {
            weaponSlot.AddItem(item, 1);
        }
        else if (item.type == eItemType.Armor) {
            armorSlot.AddItem(item, 1);
        }
    }
}
