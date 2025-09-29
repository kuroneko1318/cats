using UnityEngine;

public class NewPlayerMove {
    private Transform playerTransform;     // プレイヤーのTransform
    private Animator anim;                 // アニメーション管理
    private float moveSpeed = 10f;         // 移動速度
    private float avoidanceForce = 8f;     // 回避時の力
    private bool isAvoiding = false;       // 回避中フラグ
    private Vector3 avoidanceDir;          // 回避方向
    private float avoidanceTime = 0.3f;    // 回避の持続時間
    private float avoidanceTimer = 0f;
    private bool isMoving = false;

    private Vector3 lastPosition;          // 前フレーム位置

    public NewPlayerMove(Transform transform, Animator animator) {
        playerTransform = transform;
        anim = animator;
        lastPosition = playerTransform.position;
    }

    public bool IsMoving() => isMoving;

    // 移動処理（カメラ基準）
    public void Move(Vector2 input, Transform cameraTransform) {
        if (isAvoiding) {
            // 回避中は回避方向に進む
            playerTransform.position += avoidanceDir * avoidanceForce * Time.deltaTime;
            avoidanceTimer -= Time.deltaTime;
            if (avoidanceTimer <= 0) {
                isAvoiding = false;
            }
            UpdateMovementState();
            return;
        }

        // カメラ基準の入力ベクトルを作成
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = camForward * input.y + camRight * input.x;

        if (move.sqrMagnitude > 0.01f) {
            // プレイヤーを移動
            playerTransform.position += move.normalized * moveSpeed * Time.deltaTime;

            // 向きを移動方向へ
            playerTransform.rotation = Quaternion.Slerp(
                playerTransform.rotation,
                Quaternion.LookRotation(move),
                0.2f
            );
        }

        // 実際に動いたかどうかをチェック
        UpdateMovementState();
    }

    // 回避処理
    public void Avoid(Vector2 input, Transform cameraTransform) {
        if (isAvoiding) return; // 連続回避防止

        // カメラ基準で方向を決定
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        avoidanceDir = (camForward * input.y + camRight * input.x).normalized;
        if (avoidanceDir == Vector3.zero) {
            avoidanceDir = playerTransform.forward; // 入力なしなら前方向に回避
        }

        isAvoiding = true;
        avoidanceTimer = avoidanceTime;
        anim.SetTrigger("Avoidance"); // トリガー
    }

    // 実際の移動量をチェックしてRunフラグを制御
    private void UpdateMovementState() {
        float movedDistance = (playerTransform.position - lastPosition).magnitude;

        if (movedDistance > 0.001f) { // 少しでも動いてたらRun
            anim.SetBool("Run", true);
            isMoving = true;
        }
        else {
            anim.SetBool("Run", false);
            isMoving = false;
        }

        lastPosition = playerTransform.position;
    }
}
