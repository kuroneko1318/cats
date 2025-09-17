using UnityEngine;
using UnityEngine.UI;

public class StageSelectUI : MonoBehaviour {
    [Header("ボタン一覧")]
    [SerializeField] private Button[] stageButtons; // エリア選択ボタン
    [SerializeField] private Button returnButton;   // 拠点に戻るボタン

    private StageManager stageManager; // ステージマネージャー参照

    private void Start() {
        // StageManagerを探す
        stageManager = FindObjectOfType<StageManager>();

        // ステージボタンに処理を追加
        for (int i = 0; i < stageButtons.Length; i++) {
            int index = i; // ローカルコピー（ラムダ用）
            stageButtons[i].onClick.AddListener(() => OnSelectStage(index));
        }

        // 戻るボタンに処理を追加
        if (returnButton != null) {
            returnButton.onClick.AddListener(OnReturnBase);
        }

        // 初期状態では非表示
        gameObject.SetActive(false);
    }

    // UIを表示
    public void Show() {
        gameObject.SetActive(true);
    }

    // UIを閉じる
    private void Hide() {
        gameObject.SetActive(false);
    }

    // ステージ選択処理
    private void OnSelectStage(int index) {
        if (stageManager != null) {
            stageManager.EnterArea(index);
        }
        Hide(); // 選択後にUIを閉じる
    }

    // 拠点に戻る処理
    private void OnReturnBase() {
        if (stageManager != null) {
            stageManager.ReturnToBase();
        }
        Hide(); // 選択後にUIを閉じる
    }
}
