using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : PlayerBase {

    // プレイヤーの入力関連
    PlayerInput input;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction attackAction;
    InputAction avoidanceAction;
    GameObject cam;
    [SerializeField]
    Animator anim;
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

        //diff = transform.position - latestPos; // 前回位置との差分
        //latestPos = transform.position;        // 最新位置を保存

        // 一定以上動いた場合のみ回転を更新
        //if (diff.magnitude > 0.01f) {
        //   transform.rotation = Quaternion.LookRotation(new Vector3(diff.x,0,diff.z));
        //
        // }
        transform.rotation=new Quaternion(0, cam.transform.rotation.y,0, cam.transform.rotation.w);
    }
    // プレイヤーの移動処理
    public void PlayerMove() {

        if (isAvoiding) return;

        Vector3 inputVector = moveAction.ReadValue<Vector3>();
        if (inputVector != Vector3.zero) {
            anim.SetBool("Run", true);

            // カメラの向きを基準にした移動方向を計算
            Vector3 camForward = cam.transform.forward;
            Vector3 camRight = cam.transform.right;

            // Y軸方向の影響を除去
            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();

            // 入力に基づく移動方向
            Vector3 moveDir = camForward * inputVector.y + camRight * inputVector.x;

            rb.velocity = moveDir * moveSpeed;
            direction = moveDir;

            // プレイヤーの向きを移動方向に合わせる
            transform.rotation = Quaternion.LookRotation(moveDir);
        }
        else {
            anim.SetBool("Run", false);
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
            anim.SetBool("Avoidance", true);

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
            anim.SetBool("Avoidance", false);
            rb.velocity = Vector3.zero;
        }
    }
    private void OnTriggerStay(Collider other) {
        if (other.gameObject.CompareTag("GatheringPoint") && Input.GetKeyDown(KeyCode.F)) {
            var point = other.gameObject.GetComponent<GatheringPoint>();
            point?.Interact();
        }
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

    public override void TakeDamage(int _attack, int elementalValue = 0, float staggerValue = 0) {
        _attack=attack;
    }
}
