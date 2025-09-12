using UnityEngine;

/// <summary>
/// インベントリスロット情報を保持するクラス
/// </summary>
[System.Serializable]
public class SlotContainer {
    // 表示するアイテムの画像
    public Sprite itemSprite;
    // アイテムの個数
    public int itemCount;
    // どのテーブル（プレイヤー/クラフト/結果）に属するか
    [HideInInspector] public int tableID;
    // 紐づくUIスロット
    [HideInInspector] public SlotTemplate slot;
}