using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshProを使う場合
using System.Collections;

public class MainGameEnd : MonoBehaviour {
    public CanvasGroup fadePanel;      // 黒背景用
    //public CanvasGroup textGroup;      // テキスト用
    public float fadeDuration = 2f;    // フェード時間
    public float textDisplayTime = 3f; // テキスト表示時間

    void Start() {
        StartCoroutine(PlayIntroSequence());
    }

    IEnumerator PlayIntroSequence() {
        // ① 黒背景フェードアウト
        //yield return StartCoroutine(FadeCanvasGroup(textGroup, 0f, 1f, fadeDuration));
        yield return StartCoroutine(FadeCanvasGroup(fadePanel, 0f, 1f, fadeDuration));
        //yield return StartCoroutine(FadeCanvasGroup(textGroup, 0f, 1f, fadeDuration));
        // ② テキストフェードイン
        //yield return StartCoroutine(FadeCanvasGroup(textGroup, 0f, 1f, fadeDuration));

        // ③ 表示時間待機
        yield return new WaitForSeconds(textDisplayTime);
        yield return StartCoroutine(FadeCanvasGroup(fadePanel, 1f, 0f, fadeDuration));
        // ④ テキストフェードアウト
        //yield return StartCoroutine(FadeCanvasGroup(textGroup, 1f, 0f, fadeDuration));
    }

    IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration) {
        float elapsed = 0f;
        while (elapsed < duration) {
            cg.alpha = Mathf.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cg.alpha = end;
    }
}