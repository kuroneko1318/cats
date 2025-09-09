using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

//Update にプレイヤーの操作ぶち込む

public class PlayerBase : CharacterBase {
    public static int attacker;
    // プレイヤーの入力管理
    public InputAction attackAction; // 攻撃入力
    [SerializeField] public Animator anim; // アニメーター参照

    // 攻撃判定用のコライダー（Trigger）
    public Collider attackCollider;

    public int count; // 汎用カウンター（用途不明）

    // コンボ管理
    public int comboStep = 0; // 現在のコンボ段階（1〜3）
    private float comboResetTime = 1f; // コンボ入力猶予時間
    private float comboTimer = 0f; // コンボ猶予タイマー

    // クールタイム管理
    private bool isAttackCooldown = false; // クールタイム中かどうか
    [SerializeField] private float attackCooldownDuration = 0.5f; // クールタイムの長さ
    private float attackCooldownTimer = 0f; // クールタイム残り時間

    private bool wasInIdle = false; // 前回の状態がIdleかどうか

    public SkillManager skillManager; // スキル管理クラス
    public FrontSlashSkill frontSlashSkill; // 特定スキル（前方斬り）

    // 攻撃SE再生フラグ
    private bool playedAttack1SE = false;
    private bool playedAttack2SE = false;
    private bool playedAttack3SE = false;

    private string lastStateName = ""; // 最後のアニメーションステート名
    private bool hasPlayedSE = false; // SE再生済みフラグ
                                      // プレイヤー入力アクション
    public PlayerInput input;
    public InputAction moveAction;
    public InputAction jumpAction;
    public InputAction avoidanceAction;
    public InputAction GatherAction;

    public GameObject cam; // カメラ参照
    public Rigidbody rb; // プレイヤーのRigidbody
    //public Collider damageCollider; // ダメージ判定用コライダー

    public Vector3 latestPos; // 最新位置
    public Vector3 direction; // 移動方向
    public Vector3 diff; // 位置差分

    public Vector3 intial=new Vector3(0,-9.8f,0);

    public bool isAvoiding = false; // 回避中かどうか
    public static bool attackFlag = false; // 攻撃中フラグ
    private float avoidanceTimer = 0; // 回避時間
    private float avoidanceDuration = 0.002f; // 回避持続時間

    private float avoidanceCooldown = 1f; // 回避クールタイム
    private float cooldownTimer = 0f; // クールタイム残り時間

    public Vector3 initialPosition; // 初期位置

    // 会心率（最大値は1.00f）
    public float criticalChance;
    // 会心ダメージ倍率
    public float criticalMultiplier;

    private const int _WEAPON_ID = 1001; // 武器ID（固定）

    private string inputItemString; // 装備入力文字列
    [SerializeField] private TMP_InputField EquipmentinputField; // 装備入力フィールド
    public ItemManager itemManager; // アイテム管理
    public ItemManager weaponManager; // 武器管理
    WeaponBase weapon; // 装備中の武器
    Inventory inventory = null; // インベントリ参照
    [SerializeField] RawImage swordImage; // 武器画像表示

    [SerializeField] public Animator animator; // アニメーター参照（重複あり）

    bool isCritical = false; // クリティカル判定

    void Update() {
        // 毎フレームの更新処理（未実装）
    }

    // 抽象メソッドのオーバーライド（未実装）
    public override void Attack() {
        throw new System.NotImplementedException();
    }

    public override void Dead() {
        if (hp <= 0) {
            animator.SetTrigger("Death");
            OnDeathAnimationEnd();
        }
    }

    public override void HealHp() {
        throw new System.NotImplementedException();
    }

    // 移動処理（アニメーションのみ）
    public override void Move() {
        animator.SetBool("Run", true);
    }

    
    public void SetDamage(int power) {
        damage = power;
    }

    
    // ダメージ処理
    public virtual void TakeDamage(int attack, float motionMultiplier = 1, float criticalChance = 0, float criticalMultiplier = 2,
                                   int elementalValue = 0, float staggerValue = 0) {
        isCritical = Random.value < criticalChance; // クリティカル判定
        if (isCritical) {
            damage = Mathf.RoundToInt(
                (Mathf.Pow(attack, 2) / attack + defence) * motionMultiplier * Random.Range(0.90f, 1.1f) * criticalMultiplier);
            hp -= damage;
        }
        else {
            damage = Mathf.RoundToInt(
                (Mathf.Pow(attack, 2) / attack + defence) * motionMultiplier * Random.Range(0.90f, 1.1f));
            hp -= damage;
        }

        animator.SetBool("Hit", true); // 被ダメージアニメーション
        StartCoroutine(ResetHitFlagAfterDelay(0.1f)); // 一定時間後にHitフラグを戻す

        if (hp <= 0) Dead(); // HPが0以下なら死亡処理
    }

