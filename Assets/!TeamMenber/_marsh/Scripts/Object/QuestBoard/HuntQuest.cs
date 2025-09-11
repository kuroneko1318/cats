using System.Collections.Generic;
using UnityEngine;

public enum QuestStatus {
    NotAccepted,
    InProgress,
    Completed,
    Cleared
}

[System.Serializable]
public class HuntTarget {
    public EnemyType enemyType;
    public int requiredAmount;
    [HideInInspector] public int currentAmount;
}

[CreateAssetMenu(menuName = "Quest/HuntQuest")]
public class HuntQuest : ScriptableObject {
    public string questName;
    public EnemyType targetEnemyType;
    public int requiredAmount;
    public int rewardMoney;

    [HideInInspector] public int currentAmount;
    [HideInInspector] public QuestStatus status;
    [HideInInspector] public bool isCleared = false;
}