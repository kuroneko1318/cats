using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class EnemySpawnData {
    public string enemyName;          // 敵の名前
    public GameObject prefab;         // 敵プレハブ
    public int poolSize = 5;          // プールの初期サイズ
    public int maxAlive = 3;          // 同時に存在できる最大数
    [Range(0f, 1f)]
    public float spawnWeight = 1f;    // 出現率の重み（確率）
}

public class EnemySpawner : MonoBehaviour {
    [Header("生成範囲")]
    public SphereCollider spawnArea;

    [Header("敵ごとの設定")]
    public List<EnemySpawnData> enemies = new List<EnemySpawnData>();

    [Header("生成管理")]
    public Transform monsterPool;
    public float spawnInterval = 3f;  // 何秒ごとに出現判定するか
    public int maxTotalAlive = 10;    // スポナー全体での最大数

    private Dictionary<string, Queue<GameObject>> enemyPools = new Dictionary<string, Queue<GameObject>>();
    private float timer;

    void Awake() {
        // プール作成
        foreach (var data in enemies) {
            var pool = new Queue<GameObject>();

            for (int i = 0; i < data.poolSize; i++) {
                var obj = Instantiate(data.prefab, monsterPool);
                obj.name = data.enemyName;
                obj.SetActive(false);
                pool.Enqueue(obj);
            }

            enemyPools.Add(data.enemyName, pool);
        }
    }

    void Update() {
        timer += Time.deltaTime;
        if (timer >= spawnInterval) {
            TrySpawn();
            timer = 0f;
        }
    }

    private void TrySpawn() {
        // 全体の上限チェック
        int aliveCount = monsterPool.GetComponentsInChildren<Transform>()
            .Count(t => t.gameObject.activeSelf && t != monsterPool);
        if (aliveCount >= maxTotalAlive) return;

        // ランダムで敵の種類を選択
        EnemySpawnData selected = GetRandomEnemyData();
        if (selected == null) return;

        // この敵の現在数を確認
        int currentAlive = monsterPool.GetComponentsInChildren<Transform>()
            .Count(t => t.gameObject.activeSelf && t.name == selected.enemyName);
        if (currentAlive >= selected.maxAlive) return;

        // 生成
        SpawnEnemy(selected);
    }

    private EnemySpawnData GetRandomEnemyData() {
        float totalWeight = enemies.Sum(e => e.spawnWeight);
        if (totalWeight <= 0f) return null;

        float randomValue = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var e in enemies) {
            cumulative += e.spawnWeight;
            if (randomValue <= cumulative)
                return e;
        }
        return null;
    }

    private void SpawnEnemy(EnemySpawnData data) {
        var pool = enemyPools[data.enemyName];
        GameObject enemy = null;

        // 使えるやつを探す
        foreach (var obj in pool) {
            if (!obj.activeInHierarchy) {
                enemy = obj;
                break;
            }
        }

        // 全部使用中なら新規生成してプールに追加
        if (enemy == null) {
            enemy = Instantiate(data.prefab, monsterPool);
            enemy.name = data.enemyName;
            pool.Enqueue(enemy);
        }

        // 位置を決定
        Vector3 spawnPos = GetRandomPointInSphere();
        enemy.transform.position = spawnPos;
        enemy.SetActive(true);
    }

    private Vector3 GetRandomPointInSphere() {
        Vector3 randomPoint = Random.insideUnitSphere * spawnArea.radius;
        Vector3 spawnPos = spawnArea.transform.position + randomPoint;
        spawnPos.y = spawnArea.transform.position.y; // Yを固定したい場合
        return spawnPos;
    }
}
