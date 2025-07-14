
using UnityEngine;
using UnityEngine.EventSystems;

// このスクリプトは UI Image をドラッグ可能にし、指定の UI Image 上でのみドロップを成功と判定します
public class DragAndDropImage : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    private RectTransform rectTransform;     // 自身の RectTransform
    private CanvasGroup canvasGroup;         // 透明度やクリック判定を制御
    private Canvas canvas;                   // 親の Canvas（スケール調整用）

    private Vector2 originalPosition;        // ドラッグ開始前の位置
    public bool droppedOnValidTarget = false; // ドロップ先が有効だったかどうか

    void Awake() {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData) {
        originalPosition = rectTransform.anchoredPosition; // 現在の位置を保存
        canvasGroup.alpha = 0.6f;                           // 半透明にする
        canvasGroup.blocksRaycasts = false;                // 他のUIとの干渉を無効化
        droppedOnValidTarget = false;                      // ドロップ成功フラグを初期化
    }

    public void OnDrag(PointerEventData eventData) {
        // マウスの移動量に応じて位置を更新（Canvasのスケールを考慮）
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData) {
        canvasGroup.alpha = 1f;               // 透明度を元に戻す
        canvasGroup.blocksRaycasts = true;    // UIとの干渉を再有効化

        // ドロップ先が有効でなければ元の位置に戻す
        if (!droppedOnValidTarget) {
            rectTransform.anchoredPosition = originalPosition;
        }
    }
}
