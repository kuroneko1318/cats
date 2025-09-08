using UnityEngine;


public class StageGate : MonoBehaviour {
    private StageManager stageManager;

    [Header("移動先設定")]
    public bool goToArea2 = true;  // true = エリア2へ, false = エリア1へ

    private void Awake() {
        // シーン上に常駐している StageManager を探す
        stageManager = FindObjectOfType<StageManager>();

        if (stageManager == null) {
            Debug.LogError("StageManager がシーンに存在しません！");
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") && stageManager != null) {
            if (goToArea2)
                stageManager.MoveToArea2();
            else
                stageManager.ReturnToArea1();
        }
    }
}
