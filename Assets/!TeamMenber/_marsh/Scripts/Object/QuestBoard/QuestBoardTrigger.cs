using UnityEngine;

public class QuestBoardTrigger : MonoBehaviour {
    public GameObject questBoardPanel;   // クエストボードUI
    public float interactRange = 3f;     // プレイヤーとの距離

    private bool isPlayerNearby = false;

    private void Start() {
        questBoardPanel.SetActive(false);
    }

    public void OpenBoard() {
        questBoardPanel.SetActive(true);
    }

    public void CloseBoard() {
        questBoardPanel.SetActive(false);
    }

    // （任意）Gizmoで範囲確認
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}