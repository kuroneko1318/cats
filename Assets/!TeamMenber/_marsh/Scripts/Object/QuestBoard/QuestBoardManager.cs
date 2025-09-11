using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestBoardManager : MonoBehaviour {
    [Header("UI")]
    public GameObject questPrefab;      // 作ったQuestPrefab
    public Transform contentParent;     // GridLayoutGroupがついた親（横3×縦2）

    [Header("クエストデータ")]
    public HuntQuest[] allQuests;      // 全クエスト（最大6個）

    private void Start() {
        PopulateQuestBoard();
    }

    public void PopulateQuestBoard() {
        // 既存の子を削除
        foreach (Transform child in contentParent) {
            Destroy(child.gameObject);
        }

        foreach (var quest in allQuests) {
            GameObject obj = Instantiate(questPrefab, contentParent);

            // 名前
            obj.transform.Find("QuestType").GetComponent<TextMeshProUGUI>().text = quest.questName;
            // 倒す対象と数
            obj.transform.Find("TargetAmount").GetComponent<TextMeshProUGUI>().text =
                $"{quest.targetEnemyType} {quest.requiredAmount}体";
            // 報酬
            obj.transform.Find("Reward").GetComponent<TextMeshProUGUI>().text =
                $"{quest.rewardMoney}G";
        }
    }
}
