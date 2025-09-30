using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// スキルのクールタイムをUIスライダーで表示する
/// </summary>
public class GaterCooldownUI : MonoBehaviour {


    [Header("表示非表示用")]
    [SerializeField] private GameObject gatherCooldownUI;

    public void ShowUI() {
        gatherCooldownUI.SetActive(true);
    }

    public void HideUI() {
        gatherCooldownUI.SetActive(false);
    }

}
