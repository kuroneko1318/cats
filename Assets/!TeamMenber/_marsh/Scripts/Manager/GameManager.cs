using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SystemObject<GameManager> {

    public GameObject clearUI;
    public GameObject deadUI;
    public GameObject questClearUI;
    private GameObject player;
    // どのNastyを倒したかを管理
    private Dictionary<EnemyType, bool> defeatedNasty = new Dictionary<EnemyType, bool>();

    public override void Initialize() {
        defeatedNasty.Clear();
        defeatedNasty[EnemyType.Nasty] = false;
        defeatedNasty[EnemyType.NastyDesert] = false;
        defeatedNasty[EnemyType.NastyVolcano] = false;
        player = GameObject.Find("Player");
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

    public bool AllNastyDefeated() {
        foreach (var kv in defeatedNasty) {
            if (!kv.Value) return false;
        }
        return true;
    }

    private void OnClear() {
        Debug.Log("ゲームクリア！！");
        if (clearUI != null) Instantiate(clearUI);
        player.transform.position = new Vector3(0,0.55f,0);
        TalkNPC.Instance.ChangeAnimation();
        AudioManager.Instance.PlaySE("Clear");
    }

    public void OnDead() {
        if (deadUI != null) Instantiate(deadUI);
    }

    public void QuestClear() {
        if (questClearUI != null) Instantiate(questClearUI);
        
    }
}
