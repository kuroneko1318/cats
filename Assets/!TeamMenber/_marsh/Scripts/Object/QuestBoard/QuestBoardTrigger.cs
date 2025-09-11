using UnityEngine;

public class QuestBoardTrigger : MonoBehaviour {
    public GameObject questBoardPanel;   // クエストボードUI
    public float interactRange = 3f;     // プレイヤーとの距離
    private Transform player;

    private bool isPlayerNearby = false;

    private void Start() {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        questBoardPanel.SetActive(false);
    }

    private void Update() {
        if (player == null) return;

        // プレイヤーが一定範囲内か
        isPlayerNearby = Vector3.Distance(player.position, transform.position) <= interactRange;

        // E キーで開く
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E)) {
            questBoardPanel.SetActive(true);
        }

        // X キーで閉じる
        if (questBoardPanel.activeSelf && Input.GetKeyDown(KeyCode.X)) {
            questBoardPanel.SetActive(false);
        }
    }

    // （任意）Gizmoで範囲確認
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}