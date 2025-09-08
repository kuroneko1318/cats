using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour {
    [Header("プレイヤー参照")]
    public GameObject player;

    [Header("エリア1（常設）")]
    public GameObject area1;

    [Header("エリア2プレハブ")]
    public GameObject area2Prefab;

    private GameObject currentStage; // 今の動的エリア（エリア2）

    void Start() {
        // 最初はエリア1を現在ステージとして設定
        currentStage = area1;
    }

    // エリア2へ移動
    public void MoveToArea2() {
        if (area2Prefab == null) {
            Debug.LogError("エリア2のプレハブが設定されていません！");
            return;
        }

        // エリア2を生成
        GameObject newStage = Instantiate(area2Prefab, new Vector3(100, 0, 0), Quaternion.identity);

        // StartPos を探す
        Transform startPos = newStage.transform.Find("StartPos");
        if (startPos == null) {
            Debug.LogError("エリア2に StartPos が見つかりません！");
            return;
        }

        // プレイヤーを移動
        player.transform.position = startPos.position;
        player.transform.rotation = startPos.rotation;

        // 現在ステージを更新
        currentStage = newStage;
    }

    // エリア1へ戻る
    public void ReturnToArea1() {
        if (area1 == null) {
            Debug.LogError("エリア1が設定されていません！");
            return;
        }

        // StartPos を探す（エリア1に配置しておく）
        Transform startPos = area1.transform.Find("StartPos");
        if (startPos == null) {
            Debug.LogError("エリア1に StartPos が見つかりません！");
            return;
        }

        // プレイヤーを移動
        player.transform.position = startPos.position;
        player.transform.rotation = startPos.rotation;

        // エリア2があれば削除
        if (currentStage != null && currentStage != area1) {
            Destroy(currentStage);
        }

        // ステージをエリア1に更新
        currentStage = area1;
    }
}
