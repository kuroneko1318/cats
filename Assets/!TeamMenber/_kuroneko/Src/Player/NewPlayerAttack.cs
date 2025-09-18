using System.Transactions;
using UnityEngine;

/// <summary>
/// プレイヤー攻撃クラス（3段コンボ対応＋攻撃ごとCollider表示制御、アニメイベント管理）
/// </summary>
public class NewPlayerAttack {
    private Animator anim;
    private int attackIndex = 0;             // 現在の攻撃段
    private bool isAttacking = false;        // 攻撃中フラグ
    private bool queuedNextAttack = false;   // 次段予約

    private GameObject attackCollider1;
    private GameObject attackCollider2;
    private GameObject attackCollider3;

    public int damage;                        // 攻撃力

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

    /// <summary>
    /// 攻撃入力（ボタン押下時に呼ぶ）
    /// </summary>
    public void Attack() {
        if (!isAttacking) {
            // 攻撃未中なら1段目を開始
            StartAttack();
        }
        else if (!queuedNextAttack && attackIndex < 3) {
            // 攻撃中かつ最大3段未満なら次段予約
            queuedNextAttack = true;
        }
    }

    /// <summary>
    /// 攻撃開始処理（AttackStartイベントで呼ぶ）
    /// </summary>
    private void StartAttack() {
        isAttacking = true;
        attackIndex = Mathf.Clamp(attackIndex + 1, 1, 3);

        anim.SetInteger("AttackIndex", attackIndex);
        anim.SetBool("Attack", true);

        queuedNextAttack = false;
    }

    /// <summary>
    /// 毎フレーム更新（Update）: 入力予約の管理のみ
    /// </summary>
    public void Update() {
        // 攻撃中で予約があれば、次段はイベントで開始するのでここでは触らない
    }

    // =========================================
    // アニメーションイベント用関数
    // =========================================

    /// <summary>
    /// 各段攻撃開始時にイベントから呼ぶ
    /// </summary>
    public void AttackStart() {
        // 現在段のColliderのみON
        attackCollider1.SetActive(attackIndex == 1);
        attackCollider2.SetActive(attackIndex == 2);
        attackCollider3.SetActive(attackIndex == 3);

        // ダメージ設定
        if (attackIndex == 1) {
            attackCollider1.GetComponent<HitEnemy>().SetDamage(damage);
            AudioManager.Instance.PlaySE("FirstAttack");
        }
        if (attackIndex == 2) {
            attackCollider2.GetComponent<HitEnemy>().SetDamage(damage);
            AudioManager.Instance.PlaySE("SecondAttack");
        }
        if (attackIndex == 3) {
            attackCollider3.GetComponent<HitEnemy>().SetDamage(damage);
            AudioManager.Instance.PlaySE("ThirdAttack");
        }
    }

    /// <summary>
    /// 各段攻撃終了時にイベントから呼ぶ
    /// </summary>
    public void AttackEnd() {
        // Colliderを全てOFF
        attackCollider1.SetActive(false);
        attackCollider2.SetActive(false);
        attackCollider3.SetActive(false);

        if (queuedNextAttack) {
            // 次段予約がある場合は次段を開始
            queuedNextAttack = false;
            StartAttack();
        }
        else {
            // 予約なしなら攻撃終了
            anim.SetBool("Attack", false);
            isAttacking = false;
            attackIndex = 0;
        }
    }

    /// <summary>
    /// 攻撃リセット
    /// </summary>
    public void ResetAttack() {
        anim.SetBool("Attack", false);
        isAttacking = false;
        attackIndex = 0;
        queuedNextAttack = false;

        attackCollider1.SetActive(false);
        attackCollider2.SetActive(false);
        attackCollider3.SetActive(false);
    }

    public void SetPower(float power) {
        damage = (int)power;
    }
}
