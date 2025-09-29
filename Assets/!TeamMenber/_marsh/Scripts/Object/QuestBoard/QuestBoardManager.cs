using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class QuestBoardManager : SystemObject<QuestBoardManager> {
    [Header("UI")]
    public GameObject acceptQuestUI;
    public GameObject questBoardUIPrefab;   // プレハブをここにアサイン
    private GameObject questBoardInstance;
    private GameObject questBoardPanel;
    public GameObject questPrefab;       // 作ったQuestPrefab
    public Transform contentParent;      // GridLayoutGroupがついた親（横3×縦2）
    public Color selectedColor = Color.white; // 選択枠の色
    [NonSerialized] public GameObject UI = null;

    [Header("クエストデータ")]
    public List<HuntQuest> allQuests = new List<HuntQuest>();   // 登録されている全クエスト
    public List<HuntQuest> displayQuests = new List<HuntQuest>(); // 表示用（最大6件）
    [SerializeField] private int maxDisplayCount = 6;

    private Image[] selectionImages;     // クエスト後ろの選択用Image
    private int selectedIndex = 0;

    public override void Initialize() {
        // QuestBoardUIを生成
        if (questBoardUIPrefab != null) {
            questBoardInstance = Instantiate(questBoardUIPrefab);

            // パネルを取得
            questBoardPanel = questBoardInstance.transform.Find("QuestUIPanel").gameObject;

            // Content を取得
            contentParent = questBoardPanel.transform.Find("Content");

            // 最初は非表示
            questBoardPanel.SetActive(false);
            UI = GameObject.FindWithTag("QuestBoardUI");
            UI.SetActive(false);
        }
        else {
            Debug.LogError("QuestBoardManager: questBoardUIPrefab がアサインされていません！");
        }
        PopulateQuestBoard();
    }

    // --- クエストボードを更新 ---
    public void PopulateQuestBoard() {
        // 既存の子を削除
        foreach (Transform child in contentParent) {
            Destroy(child.gameObject);
        }

        // --- 表示用リストを再抽選 ---
        displayQuests.Clear();
        List<HuntQuest> shuffled = new List<HuntQuest>(allQuests);
        ShuffleList(shuffled);

        HashSet<HuntQuest> chosen = new HashSet<HuntQuest>();
        foreach (var quest in shuffled) {
            if (!chosen.Contains(quest)) {
                chosen.Add(quest);
                displayQuests.Add(quest);
            }
            if (displayQuests.Count >= maxDisplayCount) break;
        }

        // 配列初期化
        selectionImages = new Image[displayQuests.Count];

        // --- UI生成 ---
        for (int i = 0; i < displayQuests.Count; i++) {
            var quest = displayQuests[i];
            GameObject obj = Instantiate(questPrefab, contentParent);

            // 名前
            obj.transform.Find("QuestType").GetComponent<TextMeshProUGUI>().text = quest.questName;
            // 倒す対象と数
            obj.transform.Find("TargetAmount").GetComponent<TextMeshProUGUI>().text =
                $"{quest.targetEnemyType} {quest.requiredAmount}体";
            // 報酬
            obj.transform.Find("Reward").GetComponent<TextMeshProUGUI>().text =
                $"ポーション";

            // 選択用 Image
            var selectImg = obj.transform.Find("SelectionImage").GetComponent<Image>();
            if (selectImg != null) {
                selectionImages[i] = selectImg;
                selectImg.gameObject.SetActive(false);
            }
        }

        UpdateSelection();
    }

    // --- シャッフル ---
    private void ShuffleList<T>(List<T> list) {
        for (int i = 0; i < list.Count; i++) {
            int rand = UnityEngine.Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
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
        if (displayQuests == null || displayQuests.Count == 0) return;
        var quest = displayQuests[selectedIndex];
        if (QuestManager.Instance.AcceptQuest(quest)) {
            Debug.Log($"{quest.questName} を受注しました！");
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<PlayerBase>().input.SwitchCurrentActionMap("GamePlay");
            Instantiate(acceptQuestUI, player.transform);
            UI.SetActive(true);
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
        questBoardPanel.SetActive(false);
    }
}
