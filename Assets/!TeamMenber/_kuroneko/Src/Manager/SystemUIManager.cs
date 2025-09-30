using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SystemUIManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI goldText;
    [SerializeField]
    private TextMeshProUGUI lapsText;

    // Update is called once per frame
    void Update()
    {
        goldText.GetComponent<TextMeshProUGUI>().text = (QuestManager.Instance.playerMoney + "G");
        lapsText.GetComponent<TextMeshProUGUI>().text = (GameManager.Instance.laps + "Žü–Ú");
    }
}
