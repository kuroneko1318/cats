using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemManager
{

    // ゲーム内に存在する全アイテムの一覧（Listで管理）
    private List<ItemBase> itemList = new List<ItemBase>();

    //  コンストラクタ：初期化時に全アイテムを登録
    public ItemManager() {
        // ここでアイテム5種を登録（ID）
        itemList.Add(new ItemBase("薬草", 4001, eItemType.Material));
        itemList.Add(new ItemBase("棒", 4002, eItemType.Material));
        itemList.Add(new ItemBase("石A", 4003, eItemType.Material));
        itemList.Add(new ItemBase("石B", 4004, eItemType.Material));
        itemList.Add(new ItemBase("鉄", 4005, eItemType.Material));
    }

    //  IDからアイテムを取得する
    public ItemBase GetItemByID(int id) {
        return itemList.Find(i => i.itemID == id);
    }

    //  名前からアイテムを取得する
    public ItemBase GetItemByName(string name) {
        return itemList.Find(i => i.itemName == name);
    }

    //  登録されているすべてのアイテムを取得（デバッグ・UI用など）
    public List<ItemBase> GetAllItems() {
        return new List<ItemBase>(itemList); // 外部から編集されないようコピーして返す
    }

}
