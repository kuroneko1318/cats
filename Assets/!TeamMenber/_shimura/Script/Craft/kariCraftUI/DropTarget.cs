
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropTarget : MonoBehaviour, IDropHandler {
    private Image validDropArea;

    void Start() {
        // "DropTarget" という名前の GameObject を探して Image を取得
        GameObject targetObject = GameObject.Find("DropTarget");
        if (targetObject != null) {
            validDropArea = targetObject.GetComponent<Image>();
        }

        if (validDropArea == null) {
            Debug.LogWarning("DropTarget が見つからないか、Image コンポーネントがありません");
        }
    }

    public void OnDrop(PointerEventData eventData) {
        DragAndDropImage draggable = eventData.pointerDrag?.GetComponent<DragAndDropImage>();
        if (draggable == null || validDropArea == null) return;

        RectTransform dropRect = validDropArea.GetComponent<RectTransform>();
        Vector2 localMousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
        dropRect, eventData.position, eventData.pressEventCamera, out localMousePos);

        if (dropRect.rect.Contains(localMousePos)) {
            Debug.Log("有効なドロップ領域に入りました！");
            draggable.droppedOnValidTarget = true;

            // 親を変更（ターゲットの子にする）
            draggable.transform.SetParent(validDropArea.transform);

            // RectTransform を完全に重ねるように調整
            RectTransform draggableRect = draggable.GetComponent<RectTransform>();
            draggableRect.anchoredPosition = Vector2.zero; // 中央に配置
            draggableRect.sizeDelta = dropRect.sizeDelta;  // サイズを合わせる
        }
        else {
            Debug.Log("ドロップ位置が有効領域外です");
        }
    }
}
