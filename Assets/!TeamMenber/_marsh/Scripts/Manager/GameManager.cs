using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class GameManager : SystemObject<GameManager> {

    public GameObject clearUI;
    public GameObject deadUI;
    public GameObject questClearUI;
    private GameObject player;
    private PlayerBase playerBase;

    // どのNastyを倒したかを管理
    private Dictionary<EnemyType, bool> defeatedNasty = new Dictionary<EnemyType, bool>();

    public int laps = 1;

    // ステータス増加率
    [Header("Enemy Status Increase")]
    public float hpIncreaseRate = 1.2f;   // 20%ずつ増加
    public float attackIncreaseRate = 1.1f; // 10%ずつ増加

    public override void Initialize() {
        defeatedNasty.Clear();
        defeatedNasty[EnemyType.Nasty] = false;
        defeatedNasty[EnemyType.NastyDesert] = false;
        defeatedNasty[EnemyType.NastyVolcano] = false;
        player = GameObject.Find("Player");
        playerBase = player.GetComponent<PlayerBase>();
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
        laps++;

        // SE
        AudioManager.Instance.PlaySE("Clear");

        //プレイヤーの位置リセット
        playerBase.DeathAnimationEnd();

        // ラップ加算
        laps++;

        //  NPCのアニメーションを変更
        TalkNpcChangeAnimation();

        // 敵ステータスを増加
        IncreaseEnemyStatus();

        // 敵の倒したフラグリセット
        ResetDefeatedNasty();
    }

    private void IncreaseEnemyStatus() {
        // すべてのEnemyBaseを取得
        EnemyBase[] enemies = FindObjectsOfType<EnemyBase>();
        foreach (var enemy in enemies) {
            enemy.maxHp = Mathf.RoundToInt(enemy.maxHp * hpIncreaseRate);
            enemy.attack = Mathf.RoundToInt(enemy.attack * attackIncreaseRate);

            // 現在HPも最大HPに合わせる
            enemy.hp = enemy.maxHp;
        }
        Debug.Log($"ラップ {laps} : 敵ステータスを増加しました");
    }

    private void TalkNpcChangeAnimation() {
        TalkNPC[] npcs = FindObjectsOfType<TalkNPC>();
        foreach(var npc in npcs) {
            npc.ChangeAnimation();
        }
    }

    private void ResetDefeatedNasty() {
        var keys = new List<EnemyType>(defeatedNasty.Keys);
        foreach (var key in keys) {
            defeatedNasty[key] = false;
        }
        Initialize();
    }

    public void OnDead() {
        if (deadUI != null) Instantiate(deadUI);
    }

    public void QuestClear() {
        if (questClearUI != null) Instantiate(questClearUI);
    }
}
