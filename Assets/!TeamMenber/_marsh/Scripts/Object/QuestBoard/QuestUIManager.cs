using UnityEngine;
using UnityEngine.UI;

public class QuestUIManager : MonoBehaviour {
    [SerializeField] private QuestManager questManager;
    [SerializeField] private Text questText;

    private void Update() {
        if (questManager.activeQuest != null) {
            var q = questManager.activeQuest;
            questText.text = $"{q.questName}\n{q.currentAmount}/{q.requiredAmount}";
        }
        else {
            questText.text = "";
        }
    }
}
