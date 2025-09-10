using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour {
    [System.Serializable]
    public class EnemyPool {
        public GameObject enemyPrefab;
        [Range(0f, 1f)] public float spawnWeight = 1f; // èoåªî‰èd
        public int poolSize = 5;
        [HideInInspector] public List<GameObject> pool;
    }

    public List<EnemyPool> enemyPools = new List<EnemyPool>();
    public float spawnInterval = 2f;
    public int maxAlive = 15;

    private float timer;
    private int aliveCount;

    private void Start() {
        foreach (var pool in enemyPools) {
            pool.pool = new List<GameObject>();
            GameObject parent = new GameObject(pool.enemyPrefab.name + "_Pool");
            parent.transform.SetParent(transform);

            for (int i = 0; i < pool.poolSize; i++) {
                GameObject obj = Instantiate(pool.enemyPrefab, parent.transform);
                obj.SetActive(false);
                pool.pool.Add(obj);
            }
        }
    }

    private void Update() {
        timer += Time.deltaTime;
        if (timer >= spawnInterval) {
            timer = 0f;
            TrySpawnEnemy();
        }
    }

    private void TrySpawnEnemy() {
        if (aliveCount >= maxAlive) return;

        EnemyPool selectedPool = GetRandomPoolByWeight();
        if (selectedPool == null) return;

        GameObject enemyObj = GetInactiveEnemy(selectedPool);
        if (enemyObj == null) return;

        Vector3 spawnPos = GetRandomPointInSphere();
        enemyObj.transform.position = spawnPos;
        enemyObj.SetActive(true);

        EnemyBase eb = enemyObj.GetComponent<EnemyBase>();
        if (eb != null) {
            eb.enabled = true;
            eb.OnEnemyDead -= HandleEnemyDead;
            eb.OnEnemyDead += HandleEnemyDead;
            eb.OnSpawned();
        }

        aliveCount++;
    }

    private EnemyPool GetRandomPoolByWeight() {
        float totalWeight = 0f;
        foreach (var pool in enemyPools) totalWeight += pool.spawnWeight;

        float randomValue = Random.Range(0f, totalWeight);
        float sum = 0f;

        foreach (var pool in enemyPools) {
            sum += pool.spawnWeight;
            if (randomValue <= sum) return pool;
        }

        return null;
    }

    private GameObject GetInactiveEnemy(EnemyPool pool) {
        foreach (var obj in pool.pool) {
            if (!obj.activeInHierarchy) return obj;
        }
        return null;
    }

    private Vector3 GetRandomPointInSphere() {
        SphereCollider col = GetComponent<SphereCollider>();
        if (col == null) return transform.position;

        Vector3 randomOffset = Random.insideUnitSphere * col.radius;
        randomOffset.y = 0f;
        Vector3 spawnPos = transform.position + randomOffset;

        UnityEngine.AI.NavMeshHit hit;
        if (UnityEngine.AI.NavMesh.SamplePosition(spawnPos, out hit, 2f, UnityEngine.AI.NavMesh.AllAreas))
            return hit.position;

        return transform.position;
    }

    private void HandleEnemyDead(EnemyBase enemy) {
        aliveCount--;
    }
}
