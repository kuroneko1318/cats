using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; // Input Systemを使用

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

    // InputSystem関連
    private PlayerInput playerInput;
    private InputAction moveMenuAction;
    private InputAction selectAction;

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
        if (panel != null)
            panel.SetActive(false);

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

            stageButtons[i].onClick.AddListener(() => ActivateStage(index));
        }

        if (returnButton != null)
            returnButton.onClick.AddListener(OnReturnBase);
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

        Vector2 input = ctx.ReadValue<Vector2>();

        // 上下で移動
        if (input.y > 0.5f) {
            selectedIndex--;
            if (selectedIndex < 0) selectedIndex = stageButtons.Length;
            UpdateCursor();
        }
        else if (input.y < -0.5f) {
            selectedIndex++;
            if (selectedIndex > stageButtons.Length) selectedIndex = 0;
            UpdateCursor();
        }

        // （オプションで左右移動も対応可。必要ならここに追加）
    }

    private void OnSelect(InputAction.CallbackContext ctx) {
        if (panel == null || !panel.activeSelf) return;
        ActivateSelected();
    }

    private void UpdateCursor() {
        if (cursorImage == null) return;

        RectTransform target = (selectedIndex < stageButtons.Length) ?
            stageButtons[selectedIndex].GetComponent<RectTransform>() :
            returnButton.GetComponent<RectTransform>();

        Vector2 anchoredPos = target.anchoredPosition;
        float scaledWidth = target.rect.width * target.lossyScale.x;
        anchoredPos.x -= scaledWidth / 2 + cursorOffsetX;
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
        if (panel != null)
            panel.SetActive(true);

        selectedIndex = 0;
        UpdateCursor(); // カーソルを表示＆初期位置更新
    }

    public void Hide() {
        if (panel != null)
            panel.SetActive(false);

        if (cursorImage != null)
            cursorImage.gameObject.SetActive(false);
    }
}
