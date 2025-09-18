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
public enum EnemyType {
    Beetle,
    StagBeetle,
    Nasty,
    Cow,
    // 必要に応じて追加
}

public class EnemyBase : MonoBehaviour {
    [Header("敵の種類設定")]
    public EnemyType enemyType;

    [Header("共通ステータス")]
    public int hp;
    public int maxHp;
    public int attack;
    public int defence;
    public float moveSpeed = 3.5f;
    public float detectionRange = 10f;
    public float combatRange = 3f;
    private bool isCritical = false;
    public float attackMultiplier = 1f; // 敵ごとに設定 現状は不要。上位など追加する場合は使う
    public float deadTime = 3f; // 死亡後に待つ時間（Inspectorで調整可能）

    private int damage;

    [Header("参照")]
    protected Animator animator;
    protected NavMeshAgent agent;
    protected AttackHitbox hitbox;
    protected Transform player;
    [SerializeField] protected GameObject damagePopupPrefab;
    private DamagePopupController currentPopup;

    protected EnemyState state = EnemyState.Idle;

    //死亡時用のアクション
    public System.Action<EnemyBase> OnEnemyDead;

    protected virtual void Start() {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        hitbox = GetComponentInChildren<AttackHitbox>(true);
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (agent != null) agent.speed = moveSpeed;
        if (hitbox != null) hitbox.gameObject.SetActive(false);

        hp = maxHp;
    }

    protected virtual void OnEnable() {
        if (animator == null) animator = GetComponent<Animator>();
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (hitbox == null) hitbox = GetComponentInChildren<AttackHitbox>(true);
        if (player == null) player = GameObject.FindGameObjectWithTag("Player")?.transform;
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
                animator.SetBool("Walk", false);
                animator.SetBool("Run", false);
                break;
            case EnemyState.Patrol:
                agent.isStopped = false;
                animator.SetBool("Walk", true);
                animator.SetBool("Run", false);
                break;
            case EnemyState.LookAround:
                agent.isStopped = true;
                animator.SetTrigger("LookAround");
                animator.SetBool("Walk", false);
                animator.SetBool("Run", false);
                break;
            case EnemyState.Chase:
                agent.isStopped = false;
                animator.SetBool("Run", true);
                animator.SetBool("Walk", false);
                break;
            case EnemyState.Combat:
                agent.isStopped = true;
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
        int finalPower = Mathf.RoundToInt((power + attack) * attackMultiplier);
        hitbox.SetDamage(finalPower);
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
        if (currentPopup != null && !currentPopup.IsFadingOut) {
            currentPopup.AddDamage(damage, isCritical);
        }
        else {
            Vector3 popupPos = transform.position + Vector3.up * 2f;
            GameObject popupObj = Instantiate(damagePopupPrefab, popupPos, Quaternion.identity);
            currentPopup = popupObj.GetComponent<DamagePopupController>();

            currentPopup.AddDamage(damage, isCritical);

        }
        if (hp <= 0) Dead();

        animator.SetTrigger("Hit"); // アニメーション切り替え
    }

    public virtual void Dead() {
        if (state == EnemyState.Dead) return; // 二重呼び出し防止
        ChangeState(EnemyState.Dead);
        animator.SetTrigger("Dead");
        agent.isStopped = true;
        // クエストへ報告
        QuestManager.Instance.EnemyDefeated(enemyType);

        StartCoroutine(DeadRoutine());
    }

    private IEnumerator DeadRoutine() {
        yield return new WaitForSeconds(deadTime);

        // Spawnerへ通知
        OnEnemyDead?.Invoke(this);

        gameObject.SetActive(false);
    }

    // --- これを呼べばプールから再表示したときに完全に初期化される ---
    public virtual void OnSpawned() {
        StopAllCoroutines();

        this.enabled = true;

        // ステータス初期化
        hp = maxHp;
        isCritical = false;
        currentPopup = null;

        // State を Patrol に
        state = EnemyState.Patrol;

        // Animator 初期化
        if (animator != null) {
            animator.Rebind();
            animator.Update(0f);
            animator.ResetTrigger("Dead");
            animator.ResetTrigger("Hit");
            animator.SetBool("Walk", true);  // Patrol 動作に合わせる
            animator.SetBool("Run", false);
            animator.Play("Idle"); // Animator で待機状態に戻す場合
        }

        // NavMeshAgent 初期化
        if (agent != null) {
            agent.enabled = true;
            agent.ResetPath();
            agent.isStopped = false;
            agent.speed = moveSpeed;
        }

        // ヒットボックスOFF
        if (hitbox != null) hitbox.gameObject.SetActive(false);
    }
}