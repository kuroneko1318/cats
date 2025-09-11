using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerMove
{
    private CharacterController controller; // 移動制御用
    private Animator anim;                  // アニメーター参照
    private float speed = 5f;               // 通常移動速度
    private float dashSpeed = 10f;          // 回避(ダッシュ)速度
    private bool isDashing = false;         // 回避中フラグ
    private float dashTime = 0.3f;          // 回避持続時間
    private float dashTimer = 0f;
    private Vector3 dashDirection;          // 回避方向

    public NewPlayerMove(CharacterController ctrl, Animator animator) {
        controller = ctrl;
        anim = animator;
    }

    // 通常移動処理
    public void Move(Vector2 input) {
        if (isDashing) return; // 回避中は通常移動しない

        Vector3 move = new Vector3(input.x, 0, input.y);
        controller.Move(move * speed * Time.deltaTime);

        // Animatorに移動中かどうかをBoolで渡す
        bool isMoving = move.magnitude > 0.1f;
        anim.SetBool("Run", isMoving);
    }

    // 回避開始（Triggerでアニメーションを再生）
    public void Dash(Vector2 input) {
        if (isDashing) return;

        isDashing = true;
        dashTimer = dashTime;

        dashDirection = new Vector3(input.x, 0, input.y).normalized;
        if (dashDirection == Vector3.zero) dashDirection = Vector3.forward; // 入力がない場合は前方向

        anim.SetTrigger("Avoidance"); // Trigger で回避アニメを再生
    }

    // 毎フレーム更新
    public void Update() {
        if (isDashing) {
            controller.Move(dashDirection * dashSpeed * Time.deltaTime);

            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f) {
                isDashing = false;
            }
        }
    }
}
