using System.Collections;
using UnityEngine;

public class Beetle : EnemyBase {
    [SerializeField] private GameObject attackHitbox;
    [SerializeField] private float attackDuration = 0.2f;
    [SerializeField] private float attackCooldown = 1.5f;
    private float attackTimer;

    private void Start() {
        //InitializeStats(50, 10, 5, 2.0f); // カブトムシのステータス
        animator = GetComponent<Animator>();
        attackTimer = attackCooldown;
        InitializeStats(hp, attack, defence, moveSpeed);
    }

    private void Update() {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f) {
            Attack();
            attackTimer = attackCooldown;
        }
    }

    public override void Attack() {
        StartCoroutine(PerformSlashAttack());
    }

    private IEnumerator PerformSlashAttack() {
        animator.SetBool("Attack", true); // アニメーション切り替え
        yield return new WaitForSeconds(0.1f); // アニメーションに合わせてタイミング調整

        attackHitbox.SetActive(true);
        yield return new WaitForSeconds(attackDuration);
        attackHitbox.SetActive(false);
        animator.SetBool("Attack", false);
    }

    public override void Move() {
        // シンプルな前進移動（必要に応じてAI追加）
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    public override void HealHp() {
        hp = Mathf.Min(hp + 10, maxHp);
    }

    public override void Dead() {
        animator.SetBool("Dead", false);
        Destroy(gameObject, 1f);
    }
}
