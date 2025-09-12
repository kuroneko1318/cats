using UnityEngine;

/// <summary>
/// クラフト可能なアイテム情報
/// </summary>
[System.Serializable]
public class Item {
    // アイテムの画像
    public Sprite itemSprite;
    // 複数個スタック可能か
    public bool stackable = false;
    // クラフトレシピ文字列 
    public string craftRecipe;
}