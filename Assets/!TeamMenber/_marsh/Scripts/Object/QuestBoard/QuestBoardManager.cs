using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class QuestBoardManager : SystemObject<QuestBoardManager> {
    [Header("UI")]
    public GameObject questBoardPrefab;   // プレハブをここにアサイン
    private GameObject questBoardInstance;
    private GameObject questBoardPanel;
    public GameObject questPrefab;       // 作ったQuestPrefab
    public Transform contentParent;      // GridLayoutGroupがついた親（横3×縦2）
    public Color selectedColor = Color.white; // 選択枠の色

    [Header("クエストデータ")]
    public HuntQuest[] allQuests;       // 全クエスト（最大6個）

    private Image[] selectionImages;     // クエスト後ろの選択用Image
    private int selectedIndex = 0;

    public override void Initialize() {
        // QuestBoardUIを生成
        if (questBoardPrefab != null) {
            questBoardInstance = Instantiate(questBoardPrefab);

            // パネルを取得
            questBoardPanel = questBoardInstance.transform.Find("QuestUIPanel").gameObject;

            // Content を取得
            contentParent = questBoardPanel.transform.Find("Content");

            // 最初は非表示
            questBoardPanel.SetActive(false);
        }
        else {
            Debug.LogError("QuestBoardManager: questBoardPrefab がアサインされていません！");
        }
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

    public void MoveRight() {
        int prev = selectedIndex;
        selectedIndex = (selectedIndex + 1) % selectionImages.Length;
        if (prev != selectedIndex) UpdateSelection();
    }

    public void MoveLeft() {
        int prev = selectedIndex;
        selectedIndex = (selectedIndex - 1 + selectionImages.Length) % selectionImages.Length;
        if (prev != selectedIndex) UpdateSelection();
    }

    public void MoveUp() {
        int prev = selectedIndex;
        selectedIndex = (selectedIndex - 3 + selectionImages.Length) % selectionImages.Length;
        if (prev != selectedIndex) UpdateSelection();
    }

    public void MoveDown() {
        int prev = selectedIndex;
        selectedIndex = (selectedIndex + 3) % selectionImages.Length;
        if (prev != selectedIndex) UpdateSelection();
    }

    private void UpdateSelection() {
        for (int i = 0; i < selectionImages.Length; i++) {
            selectionImages[i].gameObject.SetActive(i == selectedIndex);
        }
    }

    public void AcceptQuest() {
        if (allQuests == null || allQuests.Length == 0) return;
        var quest = allQuests[selectedIndex];
        if (QuestManager.Instance.AcceptQuest(quest)) {
            Debug.Log($"{quest.questName} を受注しました！");
            CloseQuestBoard();
        }
        else {
            Debug.Log("既にクエスト受注中です！");
        }
    }

    public void OpenQuestBoard() {
        questBoardPanel.SetActive(true);
        selectedIndex = 0;
        UpdateSelection();
    }

    public void CloseQuestBoard() {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerBase>().input.SwitchCurrentActionMap("GamePlay");
        questBoardPanel.SetActive(false);
    }
}
