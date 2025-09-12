using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// スロットUIのテンプレートクラス  
/// ・マウスクリックを検知して ItemCrafting に通知する  
/// ・アイテム画像、個数表示を保持
/// </summary>
public class SlotTemplate : MonoBehaviour, IPointerClickHandler {
    [Header("UI要素")]
    public Image container; // スロットの背景
    public Image item;      // アイテムの画像
    public TMP_Text count;  // アイテムの個数表示（TextMeshPro）

    [HideInInspector] public bool hasClicked = false; // クリックされたかどうか
    [HideInInspector] public ItemCrafting craftingController; // 呼び出し元のItemCrafting参照

    /// <summary>
    /// スロットがクリックされた時に呼び出される
    /// </summary>
    public void OnPointerClick(PointerEventData eventData) {
        hasClicked = true;                 // クリック状態を記録
        craftingController.ClickEventRecheck(); // ItemCraftingへ通知
    }
}