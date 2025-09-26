using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SystemObject<GameManager> {

    public GameObject clearUI;

    // どのNastyを倒したかを管理
    private Dictionary<EnemyType, bool> defeatedNasty = new Dictionary<EnemyType, bool>();

    public override void Initialize() {
        defeatedNasty.Clear();
        defeatedNasty[EnemyType.Nasty] = false;
        defeatedNasty[EnemyType.NastyDesert] = false;
        defeatedNasty[EnemyType.NastyVolcano] = false;
    }

    // EnemyBaseから呼び出す
    public void ReportEnemyDefeat(EnemyType type) {
        if (defeatedNasty.ContainsKey(type) && !defeatedNasty[type]) {
            defeatedNasty[type] = true;
            Debug.Log($"{type} を倒した！");

            // 全て倒したか確認
            if (AllNastyDefeated()) {
                OnClear();
            }
        }
    }

    private bool AllNastyDefeated() {
        foreach (var kv in defeatedNasty) {
            if (!kv.Value) return false;
        }
        return true;
    }

    private void OnClear() {
        Debug.Log("ゲームクリア！！");
        if (clearUI != null) Instantiate(clearUI);
    }
}
