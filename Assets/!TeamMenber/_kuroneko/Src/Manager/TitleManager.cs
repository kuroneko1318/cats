using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour {
    [Header("ロード画面シーン名")]
    [SerializeField] private string loadingSceneName = "LoadingScene";

    private PlayerInput playerInput;
    private InputAction selectAction;

    // 連打防止フラグ
    private bool isLoading = false;

    void Awake() {
        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null) {
            Debug.LogError("PlayerInput が見つかりません。TitleManagerと同じオブジェクトに追加してください。");
            return;
        }

        selectAction = playerInput.actions["Select"];
    }

    void OnEnable() {
        if (selectAction != null)
            selectAction.performed += OnSelect;
    }

    void OnDisable() {
        if (selectAction != null)
            selectAction.performed -= OnSelect;
    }

    private void OnSelect(InputAction.CallbackContext context) {
        if (isLoading) return; // 既にロード中なら無視
        isLoading = true;

        Debug.Log("タイトル画面で決定ボタンを押しました → ロード画面へ");

        if (!string.IsNullOrEmpty(loadingSceneName)) {
            SceneManager.LoadScene(loadingSceneName);
        }
    }
}
