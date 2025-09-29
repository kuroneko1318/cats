using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// スキルのクールタイムをUIスライダーで表示する
/// </summary>
public class GaterCooldownUI : MonoBehaviour {


    [Header("表示非表示用")]
    [SerializeField] private GameObject skillCooldownUI;

    private void Start() {
        var gater=GetComponent<GatheringPoint>();
    }

    public void ShowUI() {
        skillCooldownUI.SetActive(true);
    }

    public void HideUI() {
        skillCooldownUI.SetActive(false);
    }

}
