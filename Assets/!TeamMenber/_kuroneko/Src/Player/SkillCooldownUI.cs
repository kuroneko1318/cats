using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// スキルのクールタイムをUIスライダーで表示する
/// </summary>
public class SkillCooldownUI : MonoBehaviour {
    [Header("UIスライダー")]
    [SerializeField] private Slider cooldownSlider; // UIのスライダー

    [Header("対象プレイヤー")]
    [SerializeField] private PlayerBase player;    // プレイヤー参照

    [Header("対象スキル")]
    [SerializeField] private int skillIndex = 0;   // 表示するスキルのインデックス

    private void Update() {
        if (player == null || cooldownSlider == null) return;

        // PlayerBase経由でSkillManagerを取得
        var skillManager = player.GetSkillManager();
        if (skillManager == null) return;

        // 対象スキルの残りクールタイムと最大クールタイムを取得
        float remaining = skillManager.GetRemainingCooldown(skillIndex);
        float max = skillManager.GetCooldownTime(skillIndex);

        // スキル未登録ならゲージをリセット
        if (max <= 0f) {
            cooldownSlider.value = 0f;
            return;
        }

        // 0→1に正規化してスライダーに反映
        float normalized = 1f - (remaining / max);
        cooldownSlider.value = normalized;
    }
}
