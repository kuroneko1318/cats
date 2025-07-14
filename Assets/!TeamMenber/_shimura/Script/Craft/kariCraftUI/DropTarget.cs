

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// このスクリプトは、指定された名前の UI Image 上でのみドロップを成功と判定します
public class DropTarget : MonoBehaviour, IDropHandler {
    private Image validDropArea; // ドロップ先の Image を自動取得

    void Start() {
        // "DropTarget" という名前の GameObject を探して Image を取得
        GameObject targetObject = GameObject.Find("Item1");
        if (targetObject != null) {
            validDropArea = targetObject.GetComponent<Image>();
        }

        // 見つからなかった場合は警告を表示
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
            draggable.transform.SetParent(validDropArea.transform);
        }
        else {
            Debug.Log("ドロップ位置が有効領域外です");
        }
    }
}

