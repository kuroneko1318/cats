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
        AddRecipe("鉄", "棒", "普通の剣");
        AddRecipe("鉄", "紐", "普通のアーマー");
        AddRecipe("薬草", "紐", "回復薬");
        AddRecipe("棒", "ライトストーン", "ライトソード");
        AddRecipe("木", "紐", "丈夫な木");
        AddRecipe("ライトストーン", "石", "研磨石");
        AddRecipe("研磨石", "黒曜石", "鋭い黒曜石");
        AddRecipe("ライトストーン", "ブラッドストーン", "魔石");
        AddRecipe("魔石", "鋭い黒曜石", "魔剣の剣身");
        AddRecipe("鉄", "木", "普通の柄");
        AddRecipe("棒", "石", "斧");
        AddRecipe("普通の柄", "融合石", "魔剣の柄");
        AddRecipe("マカライト", "シーネリアン", "融合石");
        AddRecipe("魔剣の剣身", "魔剣の柄", "未完成の魔剣");
        AddRecipe("未完成の魔剣", "ブラックマリン", "魔剣ズルフィカール");
        AddRecipe("未完成の魔剣", "ライトストーン", "魔剣フルンティング");
        AddRecipe("未完成の魔剣", "ブラッドストーン", "魔剣ネイリング");
        //AddRecipe("棒", "ブラッドストーン", "");
        //AddRecipe("", "", "");
        //AddRecipe("", "", "");
        //AddRecipe("", "", "");
        //AddRecipe("", "", "");
        //AddRecipe("", "", "");
        //AddRecipe("", "", "");
        //AddRecipe("", "", "");
        //AddRecipe("", "", "");
        //AddRecipe("", "", "");
        //AddRecipe("", "", "");
        //AddRecipe("", "", "");
        //AddRecipe("", "", "");
        //AddRecipe("", "", "");
        //AddRecipe("", "", "");
        //AddRecipe("", "", "");
        //AddRecipe("", "", "");
        //AddRecipe("", "", "");
        //AddRecipe("", "", "");
        //AddRecipe("", "", "");
        //AddRecipe("", "", "");
        //AddRecipe("", "", "");
        //AddRecipe("", "", "");
        //AddRecipe("", "", "");
        //AddRecipe("", "", "");
        //AddRecipe("", "", "");
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
        if (Input.GetKeyDown(KeyCode.P)) {
            PrintAllRecipes();
        }
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
            Debug.Log("[Craft Debug] どちらかのスロットが空なのでクラフト不可");
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
        string resultName;

        foreach (var kvp in recipes) {
            var key = kvp.Key;
            if ((key.Item1 == item1.itemName && key.Item2 == item2.itemName) ||
                (key.Item1 == item2.itemName && key.Item2 == item1.itemName)) {
                resultName = kvp.Value;
                return ItemManager.Instance.GetItemByName(resultName);
            }
        }

        Debug.Log($"[Craft Debug] レシピなし: {item1.itemName} + {item2.itemName}");
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

    public void PrintAllRecipes() {
        Debug.Log("=== レシピ一覧 ===");
        foreach (var kv in recipes) {
            Debug.Log($"{kv.Key.Item1} + {kv.Key.Item2} => {kv.Value}");
        }
    }
}
