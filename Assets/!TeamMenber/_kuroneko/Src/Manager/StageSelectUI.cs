using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    private void Update() {
        if (panel == null || !panel.activeSelf) return;

        // 矢印キーで選択移動
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            selectedIndex--;
            if (selectedIndex < 0) selectedIndex = stageButtons.Length;
            UpdateCursor();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            selectedIndex++;
            if (selectedIndex > stageButtons.Length) selectedIndex = 0;
            UpdateCursor();
        }

        // 決定
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
            ActivateSelected();
        }
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
        UpdateCursor(); // ここでカーソルを表示＆初期位置更新
    }

    public void Hide() {
        if (panel != null)
            panel.SetActive(false);

        if (cursorImage != null)
            cursorImage.gameObject.SetActive(false);
    }
}
