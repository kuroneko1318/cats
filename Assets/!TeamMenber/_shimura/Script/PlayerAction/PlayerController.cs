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
    InputAction MateliargetAction;
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
    private float avoidanceDuration = 0.002f;  // 回避の持続時間

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


        MateliargetAction = input.actions["Mateliar"];
        if (MateliargetAction == null) {
            Debug.LogError("Mateliar アクションが見つかりません");
        }


        cam = GameObject.Find("Main Camera");
    }

    void Update() {
        PlayerMove();   // 移動処理
        RotMove();      // 回転処理
        Avoidance();    // 回避処理
        Stop();         // 停止処理（慣性制御）

    }

    // プレイヤーの向きを移動方向に合わせる処理
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

    // プレイヤーの移動処理
    public void PlayerMove() {
        if (isAvoiding) return;

        Vector2 inputVector = moveAction.ReadValue<Vector2>();
        if (inputVector != Vector2.zero) {
            animator.SetBool("Run", true);

            Vector3 camForward = cam.transform.forward;
            Vector3 camRight = cam.transform.right;

            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDir = camForward * inputVector.y + camRight * inputVector.x;

            rb.velocity = moveDir * moveSpeed;
            direction = moveDir;

            transform.rotation = Quaternion.LookRotation(moveDir);
        }
        else {
            animator.SetBool("Run", false);
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
            animator.SetTrigger("Avoidance");

            isAvoiding = true;
            isInvincible = true;
            avoidanceTimer = avoidanceDuration;
            cooldownTimer = avoidanceCooldown;

            // 前方に高速移動
            rb.velocity = transform.forward * moveSpeed * 2f;
        }

        // 回避中の時間管理
        if (isAvoiding) {
            avoidanceTimer -= Time.deltaTime;
            if (avoidanceTimer <= 0f) {
                
                isAvoiding = false;

                isInvincible = false;
               // animator.SetBool("Avoidance", false);
            }
        }
    }

    public void MateGet() {
        if (MateliargetAction.WasPressedThisFrame()) {
            getM = true;
        }
       
    }


    // 停止処理（キー入力がないときに慣性を止める。ただし回避中は除外）
    public void Stop() {
        if (!isAvoiding && !moveAction.IsInProgress() && !avoidanceAction.IsInProgress()) {
            //animator.SetBool("Avoidance", false);
            rb.velocity = Vector3.zero;
        }
    }

    public void AddAttack(int _weaponATK) {
        attack += _weaponATK;
    }

    private void OnTriggerStay(Collider other) {
       


            if (other.gameObject.CompareTag("GatheringPoint")&&getM==true) {
                var point = other.gameObject.GetComponent<GatheringPoint>();
                point?.Interact();
            }
        
    }
}
