
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour {
    public Transform target;
    public Vector3 offset = new Vector3(0, 2.5f, -4f);
    public float mouseSensitivity = 1.5f;
    public float minYAngle = -35f;
    public float maxYAngle = 60f;

    private float rotX = 0f;
    private float rotY = 0f;
    private Vector2 lookInput = Vector2.zero;

    private PlayerInput input;
    private InputAction camAction;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        input = GetComponent<PlayerInput>();
        if (input == null) {
            Debug.LogError("PlayerInput が取得できませんでした");
            return;
        }

        camAction = input.actions["CameraMove"];
        if (camAction == null) {
            Debug.LogError("CameraMove アクションが見つかりません");
            return;
        }

        camAction.Enable();
    }

    void LateUpdate() {
        if (target == null) {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null) {
                target = playerObj.transform;
            }
            else {
                return;
            }
        }

        // 🔹 InputAction からの入力
        Vector2 actionInput = camAction.ReadValue<Vector2>();

        // 🔹 マウスの Raw Input（優先度を調整したい場合はここで調整可能）
        Vector2 mouseInput = Vector2.zero;
        if (Mouse.current != null) {
            mouseInput = Mouse.current.delta.ReadValue();
        }

        // 両方を合算（必要に応じてウェイトを調整）
        lookInput = actionInput + mouseInput;

        rotY += lookInput.x * mouseSensitivity;
        rotX -= lookInput.y * mouseSensitivity;
        rotX = Mathf.Clamp(rotX, minYAngle, maxYAngle);

        Quaternion rotation = Quaternion.Euler(rotX, rotY, 0);
        Vector3 targetPosition = target.position + rotation * offset;

        transform.position = targetPosition;
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
