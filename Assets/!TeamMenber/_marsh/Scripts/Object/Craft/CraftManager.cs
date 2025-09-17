using UnityEngine;
using System.Collections.Generic;

public class CraftManager : MonoBehaviour {
    public CraftingSlot slot1;
    public CraftingSlot slot2;
    public CraftingSlot resultSlot;

    // レシピ辞書（順不同対応）
    private Dictionary<(string, string), string> recipes = new Dictionary<(string, string), string>();

    void Start() {
        // レシピ登録例
        AddRecipe("木", "石", "斧");
        AddRecipe("鉄", "木", "剣");
        AddRecipe("薬草", "紐", "回復薬");
    }

    void AddRecipe(string item1, string item2, string result) {
        recipes[(item1, item2)] = result;
        recipes[(item2, item1)] = result;
    }

    void Update() {
        UpdateResult();
    }

    void UpdateResult() {
        resultSlot.Clear();

        if (slot1.IsEmpty() || slot2.IsEmpty())
            return;

        if (recipes.TryGetValue((slot1.currentItem, slot2.currentItem), out string result)) {
            int craftCount = Mathf.Min(slot1.amount, slot2.amount);

            // アイコンを ItemManager から取得（例）
            Sprite resultIcon = ItemManager.Instance?.GetItemByName(result)?.icon;

            resultSlot.SetItem(result, craftCount, resultIcon);
        }
    }

    // 結果取得処理（Aボタンで呼ばれる想定）
    public string TakeResult(out int craftedAmount) {
        craftedAmount = 0;
        if (resultSlot.IsEmpty()) return null;

        string craftedItem = resultSlot.currentItem;
        craftedAmount = resultSlot.amount;

        // 素材消費
        slot1.Consume(craftedAmount);
        slot2.Consume(craftedAmount);

        // 結果スロット消去
        resultSlot.Clear();

        return craftedItem;
    }
}
