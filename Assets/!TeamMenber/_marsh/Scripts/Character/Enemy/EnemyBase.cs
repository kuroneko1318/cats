using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState {
    Idle,       // 待機
    Patrol,     // 巡回
    LookAround, // 観察（Idleから遷移）
    Chase,      // プレイヤー追跡
    Combat,     // 戦闘
    Dead
}

public class EnemyBase : MonoBehaviour {
    [Header("共通ステータス")]
    public int hp;
    public int maxHp;
    public int attack;
    public int defence;
    public float moveSpeed = 3.5f;
    public float detectionRange = 10f;
    public float combatRange = 3f;
    private bool isCritical = false;
    public float attackMultiplier = 1f; // 敵ごとに設定

    private int damage;

    [Header("参照")]
    protected Animator animator;
    protected NavMeshAgent agent;
    protected AttackHitbox hitbox;
    protected Transform player;
    [SerializeField]protected DamagePopupController damagePopupController;

    protected EnemyState state = EnemyState.Idle;

    protected virtual void Start() {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        hitbox = GetComponentInChildren<AttackHitbox>(true);
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (agent != null) agent.speed = moveSpeed;
        if (hitbox != null) hitbox.gameObject.SetActive(false);
    }

    protected virtual void Update() {
        if (state == EnemyState.Dead) return;

        float dist = player ? Vector3.Distance(transform.position, player.position) : Mathf.Infinity;

        switch (state) {
            case EnemyState.Idle:
            case EnemyState.Patrol:
            case EnemyState.LookAround:
                if (dist < detectionRange) ChangeState(EnemyState.Chase);
                break;

            case EnemyState.Chase:
                if (dist <= combatRange) ChangeState(EnemyState.Combat);
                else if (dist > detectionRange * 1.5f) ChangeState(EnemyState.Patrol);
                break;

            case EnemyState.Combat:
                if (dist > combatRange + 1f) ChangeState(EnemyState.Chase);
                break;
        }
    }

    protected void ChangeState(EnemyState newState) {
        if (state == newState) return;
        state = newState;
        OnStateChanged(newState);
    }

    protected virtual void OnStateChanged(EnemyState newState) {
        switch (newState) {
            case EnemyState.Idle:
                agent.isStopped = true;
                animator.SetBool("Idle", true);
                animator.SetBool("Walk", false);
                break;
            case EnemyState.Patrol:
                agent.isStopped = false;
                animator.SetBool("Walk", true);
                animator.SetBool("Idle", false);
                break;
            case EnemyState.LookAround:
                agent.isStopped = true;
                animator.SetTrigger("LookAround");
                break;
            case EnemyState.Chase:
                agent.isStopped = false;
                animator.SetBool("Run", true);
                animator.SetBool("Walk", false);
                break;
            case EnemyState.Combat:
                agent.isStopped = true;
                animator.SetBool("Idle", true);
                animator.SetBool("Walk", false);
                animator.SetBool("Run", false);
                break;
        }
    }

    // 攻撃共通処理
    protected void PerformAttack(string triggerName, float preDelay, float activeTime) {
        animator.SetTrigger(triggerName);
        StartCoroutine(AttackCoroutine(preDelay, activeTime));
    }

    private IEnumerator AttackCoroutine(float preDelay, float activeTime) {
        yield return new WaitForSeconds(preDelay);
        //EnableHitbox(power);
        yield return new WaitForSeconds(activeTime);
        //DisableHitbox();
    }

    private void EnableHitbox(int power) {
        int finalPower = Mathf.RoundToInt(power * attackMultiplier);
        hitbox.damage = finalPower;
        hitbox.gameObject.SetActive(true);
    }

    private void DisableHitbox() {
        hitbox.gameObject.SetActive(false);
    }

    public virtual void TakeDamage(int attack, float motionMultiplier = 1, float criticalChance = 0, float criticalMultiplier = 2,
                                   int elementalValue = 0, float staggerValue = 0) {
        isCritical = Random.value < criticalChance; // 20%でクリティカル
        if (isCritical) {
            damage = Mathf.RoundToInt
                ((Mathf.Pow(attack, 2) / attack + defence) * motionMultiplier * Random.Range(0.90f, 1.1f) * criticalMultiplier);
            hp -= damage;
        }
        else {
            damage = Mathf.RoundToInt
                ((Mathf.Pow(attack, 2) / attack + defence) * motionMultiplier * Random.Range(0.90f, 1.1f));
            hp -= damage;
        }
        damagePopupController.AddDamage(damage);
        animator.SetTrigger("Hit"); // アニメーション切り替え

        if (hp <= 0) Dead();
    }

    public virtual void Dead() {
        ChangeState(EnemyState.Dead);
        animator.SetTrigger("Dead");
        agent.isStopped = true;
        this.enabled = false;
    }
}
