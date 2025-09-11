using TMPro;
using UnityEngine;

public class QuestProgressUI : MonoBehaviour {
    public TextMeshProUGUI progressText;

    void Update() {
        var quest = QuestManager.Instance.activeQuest;
        if (quest != null) {
            progressText.text = $"{quest.questName} : {quest.currentAmount}/{quest.requiredAmount}";
        }
        else {
            progressText.text = "受注中のクエストなし";
        }
    }
}

