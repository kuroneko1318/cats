using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Nasty : EnemyBase {
    [Header("巡回関連")]
    public float patrolRadius = 10f;   // 巡回範囲
    public float idleTime = 2f;

    private float idleTimer = 0f;
    private Vector3 patrolDestination;
    private bool hasPatrolDestination = false;
    private bool isAttacking = false;

    protected override void Start() {
        base.Start();
        InitializeNavMeshAgent();
        ChangeState(EnemyState.Patrol);
    }

    protected override void Update() {
        base.Update();

        switch (state) {
            case EnemyState.Idle:
                IdleUpdate();
                CheckPlayerDetection();
                break;

            case EnemyState.Patrol:
                PatrolUpdate();
                CheckPlayerDetection();
                break;

            case EnemyState.Chase:
                ChaseUpdate();
                break;

            case EnemyState.Combat:
                CombatUpdate();
                break;
        }
    }

    public override void OnSpawned() {
        base.OnSpawned();
        idleTimer = 0f;
        hasPatrolDestination = false;
        isAttacking = false;
        InitializeNavMeshAgent();
        ChangeState(EnemyState.Patrol);
    }

    private void InitializeNavMeshAgent() {
        if (agent == null) return;

        agent.enabled = false;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 10f, NavMesh.AllAreas)) {
            transform.position = hit.position;
            agent.Warp(hit.position);
            agent.enabled = true;
            if (agent.isOnNavMesh) {
                agent.ResetPath();
                agent.isStopped = false;
            }
            else {
                Debug.LogWarning("NavMeshAgent は NavMesh 上にありません。");
            }
        }
        else {
            Debug.LogWarning("NavMesh 上に配置できません: " + transform.position);
            agent.enabled = true;
        }
    }

    private void PatrolUpdate() {
        if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh) return;

        if (!hasPatrolDestination) {
            patrolDestination = GetSafePatrolPoint();
            agent.SetDestination(patrolDestination);
            hasPatrolDestination = true;
        }

        if (agent.pathPending) return;

        if (agent.remainingDistance <= agent.stoppingDistance) {
            hasPatrolDestination = false;
            ChangeState(EnemyState.Idle);
        }
    }

    private Vector3 GetSafePatrolPoint() {
        for (int i = 0; i < 10; i++) {
            Vector3 randDir = Random.insideUnitSphere * patrolRadius + transform.position;
            randDir.y = transform.position.y;

            NavMeshHit navHit;
            if (NavMesh.SamplePosition(randDir, out navHit, patrolRadius, NavMesh.AllAreas)) {
                // NavMesh 上の位置が取得できたらそれを返す
                return navHit.position;
            }
        }
        // 安全な位置が見つからなければ現在位置
        return transform.position;
    }

    private void IdleUpdate() {
        idleTimer += Time.deltaTime;
        if (idleTimer >= idleTime) {
            idleTimer = 0f;
            ChangeState(EnemyState.Patrol);
        }
    }

    private void CheckPlayerDetection() {
        if (player == null) return;
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist < detectionRange) {
            ChangeState(EnemyState.Chase);
        }
    }

    private void ChaseUpdate() {
        if (player == null) return;
        if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh) {
            agent.SetDestination(player.position);
        }

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= combatRange) {
            ChangeState(EnemyState.Combat);
        }
        else if (dist > detectionRange * 1.5f) {
            ChangeState(EnemyState.Patrol);
        }
    }

    private void CombatUpdate() {
        if (player == null) return;

        transform.LookAt(player);

        if (!isAttacking) {
            StartCoroutine(AttackRoutine());
        }

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > combatRange + 1f) {
            ChangeState(EnemyState.Chase);
        }
    }

    private IEnumerator AttackRoutine() {
        isAttacking = true;

        int rand = Random.Range(0, 3);
        switch (rand) {
            case 0: PerformAttack("Single", 0.2f, 1.5f); break;
            case 1: PerformAttack("Double", 0.3f, 1.5f); break;
            case 2: PerformAttack("Charge", 0.25f, 1.5f); break;
        }

        yield return new WaitForSeconds(1.5f);
        isAttacking = false;
    }

    public override void Dead() {
        base.Dead();
        if (agent != null) agent.isStopped = true;
    }

    protected override void OnStateChanged(EnemyState newState) {
        if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh) return;

        switch (newState) {
            case EnemyState.Idle:
                agent.isStopped = true;
                animator.SetBool("Walk", false);
                break;
            case EnemyState.Combat:
                agent.isStopped = true;
                animator.SetBool("Walk", false);
                break;
            case EnemyState.Dead:
                agent.isStopped = true;
                animator.SetBool("Walk", false);
                break;

            case EnemyState.Patrol:
                agent.isStopped = false;
                animator.SetBool("Walk", true);
                break;
            case EnemyState.Chase:
                agent.isStopped = false;
                animator.SetBool("Walk", true);
                break;
        }
    }
}
