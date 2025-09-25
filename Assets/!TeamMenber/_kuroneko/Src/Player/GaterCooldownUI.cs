using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// スキルのクールタイムをUIスライダーで表示する
/// </summary>
public class GaterCooldownUI : MonoBehaviour {
    [Header("UIスライダー")]
    [SerializeField] private Slider cooldownSlider; // UIのスライダー

    [Header("対象プレイヤー")]
    //[SerializeField] private PlayerBase player;    // プレイヤー参照

    [Header("対象スキル")]
    //[SerializeField] private int skillIndex = 0;   // 表示するスキルのインデックス

    [Header("表示非表示用")]
    [SerializeField] private GameObject skillCooldownUI;


    float max=15;
    private void Update() {
       
        var gater=GetComponent<GatheringPoint>();

        float remaining = gater.respawnTime;

        // 0→1に正規化してスライダーに反映
        float normalized = 1f - (remaining / max);
        cooldownSlider.value = normalized;
    }

    public void ShowUI() {
        skillCooldownUI.SetActive(true);
    }

    public void HideUI() {
        skillCooldownUI.SetActive(false);
    }

}
