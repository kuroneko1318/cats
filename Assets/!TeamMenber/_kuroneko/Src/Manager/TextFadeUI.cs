using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIFader : MonoBehaviour {
    [Header("対象UI")]
    [SerializeField] private TextMeshProUGUI text;  // フェードさせるテキスト
    [SerializeField] private Image background;      // フェードさせる背景（任意）

    [Header("設定")]
    [SerializeField] private float fadeTime = 1.5f; // フェードイン・アウトにかかる時間
    [SerializeField] private float stayTime = 1.0f; // 最大表示状態で待機する時間
    [Range(0f, 1f)]
    [SerializeField] private float backgroundMaxAlpha = 0.5f; // 背景の最大Alpha（0～1）

    private void Start() {
        StartCoroutine(FadeInOutRoutine());
    }

    /// <summary>
    /// フェードイン → 一定時間表示 → フェードアウト をまとめて実行
    /// </summary>
    public void PlayFadeInOut() {
        StartCoroutine(FadeInOutRoutine());
    }

    private IEnumerator FadeInOutRoutine() {
        // フェードイン
        yield return StartCoroutine(Fade(0f, 1f));
        // 最大表示で待機
        yield return new WaitForSeconds(stayTime);
        // フェードアウト
        yield return StartCoroutine(Fade(1f, 0f));

        Destroy(gameObject);
    }

    /// <summary>
    /// テキストと背景を同時にフェード（背景がnullでも安全）
    /// </summary>
    private IEnumerator Fade(float from, float to) {
        float time = 0f;

        // nullチェックして元の色を保持
        Color textBase = text != null ? text.color : Color.white;
        Color bgBase = background != null ? background.color : Color.white;

        while (time < fadeTime) {
            time += Time.deltaTime;
            float t = time / fadeTime;

            float alpha = Mathf.Lerp(from, to, t);

            if (text != null) {
                // テキストは常に 0～1 の範囲で
                text.color = new Color(textBase.r, textBase.g, textBase.b, alpha);
            }
            if (background != null) {
                // 背景は 0～backgroundMaxAlpha の範囲で
                float bgAlpha = alpha * backgroundMaxAlpha;
                background.color = new Color(bgBase.r, bgBase.g, bgBase.b, bgAlpha);
            }

            yield return null;
        }

        // 最終値を固定
        if (text != null) {
            text.color = new Color(textBase.r, textBase.g, textBase.b, to);
        }
        if (background != null) {
            float bgAlpha = to * backgroundMaxAlpha;
            background.color = new Color(bgBase.r, bgBase.g, bgBase.b, bgAlpha);
        }
    }
}
