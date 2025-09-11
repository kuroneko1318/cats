using UnityEngine;

public class NewPlayerAttack {
    private Animator anim;
    private int attackIndex = 0;
    private bool isAttacking = false;
    private bool queuedNextAttack = false;

    private float comboInputStart = 0.3f;
    private float comboInputEnd = 0.75f;

    private GameObject attackCollider1;
    private GameObject attackCollider2;
    private GameObject attackCollider3;

    public NewPlayerAttack(Animator animator, GameObject col1, GameObject col2, GameObject col3) {
        anim = animator;
        attackCollider1 = col1;
        attackCollider2 = col2;
        attackCollider3 = col3;

        attackCollider1.SetActive(false);
        attackCollider2.SetActive(false);
        attackCollider3.SetActive(false);
    }

    public bool IsAttacking() => isAttacking;

    public void Attack() {
        if (!isAttacking) {
            StartAttack();
        }
        else if (!queuedNextAttack && attackIndex < 3) {
            queuedNextAttack = true;
        }
    }

    private void StartAttack() {
        isAttacking = true;
        attackIndex = Mathf.Clamp(attackIndex + 1, 1, 3);

        anim.SetInteger("AttackIndex", attackIndex);
        anim.SetBool("Attack", true);

        queuedNextAttack = false;
        // Collider はイベントで制御
    }

    public void Update() {
        if (!isAttacking) return;

        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);

        // 次段予約があれば条件内で開始
        if (queuedNextAttack && state.normalizedTime >= comboInputStart && state.normalizedTime <= comboInputEnd) {
            queuedNextAttack = false;
            StartAttack();
        }

        // 攻撃終了判定（アニメ終了確認のみ、Collider はイベントで制御）
        if (state.IsTag("Attack") && state.normalizedTime >= 0.95f) {
            anim.SetBool("Attack", false);
            isAttacking = false;
            attackIndex = 0;
            queuedNextAttack = false;
        }
    }

    // =========================================
    // Animatorイベント用
    // =========================================
    public void AttackStart() {
        // 現在の攻撃段のColliderのみON
        attackCollider1.SetActive(attackIndex == 1);
        attackCollider2.SetActive(attackIndex == 2);
        attackCollider3.SetActive(attackIndex == 3);
    }

    public void AttackEnd() {
        // 攻撃終了で全てOFF
        attackCollider1.SetActive(false);
        attackCollider2.SetActive(false);
        attackCollider3.SetActive(false);

        //anim.SetBool("Attack", false);
        //isAttacking = false;
        //queuedNextAttack = false;
        //attackIndex = 0;
    }

    public void ResetAttack() {
        anim.SetBool("Attack", false);
        isAttacking = false;
        attackIndex = 0;
        queuedNextAttack = false;
    }

}
