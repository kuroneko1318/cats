using UnityEngine;

public class QuestManager : MonoBehaviour {
    public static QuestManager Instance { get; private set; }

    public HuntQuest activeQuest;   // 今進行中のクエスト
    public int playerMoney;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    // クエスト受注
    public bool AcceptQuest(HuntQuest quest) {
        if (activeQuest == null && !quest.isCleared) {
            quest.status = QuestStatus.InProgress;
            quest.currentAmount = 0;
            activeQuest = quest;
            return true;
        }
        return false; // 既に受注中
    }

    // 敵撃破処理
    public void EnemyDefeated(EnemyType enemyName) {
        if (activeQuest != null &&
            activeQuest.status == QuestStatus.InProgress &&
            activeQuest.targetEnemyType == enemyName) {

            activeQuest.currentAmount++;
            if (activeQuest.currentAmount >= activeQuest.requiredAmount) {
                CompleteQuest();
            }
        }
    }

    // クエスト達成処理
    private void CompleteQuest() {
        activeQuest.status = QuestStatus.Completed;
        playerMoney += activeQuest.rewardMoney;
        activeQuest.isCleared = true;
        activeQuest = null; // 空にして次を受注可能にする
    }
}
