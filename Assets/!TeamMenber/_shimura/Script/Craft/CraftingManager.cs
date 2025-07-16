using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CraftingManager:SystemObject<CraftingManager> {

    
    public ItemManager itemManager;
    
    private const int _POTION_ID = 4007;
    
    public void ItemInitialize() {
        itemManager.Initialize();
    }

    // 合成処理：2つのアイテムを合成して新しいアイテムを返す
    public ItemBase CraftItem(string name1, string name2) {
        
        ItemBase item1 = itemManager.GetItemByName(name1);
        ItemBase item2 = itemManager.GetItemByName(name2);

        if (item1 == null || item2 == null) {
            Debug.LogWarning("合成に失敗：アイテムが見つかりません");
            return null;
        }

        // 合成ルール（例：薬草 + 鉄 → 回復薬）
        if ((item1.itemName == "薬草" && item2.itemName == "鉄") ||
 (item1.itemName == "鉄" && item2.itemName == "薬草")) {
            return ItemManager.instance.GetItemByID(_POTION_ID);
        }

        // 他の合成ルールを追加可能
        Debug.Log("合成ルールに一致しません");
        return null;
    }

}
