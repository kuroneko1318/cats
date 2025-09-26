using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CleaFadeText : MonoBehaviour {

    public TextMeshProUGUI messageText; // 表示するテキスト
    public float displayDuration = 3f;  // 表示時間（秒）

    void Start() {
        ShowMessage("このメッセージは3秒後に消えます");
    }

    public void ShowMessage(string message) {
        messageText.text = message;
        messageText.gameObject.SetActive(true);
        StartCoroutine(HideAfterDelay(displayDuration));
    }

    private System.Collections.IEnumerator HideAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        messageText.gameObject.SetActive(false);
    }

}