    // Hitフラグを戻すコルーチン
    private IEnumerator ResetHitFlagAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        animator.SetBool("Hit", false);
    }

    // 死亡アニメーション処理
    

    // プレイヤー移動処理
    public void PlayerMove() {
        if (isAvoiding) return; // 回避中は移動不可

        Vector2 inputVector = moveAction.ReadValue<Vector2>();
        if (inputVector != Vector2.zero) {
            animator.SetBool("Run", true);

            // カメラ方向に基づく移動
            Vector3 camForward = cam.transform.forward;
            Vector3 camRight = cam.transform.right;

            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDir = camForward * inputVector.y + camRight * inputVector.x;

            rb.velocity = moveDir * moveSpeed;
            direction = moveDir;

            transform.rotation = Quaternion.LookRotation(moveDir); // 移動方向に回転
        }
        else {
            animator.SetBool("Run", false);
        }
    }

    // プレイヤーの回転のみ処理
    public void RotMove() {
        Vector2 inputVector = moveAction.ReadValue<Vector2>();
        if (inputVector != Vector2.zero) {
            Vector3 camForward = cam.transform.forward;
            Vector3 camRight = cam.transform.right;

            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDir = camForward * inputVector.y + camRight * inputVector.x;
            transform.rotation = Quaternion.LookRotation(moveDir);
        }
    }

    // 回避処理
    public void Avoidance() {
        if (cooldownTimer > 0f) {
            cooldownTimer -= Time.deltaTime;
            return;
        }

        if (avoidanceAction.WasPressedThisFrame() && !isAvoiding) {
            animator.SetTrigger("Avoidance");

            isAvoiding = true;
            isInvincible = true;
            avoidanceTimer = avoidanceDuration;
            cooldownTimer = avoidanceCooldown;

            rb.velocity = transform.forward * moveSpeed * 2f; // 前方に高速移動
        }

        if (isAvoiding) {
            avoidanceTimer -= Time.deltaTime;
            if (avoidanceTimer <= 0f) {
                isAvoiding = false;
                isInvincible = false;
            }
        }
    }


    public void Stop() {
        if (!isAvoiding && !moveAction.IsInProgress() && !avoidanceAction.IsInProgress()) {
            rb.velocity = intial;
        }
    }


    public void AddAttack(int _weaponATK) {
        attack += _weaponATK;
    }



    private void OnTriggerStay(Collider other) {
        if (other.gameObject.CompareTag("GatheringPoint") && GatherAction.WasPressedThisFrame()) {
            var point = other.gameObject.GetComponent<GatheringPoint>();
            point?.Gather();
        }
    }

    // 死亡アニメーション終了後に呼ばれる
    public void OnDeathAnimationEnd() {
        transform.position = initialPosition;
        hp = maxHp;
        animator.ResetTrigger("Death");
        // 必要に応じて他の初期化処理
    }


    public void PlayAttackSEOnAnimationStart() {
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

    public void CheckAttackAnimationEnd() {
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


        if (stateInfo.IsName("Sword And Shield Slash") && stateInfo.normalizedTime >= 0.3f && !playedAttack1SE) {
            AudioManager.Instance.PlaySE("FA");
            StartCoroutine(EnableColliderTemporarily(0.1f)); // 攻撃1の判定時間
            playedAttack1SE = true;
        }

        if (stateInfo.IsName("Sword And Shield Slash") && stateInfo.normalizedTime >= 0.8f) {
 anim.SetBool("Attack1", false);
            
        }

        if (stateInfo.IsName("Sword And Shield Slash (2)") && stateInfo.normalizedTime >= 0.3f && !playedAttack2SE) {
            AudioManager.Instance.PlaySE("SA");
            StartCoroutine(EnableColliderTemporarily(0.1f)); // 攻撃2の判定時間
        
        playedAttack2SE = true;
        }
        if (stateInfo.IsName("Sword And Shield Slash (2)") && stateInfo.normalizedTime >= 0.8f) {
            anim.SetBool("Attack2", false);
        }

        if (stateInfo.IsName("Sword And Shield Slash (1)") && stateInfo.normalizedTime >= 0.3f && !playedAttack3SE) {
            AudioManager.Instance.PlaySE("EA");
            StartCoroutine(EnableColliderTemporarily(0.1f)); // 攻撃3の判定時間
            playedAttack3SE = true;
        }
        if (stateInfo.IsName("Sword And Shield Slash (1)") && stateInfo.normalizedTime >= 0.8f) {
            anim.SetBool("Attack3", false);
        }
    }


    public void LowAttack() {
        if (isAttackCooldown) return;

        if (attackAction.WasPressedThisFrame()) {
            PlayerController.attackFlag = true;

            if (comboStep == 0) {
                comboStep = 1;
                anim.SetBool("Attack1", true);
                comboTimer = comboResetTime;
                //AudioManager.Instance.PlaySE("FA");
            }
            else if (comboTimer > 0f) {
                comboStep++;
                if (comboStep == 2) {
                    anim.SetBool("Attack2", true);
                    comboTimer = comboResetTime;
                    //AudioManager.Instance.PlaySE("SA");
                }
                else if (comboStep == 3) {
                    anim.SetBool("Attack3", true);
                    comboTimer = comboResetTime;
                    //AudioManager.Instance.PlaySE("EA");
                    StartAttackCooldown();
                }
            }
        }

        if (comboStep > 0) {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f) {
                PlayerController.attackFlag = false;
                comboStep = 0;
                StartAttackCooldown();
            }
        }
    }

   

    private IEnumerator EnableColliderTemporarily(float duration) {
        attackCollider.enabled = true;
        yield return new WaitForSeconds(duration);
        attackCollider.enabled = false;
    }


    

    // クールタイム開始処理
    private void StartAttackCooldown() {
        playedAttack1SE= false;
        playedAttack2SE= false;
        playedAttack3SE= false;
        isAttackCooldown = true;
        attackCooldownTimer = attackCooldownDuration;
    }

    // クールタイムの時間管理
    public void HandleAttackCooldown() {
        if (isAttackCooldown) {
            attackCooldownTimer -= Time.deltaTime;
            if (attackCooldownTimer <= 0f) {
                isAttackCooldown = false;
            }
        }
    }

    // 攻撃判定のサイズ変更と一時的な有効化
    public IEnumerator EnableAttackCollider(int step) {
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



    



    
}
