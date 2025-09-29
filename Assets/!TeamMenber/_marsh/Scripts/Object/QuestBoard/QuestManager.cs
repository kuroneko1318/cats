using UnityEngine;

public class QuestManager : SystemObject<QuestManager> {

    public HuntQuest activeQuest;   // 今進行中のクエスト
    public int playerMoney;

    // クエスト受注
    public bool AcceptQuest(HuntQuest quest) {
        if (activeQuest == null) {
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
        InventoryManager.Instance.bag.AddItem(ItemManager.Instance.GetItemByName("回復薬"), Random.Range(1, 5));
        activeQuest = null; // 空にして次を受注可能にする
        QuestBoardManager.Instance.UI.SetActive(false);
        QuestBoardManager.Instance.PopulateQuestBoard();
        GameManager.Instance.QuestClear();
        AudioManager.Instance.PlaySE("Quest");
    }
}
