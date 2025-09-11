using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewPlayerMove
{
    private Transform playerTransform;         // プレイヤーのTransform
    private Animator anim;                     // アニメーション管理
    private float moveSpeed = 5f;              // 移動速度
    private float avoidanceForce = 8f;         // 回避時の力
    private bool isAvoiding = false;           // 回避中フラグ
    private Vector3 avoidanceDir;              // 回避方向
    private float avoidanceTime = 0.3f;        // 回避の持続時間
    private float avoidanceTimer = 0f;

    public NewPlayerMove(Transform transform, Animator animator) {
        playerTransform = transform;
        anim = animator;
    }

    // 移動処理（カメラ基準）
    public void Move(Vector2 input, Transform cameraTransform) {
        if (isAvoiding) {
            // 回避中は回避方向に進む
            playerTransform.position += avoidanceDir * avoidanceForce * Time.deltaTime;
            avoidanceTimer -= Time.deltaTime;
            if (avoidanceTimer <= 0) {
                isAvoiding = false;
            }
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

            anim.SetBool("Run", true); // 移動アニメON
        }
        else {
            anim.SetBool("Run", false); // 移動アニメOFF
        }
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
}
