using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class StageSelectUI : MonoBehaviour {
    [Header("UIパネル")]
    [SerializeField] private GameObject panel;

    [Header("ステージボタン")]
    [SerializeField] private Button[] stageButtons;
    [SerializeField] private Button returnButton;

    [Header("ステージ名")]
    [SerializeField] private string[] stageNames;

    [Header("カーソル設定")]
    [SerializeField] private RectTransform cursorImage;
    [SerializeField] private float cursorOffsetX = 20f;

    private StageManager stageManager;
    private int selectedIndex = 0;
    private Vector3 cursorInitialScale;

    // 入力関連
    private PlayerInput playerInput;
    private InputAction moveMenuAction;
    private InputAction selectAction;

    // 入力遅延（連続入力防止）
    private float inputDelay = 0.2f;
    private float lastInputTime = 0f;

    private void Awake() {
        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null) {
            Debug.LogError("PlayerInput コンポーネントが見つかりません");
            return;
        }

        moveMenuAction = playerInput.actions["MoveMenu"];
        selectAction = playerInput.actions["Select"];
    }

    private void Start() {
        if (panel != null) panel.SetActive(false);

        if (cursorImage != null) {
            cursorImage.gameObject.SetActive(false);
            cursorInitialScale = cursorImage.localScale;
        }

        stageManager = FindObjectOfType<StageManager>();

        // ボタンにステージ名を反映＆クリック設定
        for (int i = 0; i < stageButtons.Length; i++) {
            int index = i;
            TMP_Text tmpText = stageButtons[i].GetComponentInChildren<TMP_Text>();
            if (tmpText != null && i < stageNames.Length)
                tmpText.text = stageNames[i];

            // OnClickイベント登録
            stageButtons[i].onClick.AddListener(() => ActivateStage(index));

            // NavigationをNoneにする（Unityの自動移動を無効化）
            var nav = stageButtons[i].navigation;
            nav.mode = Navigation.Mode.None;
            stageButtons[i].navigation = nav;
        }

        if (returnButton != null) {
            returnButton.onClick.AddListener(OnReturnBase);
            var nav = returnButton.navigation;
            nav.mode = Navigation.Mode.None;
            returnButton.navigation = nav;
        }
    }

    private void OnEnable() {
        if (moveMenuAction != null) moveMenuAction.performed += OnMoveMenu;
        if (selectAction != null) selectAction.performed += OnSelect;
    }

    private void OnDisable() {
        if (moveMenuAction != null) moveMenuAction.performed -= OnMoveMenu;
        if (selectAction != null) selectAction.performed -= OnSelect;
    }

    private void OnMoveMenu(InputAction.CallbackContext ctx) {
        if (panel == null || !panel.activeSelf) return;

        if (Time.time - lastInputTime < inputDelay) return;
        lastInputTime = Time.time;

        Vector2 input = ctx.ReadValue<Vector2>();

        // 上移動
        if (input.y > 0.5f) {
            selectedIndex--;
            if (selectedIndex < 0) selectedIndex = stageButtons.Length; // Returnに移動
        }
        // 下移動
        else if (input.y < -0.5f) {
            selectedIndex++;
            if (selectedIndex > stageButtons.Length) selectedIndex = 0; // 先頭に戻る
        }

        UpdateCursor();
    }

    private void OnSelect(InputAction.CallbackContext ctx) {
        if (panel == null || !panel.activeSelf) return;
        ActivateSelected();
    }

    private void UpdateCursor() {
        if (cursorImage == null) return;

        RectTransform target = null;

        // ステージボタンにいるとき
        if (selectedIndex >= 0 && selectedIndex < stageButtons.Length) {
            target = stageButtons[selectedIndex].GetComponent<RectTransform>();
        }
        // Returnボタンにいるとき
        else if (selectedIndex == stageButtons.Length) {
            target = returnButton.GetComponent<RectTransform>();
        }

        if (target == null) return;

        Vector2 anchoredPos = target.anchoredPosition;
        anchoredPos.x -= target.rect.width / 2 + cursorOffsetX;
        cursorImage.anchoredPosition = anchoredPos;

        cursorImage.localScale = cursorInitialScale;
        cursorImage.SetAsLastSibling();

        if (!cursorImage.gameObject.activeSelf)
            cursorImage.gameObject.SetActive(true);
    }

    private void ActivateSelected() {
        if (selectedIndex < stageButtons.Length)
            ActivateStage(selectedIndex);
        else
            OnReturnBase();
    }

    private void ActivateStage(int index) {
        Hide();
        stageManager?.EnterArea(index);
    }

    private void OnReturnBase() {
        Hide();
        stageManager?.ReturnToBase();
    }

    public void Show() {
        if (panel != null) panel.SetActive(true);

        selectedIndex = 0;
        UpdateCursor(); // 初期位置にカーソルを表示
    }

    public void Hide() {
        if (panel != null) panel.SetActive(false);

        if (cursorImage != null)
            cursorImage.gameObject.SetActive(false);
    }
}
