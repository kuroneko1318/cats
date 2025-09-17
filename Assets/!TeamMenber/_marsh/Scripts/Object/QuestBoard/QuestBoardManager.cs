using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestBoardManager : MonoBehaviour {
    [Header("UI")]
    public GameObject questPrefab;       // 作ったQuestPrefab
    public Transform contentParent;      // GridLayoutGroupがついた親（横3×縦2）
    public Color selectedColor = Color.white; // 選択枠の色

    [Header("クエストデータ")]
    public HuntQuest[] allQuests;       // 全クエスト（最大6個）

    private Image[] selectionImages;     // クエスト後ろの選択用Image
    private int selectedIndex = 0;

    private void Start() {
        PopulateQuestBoard();
    }

    public void PopulateQuestBoard() {
        // 既存の子を削除
        foreach (Transform child in contentParent) {
            Destroy(child.gameObject);
        }

        // 配列初期化
        selectionImages = new Image[allQuests.Length];

        for (int i = 0; i < allQuests.Length; i++) {
            var quest = allQuests[i];
            GameObject obj = Instantiate(questPrefab, contentParent);

            // 名前
            obj.transform.Find("QuestType").GetComponent<TextMeshProUGUI>().text = quest.questName;
            // 倒す対象と数
            obj.transform.Find("TargetAmount").GetComponent<TextMeshProUGUI>().text =
                $"{quest.targetEnemyType} {quest.requiredAmount}体";
            // 報酬
            obj.transform.Find("Reward").GetComponent<TextMeshProUGUI>().text =
                $"{quest.rewardMoney}G";

            // 選択用 Image
            var selectImg = obj.transform.Find("SelectionImage").GetComponent<Image>();
            if (selectImg != null) {
                selectionImages[i] = selectImg;
                selectImg.gameObject.SetActive(false);
            }
        }

        UpdateSelection();
    }

    private void Update() {
        if (selectionImages == null || selectionImages.Length == 0) return;

        int prevIndex = selectedIndex;

        // 十字キー移動（横3列固定）
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            selectedIndex = (selectedIndex + 1) % selectionImages.Length;
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            selectedIndex = (selectedIndex - 1 + selectionImages.Length) % selectionImages.Length;
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            selectedIndex = (selectedIndex - 3 + selectionImages.Length) % selectionImages.Length;
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            selectedIndex = (selectedIndex + 3) % selectionImages.Length;

        if (prevIndex != selectedIndex)
            UpdateSelection();

        // 決定ボタン（Aボタン or Enter）
        if (Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKeyDown(KeyCode.Return)) {
            AcceptQuest(allQuests[selectedIndex]);
        }

        // キャンセルボタン（Bボタン or Esc）
        if (Input.GetKeyDown(KeyCode.JoystickButton2) || Input.GetKeyDown(KeyCode.Escape)) {
            gameObject.SetActive(false);
        }
    }

    private void UpdateSelection() {
        for (int i = 0; i < selectionImages.Length; i++) {
            selectionImages[i].gameObject.SetActive(i == selectedIndex);
        }
    }

    private void AcceptQuest(HuntQuest quest) {
        if (QuestManager.Instance.AcceptQuest(quest)) {
            Debug.Log($"{quest.questName} を受注しました！");
            gameObject.SetActive(false); // ボード閉じる
        }
        else {
            Debug.Log("既にクエスト受注中です！");
        }
    }
}
