using UnityEngine;

public class StageGate : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        // プレイヤー以外なら無視
        if (!other.CompareTag("Player")) return;

        // UIを表示してエリア選択を促す
        StageSelectUI ui = FindObjectOfType<StageSelectUI>();
        if (ui != null) {
            ui.Show();
        }
    }

    private void OnTriggerExit(Collider other) {
        // プレイヤー以外なら無視
        if (!other.CompareTag("Player")) return;

        // UIを表示してエリア選択を促す
        StageSelectUI ui = FindObjectOfType<StageSelectUI>();
        if (ui != null) {
            ui.Hide();
        }
    }
}
