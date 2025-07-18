using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : CharacterBase {

    // プレイヤーの入力関連
    PlayerInput input;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction attackAction;
    InputAction avoidanceAction;
    GameObject cam;
    // 物理挙動
    public Rigidbody rb;

    // 移動・回転関連
    public Vector3 latestPos;
    public Vector3 direction;
    public Vector3 diff;

    // 回避処理関連
    private bool isAvoiding = false;         // 回避中フラグ
    
    private float avoidanceTimer = 0;        // 回避時間の残り
    private float avoidanceDuration =0.002f;  // 回避の持続時間

    // クールタイム関連
    private float avoidanceCooldown = 1f;  // 回避のクールタイム（秒）
    private float cooldownTimer = 0f;        // クールタイムの残り時間




    void Start() {
        // PlayerInput コンポーネントの取得
        input = GetComponent<PlayerInput>();
        if (input == null) {
            Debug.LogError("PlayerInput が取得できませんでした");
            return;
        }

        // 各アクションの取得
        moveAction = input.actions["Move"];
        if (moveAction == null) {
            Debug.LogError("Move アクションが見つかりません");
        }

        avoidanceAction = input.actions["Avoidance"];
        if (avoidanceAction == null) {
            Debug.LogError("Avoidance アクションが見つかりません");
        }

       cam=GameObject.Find("Main Camera");
    }

    void Update() {
        PlayerMove();   // 移動処理
        RotMove();      // 回転処理
        Avoidance();    // 回避処理
        Stop();         // 停止処理（慣性制御）
        
    }

    // プレイヤーの向きを移動方向に合わせる処理
    public void RotMove() {
        diff = transform.position - latestPos; // 前回位置との差分
        latestPos = transform.position;        // 最新位置を保存

        // 一定以上動いた場合のみ回転を更新
        if (diff.magnitude > 0.01f) {
            transform.rotation = Quaternion.LookRotation(diff);

        }
    }
    // プレイヤーの移動処理
    public void PlayerMove() {
        // 回避中は移動を無効化
        if (isAvoiding) return;

        // 入力がある場合のみ移動
        if (moveAction.IsInProgress()) {
            Vector3 dir = moveAction.ReadValue<Vector3>() * moveSpeed;
            rb.velocity = new Vector3(dir.x, 0, dir.z);
            direction = dir;
        }
    }

    // 回避処理（高速移動＋無敵＋クールタイム）
    public void Avoidance() {
        // クールタイム中は回避できない
        if (cooldownTimer > 0f) {
            cooldownTimer -= Time.deltaTime;
            return;
        }

        // 回避開始（押した瞬間のみ）
        if (avoidanceAction.WasPressedThisFrame() && !isAvoiding) {
            isAvoiding = true;
            isInvincible = true;
            avoidanceTimer = avoidanceDuration;
            cooldownTimer = avoidanceCooldown;

            // 前方に高速移動
            rb.velocity = transform.forward * moveSpeed * 3f;
        }

        // 回避中の時間管理
        if (isAvoiding) {
            avoidanceTimer -= Time.deltaTime;
            if (avoidanceTimer <= 0f) {
                isAvoiding = false;
                isInvincible = false;
            }
        }
    }

    // 停止処理（キー入力がないときに慣性を止める。ただし回避中は除外）
    public void Stop() {
        if (!isAvoiding && !moveAction.IsInProgress() && !avoidanceAction.IsInProgress()) {
            rb.velocity = Vector3.zero;
        }
    }

    


  
    

    // ダメージ処理（未実装）
    public override void TakeDamage() {
        // isInvincible が true の場合はダメージ無効にするなどの処理を追加可能
    }

    // 回復処理（未実装）
    public override void HealHp() {
    }

    // 死亡処理（未実装）
    public override void Dead() {
    }

    public override void Attack() {
        throw new System.NotImplementedException();
    }

    public override void Move() {
        throw new System.NotImplementedException();
    }
}
