using TMPro;
using UnityEngine;

public class QuestProgressUI : MonoBehaviour {
    public TextMeshProUGUI progressText;
    private QuestManager qm;

    void Update() {
        if (qm == null) {
            qm = QuestManager.Instance;
        }

        if (qm != null && qm.activeQuest != null) {
            progressText.text =
                $"{qm.activeQuest.questName} : {qm.activeQuest.currentAmount}/{qm.activeQuest.requiredAmount}";
        }
        else {
            progressText.text = "受注中のクエストなし";
        }
    }
}

