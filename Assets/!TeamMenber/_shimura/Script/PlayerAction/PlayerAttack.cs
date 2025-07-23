
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour {
    // プレイヤーの入力管理
    PlayerInput input;
    InputAction attackAction;
    [SerializeField]
    Animator anim;
    // 攻撃判定用のコライダー（Trigger）
    public Collider attackCollider;

    // コンボ管理
    public int comboStep = 0;                  // 現在のコンボ段階（1〜3）
    private float comboResetTime = 1.0f;       // コンボ入力猶予時間
    private float comboTimer = 0f;             // コンボ猶予タイマー

    // クールタイム管理
    private bool isAttackCooldown = false;     // クールタイム中かどうか
    private float attackCooldownDuration = 0.5f; // クールタイムの長さ
    private float attackCooldownTimer = 0f;    // クールタイム残り時間

    void Start() {
        // PlayerInput から Attack アクションを取得
        input = GetComponent<PlayerInput>();
        attackAction = input.actions["Attack"];
        if (attackAction == null) {
            Debug.LogError("Attack アクションが見つかりません");
        }
    }

    void Update() {
        LowAttack();             // 攻撃処理
        HandleAttackCooldown();  // クールタイム処理
        CheckAttackAnimationEnd();
    }

    // 通常攻撃（3連コンボ）
    public void LowAttack() {
        if (isAttackCooldown) return; // クールタイム中は攻撃不可

        // 攻撃ボタンが押された瞬間
        if (attackAction.WasPressedThisFrame()) {
            comboStep++; // コンボ段階を進める
            if (comboStep == 1) {
                anim.SetBool("Attack1",true);
                
            }
            if (comboStep == 2) {
                anim.SetBool("Attack2", true);
               
            }
            if (comboStep > 3) comboStep = 1; // 最大3段階まで

            comboTimer = comboResetTime; // コンボ猶予タイマーをリセット

            // アニメーション再生（例：Attack1, Attack2, Attack3）
            // animator.SetTrigger("Attack" + comboStep);

            // 攻撃判定をサイズに応じて有効化
            StartCoroutine(EnableAttackCollider(comboStep));

            // 3段目まで出し切ったらクールタイム開始
            if (comboStep == 3) {
                StartAttackCooldown();
                anim.SetBool("Attack3", true);
                

            }
        }






        // コンボ猶予時間の管理
        if (comboStep > 0) {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f) {
                comboStep = 0;           // コンボリセット
                StartAttackCooldown();  // コンボ中断時もクールタイム開始
            }
        }
    }

    public void StrongAttack() {
        if (isAttackCooldown) return; // クールタイム中は攻撃不可

        // 攻撃ボタンが押された瞬間
        if (attackAction.WasPressedThisFrame()) {
            comboStep++; // コンボ段階を進める

            if (comboStep > 2) comboStep = 1; // 最大3段階まで

            comboTimer = comboResetTime; // コンボ猶予タイマーをリセット

            // アニメーション再生（例：Attack1, Attack2, Attack3）
            // animator.SetTrigger("Attack" + comboStep);

            // 攻撃判定をサイズに応じて有効化
            StartCoroutine(EnableAttackCollider(comboStep));

            // 3段目まで出し切ったらクールタイム開始
            if (comboStep == 2) {
                StartAttackCooldown();
            }
        }






        // コンボ猶予時間の管理
        if (comboStep > 0) {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f) {
                comboStep = 0;           // コンボリセット
                StartAttackCooldown();  // コンボ中断時もクールタイム開始
            }
        }
    }

    // クールタイム開始処理
    private void StartAttackCooldown() {
        isAttackCooldown = true;
        attackCooldownTimer = attackCooldownDuration;
    }

    // クールタイムの時間管理
    private void HandleAttackCooldown() {
        if (isAttackCooldown) {
            attackCooldownTimer -= Time.deltaTime;
            if (attackCooldownTimer <= 0f) {
                isAttackCooldown = false;
            }
        }
    }

    // 攻撃判定のサイズ変更と一時的な有効化
    private IEnumerator EnableAttackCollider(int step) {
        // コンボ段階に応じて攻撃判定のサイズを変更
        switch (step) {
            case 1:
                attackCollider.transform.localScale = Vector3.one * 1.0f;
                break;
            case 2:
                attackCollider.transform.localScale = Vector3.one * 1.5f;
                break;
            case 3:
                attackCollider.transform.localScale = Vector3.one * 2.0f;
                break;
        }

        attackCollider.enabled = true;               // 攻撃判定を有効化
        yield return new WaitForSeconds(0.2f);       // 判定持続時間
        attackCollider.enabled = false;              // 攻撃判定を無効化
    }



    void CheckAttackAnimationEnd() {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Attack1") && stateInfo.normalizedTime >= 1.0f) {
            anim.SetBool("Attack1", false);
        }
        if (stateInfo.IsName("Attack2") && stateInfo.normalizedTime >= 1.0f) {
            anim.SetBool("Attack2", false);
        }
        if (stateInfo.IsName("Attack3") && stateInfo.normalizedTime >= 1.0f) {
            anim.SetBool("Attack3", false);
        }
    }

}
