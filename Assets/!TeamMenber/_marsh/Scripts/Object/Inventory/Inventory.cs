using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {
    public GameObject ItemPopupPrefab;
    private ItemPopupController currentPopup;
    //インベントリスロットの最大数
    public int slotCount = 81;
    //インベントリの中身保存用
    [SerializeField]public InventorySlot[] slots;
    
    //実行時にインベントリのスロット生成する
    private void Awake() {
        slots = new InventorySlot[slotCount];
        for (int i = 0; i < slotCount; i++) {
            slots[i] = new InventorySlot();
        }
    }

    //アイテムの追加
    public bool AddItem(ItemBase item, int amount = 1) {
        Vector3 popupPos = transform.position + Vector3.up * 2f;
        GameObject popupObj = Instantiate(ItemPopupPrefab, popupPos, Quaternion.identity);
        currentPopup = popupObj.GetComponent<ItemPopupController>();

        currentPopup.GetItemUI(item.itemName, amount);
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

    /// <summary>
    /// アイテムの削除
    /// </summary>
    /// <param 削除したいアイテム="item"></param>
    /// <param 数量="amount"></param>
    /// <returns></returns>
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
