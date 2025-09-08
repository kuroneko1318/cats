using UnityEngine;
using UnityEngine.AI;

public class BeeteAI : EnemyBase {
    [Header("攻撃ステータス")]
    public int stabPower = 5;
    public int kickPower = 8;
    public int scratchPower = 6;
    public int triplePower = 12;

    [Header("巡回関連")]
    public float patrolRadius = 5f;
    public float idleTime = 2f;
    private float idleTimer;
    private Vector3 patrolDestination;
    private bool hasPatrolDestination = false;

    protected override void Start() {
        base.Start();
        ChangeState(EnemyState.Patrol);
    }

    protected override void Update() {
        base.Update();

        switch (state) {
            case EnemyState.Patrol:
                PatrolUpdate();
                break;

            case EnemyState.LookAround:
                LookAroundUpdate();
                break;

            case EnemyState.Chase:
                if (player != null)
                    agent.SetDestination(player.position);
                break;

            case EnemyState.Combat:
                if (player != null) {
                    transform.LookAt(player);
                    if (!IsInvoking(nameof(StartAttack)))
                        Invoke(nameof(StartAttack), Random.Range(1f, 2f));
                }
                break;
        }
    }

    private void StartAttack() {
        if (state != EnemyState.Combat) return;

        int rand = Random.Range(0, 4);
        switch (rand) {
            case 0:
                PerformAttack("Stab", stabPower, 0.2f, 0.4f);
                break;
            case 1:
                PerformAttack("Kick", kickPower, 0.3f, 0.5f);
                break;
            case 2:
                PerformAttack("Scratch", scratchPower, 0.25f, 0.35f);
                break;
            case 3:
                PerformAttack("Triple", triplePower, 0.4f, 0.7f);
                break;
        }
    }

    // ランダムなNavMesh上の位置を返す
    private Vector3 RandomNavSphere(Vector3 origin, float dist) {
        Vector3 randDir = Random.insideUnitSphere * dist;
        randDir += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDir, out navHit, dist, NavMesh.AllAreas);
        return navHit.position;
    }

    private void PatrolUpdate() {
        if (!hasPatrolDestination || agent.remainingDistance < 0.5f) {
            // 新しい目的地を作る
            patrolDestination = RandomNavSphere(transform.position, patrolRadius);
            agent.SetDestination(patrolDestination);
            hasPatrolDestination = true;
        }

        // 到達したら少し待ってから次の目的地に
        if (!agent.pathPending && agent.remainingDistance < 0.5f) {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleTime) {
                idleTimer = 0;
                hasPatrolDestination = false; // 次の目的地を作る
            }
        }
    }

    private void LookAroundUpdate() {
        idleTimer += Time.deltaTime;
        if (idleTimer >= idleTime) {
            idleTimer = 0;
            ChangeState(EnemyState.Patrol);
        }
    }
}
