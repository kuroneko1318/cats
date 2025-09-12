using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour {

    [Header("エリア設定")]
    [SerializeField] private GameObject area1SceneObject;   // シーン上の常駐エリア1
    [SerializeField] private GameObject area2Prefab;        // 移動先エリア2のプレハブ

    [Header("プレイヤー参照")]
    [SerializeField] private GameObject player;             // プレイヤーオブジェクト

    private GameObject currentArea2;                        // 現在生成中のエリア2

    /// <summary>
    /// エリア1 → エリア2 に移動
    /// </summary>
    public void EnterArea2() {
        // まだ生成されていなければ生成
        if (currentArea2 == null) {
            currentArea2 = Instantiate(area2Prefab, new Vector3(100, 0, 0), Quaternion.identity);
        }

        // StartPos を探す
        Transform startPos = currentArea2.transform.Find("StartPos");
        if (startPos == null) {
            Debug.LogError("Area2 に StartPos が見つかりません！");
            return;
        }

        // プレイヤーを移動
        MovePlayerToStart(startPos);
    }

    /// <summary>
    /// エリア2 → エリア1 に戻る
    /// </summary>
    public void ReturnToArea1() {
        // エリア2を削除して負荷軽減
        if (currentArea2 != null) {
            Destroy(currentArea2);
            currentArea2 = null;
        }

        if (area1SceneObject == null) {
            Debug.LogError("シーン上のエリア1が設定されていません");
            return;
        }

        // エリア1の StartPos を取得
        Transform startPos = area1SceneObject.transform.Find("StartPos");
        if (startPos == null) {
            Debug.LogError("シーン上のエリア1に StartPos が見つかりません");
            return;
        }

        // プレイヤーを StartPos に移動＋向き合わせ
        MovePlayerToStart(startPos);
    }

    /// <summary>
    /// プレイヤーを指定 StartPos に移動し、進行方向を StartPos.forward に合わせる
    /// </summary>
    private void MovePlayerToStart(Transform startPos) {
        if (player == null) {
            Debug.LogError("プレイヤーが設定されていません");
            return;
        }

        // 位置を StartPos に合わせる
        player.transform.position = startPos.position;

        // forward を StartPos に合わせて水平回転
        Vector3 forward = startPos.forward;
        forward.y = 0f; // 上下回転は無視
        if (forward.sqrMagnitude > 0.001f)
            player.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);

        // Rigidbody がある場合は速度をリセット
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null) {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}