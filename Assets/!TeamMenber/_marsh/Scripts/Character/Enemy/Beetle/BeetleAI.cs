using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BeeteAI : EnemyBase {
    [Header("攻撃ステータス")]
    public int stabPower = 5;
    public int kickPower = 8;
    public int scratchPower = 6;
    public int triplePower = 12;
    private bool isAttacking = false;

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

                    if (!isAttacking)
                        StartCoroutine(AttackRoutine());
                }
                break;
        }
    }

    private IEnumerator AttackRoutine() {
        isAttacking = true;

        int rand = Random.Range(0, 4);
        switch (rand) {
            case 0: PerformAttack("Stab", 0.2f, 1.5f); break;
            case 1: PerformAttack("Kick", 0.3f, 1.5f); break;
            case 2: PerformAttack("Scratch", 0.25f, 1.5f); break;
            case 3: PerformAttack("Triple", 0.4f, 2.5f); break;
        }

        // 攻撃モーション分待つ（AnimationEvent で処理する場合はモーション長に合わせる）
        yield return new WaitForSeconds(2.7f); // 最大攻撃時間に合わせる

        isAttacking = false;
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
        // 目的地が無ければ新しく作る
        if (!hasPatrolDestination) {
            patrolDestination = RandomNavSphere(transform.position, patrolRadius);
            agent.SetDestination(patrolDestination);
            hasPatrolDestination = true;
        }

        // 経路計算中は待機
        if (agent.pathPending) return;

        // 到着判定（stoppingDistance を考慮）
        if (agent.remainingDistance <= agent.stoppingDistance) {
            hasPatrolDestination = false;  // 次の目的地用フラグリセット
            ChangeState(EnemyState.LookAround); // 到着した瞬間に状態変更
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
