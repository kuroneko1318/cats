using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    [Header("追従対象（プレイヤーなど）")]
    public Transform target; // カメラが追従するオブジェクト（プレイヤーなど）

    [Header("カメラの相対位置（背後・上から）")]
    public Vector3 offset = new Vector3(0, 2.5f, -4f); // ターゲットから見た相対的な位置

    [Header("感度")]
    public float mouseSensitivity = 1.5f; // 視点移動の感度（マウス・スティック共通）

    [Header("仰角の制限")]
    public float minYAngle = -35f; // カメラが下を向く限界角度
    public float maxYAngle = 60f;  // カメラが上を向く限界角度

    // 現在の回転角度
    private float rotX = 0f; // 垂直方向の回転（上下）
    private float rotY = 0f; // 水平方向の回転（左右）

    // 新InputSystemで受け取った「視点入力（マウス or スティック）」の値
    private Vector2 lookInput = Vector2.zero;

    // 入力設定（InputActionAssetから生成したプレイヤー用クラス）
    private PlayerInput inputActions;

    /// <summary>
    /// 起動時に入力設定を構築・入力イベントを登録
    /// </summary>
    void Awake() {
        inputActions = new PlayerInput(); // 入力アセットクラスを生成

        // Look入力（視点移動：マウスまたは右スティック）の取得
        //inputActions.Player.CameraMove.performed += ctx => {
        //    lookInput = ctx.ReadValue<Vector2>(); // マウス移動やスティックの入力を受け取る
        //};

        //// 入力が止まったとき（スティックを離したなど）はゼロに
        //inputActions.Player.Look.canceled += ctx => {
        //    lookInput = Vector2.zero;
        //};
    }

    /// <summary>
    /// このスクリプトが有効になったときに入力を有効化
    /// </summary>
    void OnEnable() {
        //inputActions.Player.Enable(); // プレイヤー用入力アクションを有効に
    }

    /// <summary>
    /// 無効化されたときに入力を無効にする
    /// </summary>
    void OnDisable() {
        //inputActions.Player.Disable(); // 不要になったら入力を止める（パフォーマンス向上）
    }

    /// <summary>
    /// ゲーム開始時にカーソルを非表示＆ロック（マウスを自由視点用に固定）
    /// </summary>
    void Start() {
        Cursor.lockState = CursorLockMode.Locked; // カーソルを画面中央にロック
        Cursor.visible = false;                   // カーソルを非表示
    }

    /// <summary>
    /// フレームの最後にカメラを移動（プレイヤー移動後に反映）
    /// </summary>
    void LateUpdate() {
        if (target == null) return; // 追従対象がいなければ何もしない

        // 視点移動（マウス or スティック）を感度付きで適用
        rotY += lookInput.x * mouseSensitivity;  // 横方向の回転（右スティック横 or マウスX）
        rotX -= lookInput.y * mouseSensitivity;  // 縦方向の回転（上でマイナス）

        // 上下の回転に制限をかける（真上・真下を向きすぎないように）
        rotX = Mathf.Clamp(rotX, minYAngle, maxYAngle);

        // 計算した回転をクォータニオン（3D回転）に変換
        Quaternion rotation = Quaternion.Euler(rotX, rotY, 0f);

        // 回転後のカメラ位置を算出（プレイヤー位置 + オフセット）
        Vector3 targetPosition = target.position + rotation * offset;

        // カメラの位置を更新
        transform.position = targetPosition;

        // プレイヤーの少し上（1.5mくらい）を注視（上半身を映すように）
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
