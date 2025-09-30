using UnityEngine;
using System.Collections.Generic;
using System;

public class CraftManager : SystemObject<CraftManager> {
    private CraftingSlot slot1UI;
    private CraftingSlot slot2UI;
    private CraftingSlot resultUI;

    private InventorySlot slot1Data = new InventorySlot();
    private InventorySlot slot2Data = new InventorySlot();
    private InventorySlot resultData = new InventorySlot();

    // レシピ辞書（順不同対応）
    private Dictionary<(string, string), string> recipes = new Dictionary<(string, string), string>();

    public override void Initialize() {
        Debug.Log("CraftManager Initialize called!");
        // レシピ登録例
        AddRecipe("木", "木", "棒");
        AddRecipe("棒", "棒", "えぐい棒");
        AddRecipe("えぐい棒", "えぐい棒", "きもい棒");
        AddRecipe("鉄", "棒", "普通の剣");
        AddRecipe("鉄", "紐", "普通のアーマー");
        AddRecipe("薬草", "紐", "回復薬");
        AddRecipe("回復薬", "木の実", "攻撃薬");
        AddRecipe("回復薬", "黒曜石", "防御薬");
        //AddRecipe("木", "紐", "丈夫な木");
        //AddRecipe("ライトストーン", "石", "研磨石");
        //AddRecipe("研磨石", "黒曜石", "鋭い黒曜石");
        AddRecipe("ライトストーン", "ブラッドストーン", "魔石");
        AddRecipe("魔石", "普通の剣", "魔剣ズルフィカール");
        AddRecipe("魔剣ズルフィカール", "魔剣ズルフィカール", "天上天下唯我独尊ブレード");
        AddRecipe("魔石", "普通のアーマー", "マジックアーマー");
        AddRecipe("マジックアーマー", "マジックアーマー", "金城鉄壁金剛二天アーマー");
        //AddRecipe("鉄", "木", "普通の柄");
        //AddRecipe("棒", "石", "斧");
        //AddRecipe("普通の柄", "融合石", "魔剣の柄");
        //AddRecipe("魔剣の剣身", "魔剣の柄", "未完成の魔剣");
        //AddRecipe("未完成の魔剣", "ブラックマリン", "魔剣ズルフィカール");
        //AddRecipe("普通の剣", "聖なる石", "聖剣エクスカリバー");
        //AddRecipe("棒", "ライトストーン", "ライトソード");
        //AddRecipe("棒", "ブラッドストーン", "ブラッドソード");
        //AddRecipe("棒", "ブラックマリン", "ブラックソード");
        //AddRecipe("棒", "シーネリアン", "シーソード");
        //AddRecipe("棒", "マカライト", "マカライトソード");
        //AddRecipe("普通のアーマー", "魔石", "マジックアーマー");
        //AddRecipe("普通のアーマー", "融合石", "フュージョンアーマー");
        //AddRecipe("普通のアーマー", "聖なる石", "セイントアーマー");
        //AddRecipe("", "", "");
        //AddRecipe("", "", "");
        //AddRecipe("", "", "");
        //AddRecipe("", "", "");
        //AddRecipe("", "", "");
        //AddRecipe("", "", "");
        //AddRecipe("", "", "");
    }

    public void SetCraftSlot(int index, InventorySlot data, CraftingSlot ui) {
        switch (index) {
            case 0:
                slot1Data = data;
                slot1UI = ui;
                break;
            case 1:
                slot2Data = data;
                slot2UI = ui;
                break;
            case 2:
                resultData = data;
                resultUI = ui;
                break;
        }

        // UI 初期化
        ui.SetItem(data.item, data.amount);
    }

    void AddRecipe(string item1, string item2, string result) {
        recipes[(item1, item2)] = result;
        recipes[(item2, item1)] = result;
        Debug.Log($"[Recipe Added] {item1} + {item2} => {result}");
    }

    void Update() {
        UpdateResult();
    }

    //public void UpdateResult() {
    //    if (slot1Data.IsEmpty || slot2Data.IsEmpty) {
    //        resultData.Clear();
    //        resultUI?.Clear();
    //        return;
    //    }

    //    if (recipes.TryGetValue((slot1Data.item.itemName, slot2Data.item.itemName), out string resultName)) {
    //        ItemBase resultItem = ItemManager.Instance?.GetItemByName(resultName);
    //        int craftCount = Mathf.Min(slot1Data.amount, slot2Data.amount);

    //        resultData.SetItem(resultItem, craftCount);
    //        resultUI?.SetItem(resultItem, craftCount);
    //    }
    //    else {
    //        resultData.Clear();
    //        resultUI?.Clear();
    //    }
    //}

    public void UpdateResult() {
        var slotA = slot1Data;
        var slotB = slot2Data;

        if (slotA.item == null || slotB.item == null) {
            resultData.Clear();
            resultUI?.SetItem(null, 0);
            return;
        }

        ItemBase result = GetRecipeResult(slotA.item, slotB.item);

        if (result != null) {
            int craftCount = Mathf.Min(slotA.amount, slotB.amount);
            resultData.SetItem(result, craftCount);
            resultUI?.SetItem(result, craftCount);
            Debug.Log($"[Craft Debug] クラフト成功: {result.itemName} x{craftCount}");
        }
        else {
            resultData.Clear();
            resultUI?.SetItem(null, 0);
        }
    }

    private ItemBase GetRecipeResult(ItemBase item1, ItemBase item2) {
        string name1 = item1.itemName.Trim();
        string name2 = item2.itemName.Trim();

        string resultName;
        if (recipes.TryGetValue((name1, name2), out resultName) ||
            recipes.TryGetValue((name2, name1), out resultName)) {
            // まず通常アイテムを探す
            var resultItem = ItemManager.Instance.GetItemByName(resultName);
            if (resultItem != null) return resultItem;

            // 次に武器を探す
            var resultWeapon = ItemManager.Instance.GetWeaponByName(resultName);
            if (resultWeapon != null) return resultWeapon;

            // 次に防具を探す
            var resultArmor = ItemManager.Instance.GetArmorByName(resultName);
            if (resultArmor != null) return resultArmor;

            Debug.LogError($"[Craft Debug] ItemManagerに '{resultName}' が存在しません！（Item/Weapon/Armor 全部探した）");
        }

        Debug.Log("[Craft Debug] レシピなし: " + name1 + " + " + name2);
        return null;
    }


    // 結果取得処理（Aボタンで呼ばれる想定）
    public string TakeResult(out int craftedAmount) {
        craftedAmount = 0;
        if (resultData.IsEmpty) return null;

        string craftedItem = resultData.item.itemName;
        craftedAmount = resultData.amount;

        // 素材消費
        slot1Data.Consume(craftedAmount);
        slot2Data.Consume(craftedAmount);

        slot1UI?.SetItem(slot1Data.item, slot1Data.amount);
        slot2UI?.SetItem(slot2Data.item, slot2Data.amount);

        // 結果スロット消去
        resultData.Clear();
        resultUI?.Clear();

        return craftedItem;
    }
}
