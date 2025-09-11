using System.Collections.Generic;
using UnityEngine;

public class QuestBoard : MonoBehaviour {
    [SerializeField] private List<HuntQuest> allQuests; // 全クエスト
    [SerializeField] private int displayCount = 6;
    [SerializeField] private QuestManager questManager;

    private List<HuntQuest> displayQuests = new List<HuntQuest>();

    public void RefreshBoard() {
        displayQuests.Clear();
        var candidates = new List<HuntQuest>(allQuests);
        // クリア済みを除外
        candidates.RemoveAll(q => q.isCleared);

        for (int i = 0; i < displayCount && candidates.Count > 0; i++) {
            int index = Random.Range(0, candidates.Count);
            displayQuests.Add(candidates[index]);
            candidates.RemoveAt(index);
        }

        // UIに反映
        UpdateUI();
    }

    public void AcceptQuest(HuntQuest quest) {
        bool accepted = questManager.AcceptQuest(quest);
        if (accepted) {
            displayQuests.Remove(quest);
            UpdateUI();
        }
        else {
            Debug.Log("すでにクエスト受注中です");
        }
    }


    private void UpdateUI() {
        // TODO: 左上やボードUIに反映する処理
    }
}
