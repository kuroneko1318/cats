using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCameraManager : MonoBehaviour
{
    [Header("追従対象（プレイヤーなど）")]
    public Transform target;

    [Header("カメラの相対位置（背後・上から）")]
    public Vector3 offset = new Vector3(0, 2.5f, -4f);

    [Header("マウス感度")]
    public float mouseSensitivity = 3.0f;

    [Header("距離（未使用。Raycastなどで障害物処理する際に活用可）")]
    public float distance = 4.0f;

    [Header("仰角の制限（上下角度）")]
    public float minYAngle = -35f;
    public float maxYAngle = 60f;

    // 内部変数：現在のカメラ角度（回転）
    private float rotX = 0f; // 垂直（上下）
    private float rotY = 0f; // 水平（左右）

    void Start() {
        if (target == null) {
            Debug.LogWarning("CameraManager: 追従対象が設定されていません。");
        }

        // 初期回転角度を取得（シーン配置時の角度を保持）
        Vector3 angles = transform.eulerAngles;
        rotX = angles.x;
        rotY = angles.y;

        // カーソルロック＆非表示（マウス操作で自由視点に）
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate() {
        if (target == null) return;

        // ────────────────────────────────
        // プレイヤーのマウス操作によってカメラを回転
        // ────────────────────────────────

        // 水平方向の回転（右左）: rotY を更新
        rotY += Input.GetAxis("Mouse X") * mouseSensitivity;

        // 垂直方向の回転（上下）: rotX を更新（上下逆なのでマイナス）
        rotX -= Input.GetAxis("Mouse Y") * mouseSensitivity;

        // 上下の角度を制限（カメラが真上・真下を向きすぎないように）
        rotX = Mathf.Clamp(rotX, minYAngle, maxYAngle);

        // 回転をQuaternionに変換（回転行列）
        Quaternion rotation = Quaternion.Euler(rotX, rotY, 0);

        // カメラの最終位置を計算（ターゲット＋回転後のオフセット）
        Vector3 targetPosition = target.position + rotation * offset;

        // カメラの位置を移動
        transform.position = targetPosition;

        // ターゲット（プレイヤー）を向く（やや上を見て上半身を捉える）
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
