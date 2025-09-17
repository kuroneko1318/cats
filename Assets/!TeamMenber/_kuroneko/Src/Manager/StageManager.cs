using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour {
    [Header("エリアプレハブ（順番に登録）")]
    [SerializeField] private GameObject[] areaPrefabs; // エリア1,2,3…のPrefab

    private GameObject currentArea; // 現在アクティブなエリア
    [SerializeField] private Transform baseStartPos; // 拠点のスタート地点

    // エリアに入る処理
    public void EnterArea(int index) {
        // すでにエリアが存在するなら破棄
        if (currentArea != null) {
            Destroy(currentArea);
            currentArea = null;
        }

        // 範囲外チェック
        if (index < 0 || index >= areaPrefabs.Length) {
            Debug.LogError("エリア番号が範囲外です: " + index);
            return;
        }

        // エリア生成
        currentArea = Instantiate(areaPrefabs[index], Vector3.zero, Quaternion.identity);

        // エリア内の StartPos を探す
        Transform startPos = currentArea.transform.Find("StartPos");
        if (startPos == null) {
            Debug.LogError("エリアPrefabに StartPos が見つかりません: " + areaPrefabs[index].name);
            return;
        }

        // プレイヤーをエリアのスタート地点に移動
        MovePlayer(startPos.position);
    }

    // 拠点に戻る処理
    public void ReturnToBase() {
        // エリアを破棄
        if (currentArea != null) {
            Destroy(currentArea);
            currentArea = null;
        }

        // プレイヤーを拠点のスタート地点に移動
        if (baseStartPos != null) {
            MovePlayer(baseStartPos.position);
        }
        else {
            Debug.LogError("拠点の StartPos が設定されていません");
        }
    }

    // プレイヤーを特定の位置に移動するヘルパー
    private void MovePlayer(Vector3 pos) {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) {
            player.transform.position = pos;
        }
    }
}
