using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    [Header("追従対象（プレイヤーなど）")]
    public Transform target;

    [Header("カメラの相対位置（背後・上から）")]
    public Vector3 offset = new Vector3(0, 2.5f, -4f);

    [Header("感度")]
    public float mouseSensitivity = 1.5f;

    [Header("仰角の制限")]
    public float minYAngle = -35f;
    public float maxYAngle = 60f;

    private float rotX = 0f;
    private float rotY = 0f;

    private Vector2 lookInput = Vector2.zero;

    // 入力アセット（.inputactions から生成されたクラス）
    private PlayerInput inputAction;
    private InputAction cameraMoveAction;

    void Awake() {
        // 入力アセットを初期化
        inputAction = new PlayerInput();
        cameraMoveAction = inputAction.actions["CameraMove"];


        // 入力がある場合のみカメラ移動
        if (cameraMoveAction.IsInProgress()) {
            lookInput = cameraMoveAction.ReadValue<Vector2>();
        }
    }

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate() {
        if (target == null) return;

        // 視点入力を元にカメラ角度を更新
        rotY += lookInput.x * mouseSensitivity;
        rotX -= lookInput.y * mouseSensitivity;

        // 垂直角度の制限
        rotX = Mathf.Clamp(rotX, minYAngle, maxYAngle);

        // 回転計算
        Quaternion rotation = Quaternion.Euler(rotX, rotY, 0);
        Vector3 targetPosition = target.position + rotation * offset;

        // カメラ移動と注視
        transform.position = targetPosition;
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
