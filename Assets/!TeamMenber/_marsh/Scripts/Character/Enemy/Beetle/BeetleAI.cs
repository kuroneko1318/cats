using System.Collections;
using UnityEngine;


public class BeetleAI : EnemyBase {

    [SerializeField] private GameObject attackHitbox;
    [SerializeField] private float attackDuration = 0.2f;
    private bool isAttacking = false;

    public EnemyState currentState = EnemyState.Idle;

    [Header("AI Settings")]
    public Transform player;
    public float viewRange = 10f;
    public float viewAngle = 60f;
    public float combatRange = 2f;

    private float idleTimer;
    private Vector3 randomDirection;

    private void Start() {
        InitializeStats(80, 15, 5, 2.5f);
        idleTimer = Random.Range(2f, 4f);
    }

    private void Update() {
        switch (currentState) {
            case EnemyState.Idle: HandleIdle(); break;
            case EnemyState.Alert: HandleAlert(); break;
            case EnemyState.Chase: HandleChase(); break;
            case EnemyState.Combat: HandleCombat(); break;
            case EnemyState.Observe: HandleObserve(); break;
            case EnemyState.Dead: Dead(); break;
        }
    }

    private void HandleIdle() {
        idleTimer -= Time.deltaTime;
        if (idleTimer <= 0f) {
            if (Random.value < 0.3f) {
                animator.SetTrigger("LookAround");
                idleTimer = Random.Range(2f, 4f);
            }
            else {
                randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
                idleTimer = Random.Range(3f, 6f);
            }
        }

        transform.position += randomDirection * moveSpeed * Time.deltaTime;

        if (CanSeePlayer()) {
            currentState = EnemyState.Alert;
        }
    }

    private void HandleAlert() {
        animator.SetTrigger("Alert");
        currentState = EnemyState.Chase;
    }

    private void HandleChase() {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        Vector3 targetPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(targetPos);

        if (Vector3.Distance(transform.position, player.position) <= combatRange) {
            currentState = EnemyState.Combat;
        }
        else if (Vector3.Distance(transform.position, player.position) <= combatRange + 1f) {
            currentState = EnemyState.Observe;
        }

    }

    private void HandleCombat() {
        Vector3 targetPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(targetPos);

        float distance = Vector3.Distance(transform.position, player.position);

        //// ---- 距離調整 ----
        //if (distance > combatRange + 0.5f) {
        //    // 離れすぎた → 前進
        //    Vector3 direction = (player.position - transform.position).normalized;
        //    transform.position += direction * moveSpeed * Time.deltaTime;
        //}
        //else if (distance < combatRange - 0.5f) {
        //    // 近すぎた → 後退
        //    Vector3 backStep = -transform.forward;
        //    transform.position += backStep * (moveSpeed * 0.7f) * Time.deltaTime;
        //}
        //else {
            // ---- 近距離に入ったら行動を選択 ----
            if (!isAttacking) {
                float rand = Random.value;
                if (rand < 0.3f) {
                    // 攻撃
                    animator.SetTrigger("Scratch");
                    Attack();
                }
                else if (rand < 0.6f) {
                    // 横移動（ステップ）
                    Vector3 sideStep = transform.right * (Random.value < 0.5f ? 1 : -1);
                    transform.position += sideStep * moveSpeed * Time.deltaTime;
                }
                else {
                    // 他の攻撃
                    animator.SetTrigger("Stab");
                    Attack();
                }
            }
        //}
    }


    private void HandleObserve() {
        animator.SetBool("Walk", true);
        Vector3 targetPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(targetPos);

        float rand = Random.value;
        if (rand < 0.5f) {
            // 立ち止まる
            animator.SetBool("Walk", false);
        }
        else {
            // 左右に移動
            Vector3 sideStep = transform.right * (Random.value < 0.5f ? 1 : -1);
            transform.position += sideStep * moveSpeed * Time.deltaTime;
        }

        // 攻撃可能距離ならCombatへ
        if (Vector3.Distance(transform.position, player.position) <= combatRange) {
            currentState = EnemyState.Combat;
        }
        // 戦闘距離外れたら再びChase
        else if (Vector3.Distance(transform.position, player.position) > combatRange + 1f) {
            currentState = EnemyState.Chase;
        }
    }


    private bool CanSeePlayer() {
        Vector3 directionToPlayer = player.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        return directionToPlayer.magnitude <= viewRange && angle <= viewAngle;
    }

    public override void Attack() {
        if (isAttacking) return; // 攻撃中は無視
        StartCoroutine(PerformAttack());
    }

    private IEnumerator PerformAttack() {
        isAttacking = true;

        yield return new WaitForSeconds(1.5f); // アニメーションに合わせて判定タイミング調整

        attackHitbox.SetActive(true); // 攻撃判定ON
        yield return new WaitForSeconds(attackDuration);
        attackHitbox.SetActive(false); // 攻撃判定OFF

        isAttacking = false;
    }


    public override void Dead() {
        animator.SetTrigger("Dead");
        Destroy(gameObject, 1f);
    }

    public override void HealHp() {
        hp = Mathf.Min(hp + 10, maxHp);
    }

    public override void Move() {
        // 状態によって自動制御されるので空でもOK
    }

    //攻撃判定のON、OFF
    public void EnableHitbox() {
        attackHitbox.SetActive(true);
        Invoke(nameof(DisableHitbox), attackDuration);
    }

    private void DisableHitbox() {
        attackHitbox.SetActive(false);
    }

}
