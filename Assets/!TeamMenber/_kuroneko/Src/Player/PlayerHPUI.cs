using UnityEngine;
using UnityEngine.UI;

public class PlayerHPSliderUI : MonoBehaviour {
    [SerializeField] private PlayerBase player;     // プレイヤー参照
    [SerializeField] private Slider immediateHP;    // 即時反映スライダー
    [SerializeField] private Slider delayedHP;      // 遅れて減るスライダー
    [SerializeField] private float delaySpeed = 0.5f; // 遅れて減る速度

    void Start() {
        // 最大値を設定
        immediateHP.maxValue = player.maxHp;
        delayedHP.maxValue = player.maxHp;

        // 初期値設定
        immediateHP.value = player.hp;
        delayedHP.value = player.hp;
    }

    void Update() {
        // 即時HPはプレイヤーHPに合わせる
        immediateHP.value = player.hp;

        // 遅延HPは徐々に追従
        if (delayedHP.value > player.hp) {
            delayedHP.value -= delaySpeed * Time.deltaTime * player.maxHp;
            if (delayedHP.value < player.hp)
                delayedHP.value = player.hp;
        }
        else {
            delayedHP.value = player.hp;
        }
    }
}
