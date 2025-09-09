
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
    public Animator animator;

    public int count;


    // コンボ管理
    public int comboStep = 0;                  // 現在のコンボ段階（1〜3）
    private float comboResetTime = 1f;       // コンボ入力猶予時間
    private float comboTimer = 0f;             // コンボ猶予タイマー

    // クールタイム管理
    private bool isAttackCooldown = false;     // クールタイム中かどうか
    [SerializeField]private float attackCooldownDuration = 0.5f; // クールタイムの長さ
    private float attackCooldownTimer = 0f;    // クールタイム残り時間



    private bool wasInIdle = false;

    private SkillManager skillManager;
    private FrontSlashSkill frontSlashSkill;



    private bool playedAttack1SE = false;
    private bool playedAttack2SE = false;
    private bool playedAttack3SE = false;


    private string lastStateName = "";
    private bool hasPlayedSE = false;

    

    void PlayAttackSEOnAnimationStart() {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0); // レイヤー0

        string currentStateName = GetCurrentStateName(stateInfo);

        if (currentStateName != lastStateName) {
            lastStateName = currentStateName;
            hasPlayedSE = false;
        }

        if (!hasPlayedSE && (currentStateName == "Attack1" || currentStateName == "Attack2" || currentStateName == "Attack3")) {
            AudioManager.Instance.PlaySE("FA");
            hasPlayedSE = true;
        }
    }

    string GetCurrentStateName(AnimatorStateInfo stateInfo) {
        // AnimatorControllerのステート名を取得するには、Animatorの状態名を文字列で比較する必要があります。
        // Animatorのステート名をハッシュで取得している場合は、Animator.StringToHash("Attack1") などで比較してください。
        // ここでは簡易的にステート名を取得する方法を仮定しています。
        // 実際には Animator.GetCurrentAnimatorClipInfo() を使ってクリップ名を取得する方法もあります。
        return anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;
    }

    void CheckAttackAnimationEnd() {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Idle")) {
            if (!wasInIdle) {
                anim.SetBool("Attack1", false);
                anim.SetBool("Attack2", false);
                anim.SetBool("Attack3", false);
                wasInIdle = true;

                // フラグリセット
                playedAttack1SE = false;
                playedAttack2SE = false;
                playedAttack3SE = false;
            }
        }
        else {
            wasInIdle = false;
        }

        if (stateInfo.IsName("Sword And Shield Slash") && stateInfo.normalizedTime >= 0.8f && !playedAttack1SE) {
            AudioManager.Instance.PlaySE("FA");
            playedAttack1SE = true;
        }

        if (stateInfo.IsName("Sword And Shield Slash (2)") && stateInfo.normalizedTime >= 0.8f && !playedAttack2SE) {
            AudioManager.Instance.PlaySE("SA");
            playedAttack2SE = true;
        }

        if (stateInfo.IsName("Sword And Shield Slash (1)") && stateInfo.normalizedTime >= 0.8f && !playedAttack3SE) {
            AudioManager.Instance.PlaySE("EA");
            playedAttack3SE = true;
        }
    }

    void Start() {
        attackCollider.enabled = false;
        // PlayerInput から Attack アクションを取得
        input = GetComponent<PlayerInput>();
        attackAction = input.actions["Attack"];
        if (attackAction == null) {
            Debug.LogError("Attack アクションが見つかりません");
        }
        skillManager = new SkillManager();

        frontSlashSkill = new FrontSlashSkill();
        skillManager.RegisterSkill(frontSlashSkill);
    }

    void Update() {
        PlayAttackSEOnAnimationStart();
        LowAttack();             // 攻撃処理
        HandleAttackCooldown();  // クールタイム処理
        CheckAttackAnimationEndr();
        if (Keyboard.current.digit1Key.wasPressedThisFrame) {
            skillManager.UseSkill(0, gameObject);
        }
    }

    // 通常攻撃（3連コンボ）


    public void LowAttack() {
        attackCollider.enabled = comboStep > 0;

        if (isAttackCooldown) return;

        // 攻撃ボタンが押された瞬間
        if (attackAction.WasPressedThisFrame()) {
            PlayerController.attackFlag = true;

            if (comboStep == 0) {
                // 最初の攻撃
                comboStep = 1;
                anim.SetBool("Attack1", true);
                comboTimer = comboResetTime;
                AudioManager.Instance.PlaySE("FA"); // 攻撃1のSE
            }
            else if (comboTimer > 0f) {
                // コンボ猶予時間内に再度攻撃された場合のみ次のステップへ
                comboStep++;
                if (comboStep == 2) {
                    anim.SetBool("Attack2", true);
                    comboTimer = comboResetTime;
                    AudioManager.Instance.PlaySE("SA"); // 攻撃2のSE
                }
                else if (comboStep == 3) {
                    anim.SetBool("Attack3", true);
                    comboTimer = comboResetTime;
                    AudioManager.Instance.PlaySE("EA"); // 攻撃3のSE
                    StartAttackCooldown(); // 最終段でクールタイム開始
                }
            }
        }

        // コンボ猶予時間の管理
        if (comboStep > 0) {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f) {
                PlayerController.attackFlag = false;
                comboStep = 0;
                StartAttackCooldown(); // コンボ中断時もクールタイム開始
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
            //StartCoroutine(EnableAttackCollider(comboStep));

            // 3段目まで出し切ったらクールタイム開始
            if (comboStep == 2) {
                //PlayerController.attackFlag = false;
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

        //attackCollider.enabled = true;               // 攻撃判定を有効化
        yield return new WaitForSeconds(0.2f);       // 判定持続時間
        //attackCollider.enabled = false;              // 攻撃判定を無効化
    }



    void CheckAttackAnimationEndr() {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);


        if (stateInfo.IsName("Idle")) {
            if (!wasInIdle) {
                // Idle に入った瞬間
                anim.SetBool("Attack1", false);
                anim.SetBool("Attack2", false);
                anim.SetBool("Attack3", false);
                wasInIdle = true;
            }
        }
        else {
            wasInIdle = false;
        }


        if (stateInfo.IsName("Sword And Shield Slash") && stateInfo.normalizedTime >= 0.8f) {
            anim.SetBool("Attack1", false);
            
                //AudioManager.Instance.PlaySE("FA");
            
        }
        if (stateInfo.IsName("Sword And Shield Slash (2)") && stateInfo.normalizedTime >= 0.8f) {
            anim.SetBool("Attack2", false);
            //AudioManager.Instance.PlaySE("SA");
        }
        if (stateInfo.IsName("Sword And Shield Slash (1)") && stateInfo.normalizedTime >= 0.8f) {
            anim.SetBool("Attack3", false);
            //AudioManager.Instance.PlaySE("EA");
        }
    }



    public void OnTriggerEnter(Collider other) {
        // エネミーの攻撃に触れた場合
        if (other.CompareTag("EnemyAttack")) {
            StartAttackCooldown();
            // ダメージ処理（例：HPを減らす）
            //////////////////////////////////////hp -= other.GetComponent<EnemyAttack>().damage;

            // ヒットリアクション
            animator.SetTrigger("Hit");

                // 死亡判定
                
            
        }
    }
}
