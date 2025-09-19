using UnityEngine;
using UnityEngine.InputSystem;   // 新Input System
using UnityEngine.SceneManagement; // シーン切り替え用

public class TitleManager : MonoBehaviour {
    [Header("遷移先シーン名")]
    [SerializeField] private string gameSceneName = "GameScene";
    // ゲーム本編のシーン名をインスペクタから指定

    private PlayerInput playerInput;
    private InputAction selectAction;

    void Awake() {
        // PlayerInput を同じオブジェクトに付ける前提
        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null) {
            Debug.LogError("PlayerInput が見つかりません。TitleManagerと同じオブジェクトに追加してください。");
            return;
        }

        // "Select" アクションを取得
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

    // 決定ボタン押下時に呼ばれる
    private void OnSelect(InputAction.CallbackContext context) {
        Debug.Log("タイトル画面で決定ボタンを押しました → ゲーム開始");

        if (!string.IsNullOrEmpty(gameSceneName)) {
            SceneManager.LoadScene(gameSceneName);
        }
    }
}
