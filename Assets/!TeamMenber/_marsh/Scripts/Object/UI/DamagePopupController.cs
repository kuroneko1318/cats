using UnityEngine;
using TMPro;

//テキストサイズを変えたい場合はDmgPopupUI内のオブジェクトから
//addDamageを呼べばOK

public class DamagePopupController : MonoBehaviour {
    //テキストUI関係
    [SerializeField] private TextMeshPro textMesh;
    [SerializeField] private float disappearDelay = 1f;
    [SerializeField] private float riseSpeed = 1f;
    [SerializeField] private float fadeSpeed = 2f;

    //ダメージ関係の処理
    private int totalDamage = 0;
    private float timer = 0f;
    private bool isDisappearing = false;
    public bool IsFadingOut => isDisappearing;

    //スケール関係
    [SerializeField] private float impactScale = 0.8f;
    [SerializeField] private float scaleDuration = 0.1f;

    private Vector3 originalScale;
    private float scaleTimer = 0f;
    private bool isScaling = false;

    //色関係(クリティカル時)
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color criticalColor = Color.yellow;
    [SerializeField] private float colorFadeDuration = 0.5f;

    private bool isColorFading = false;
    private float colorFadeTimer = 0f;

    private Camera mainCamera;

    private void Start() {
        mainCamera = Camera.main;
    }

    //ダメージを受ける処理
    //damage = ダメージ, isCritical = 会心
    public void AddDamage(int damage, bool isCritical = false) {
        //ダメージの加算
        totalDamage += damage;
        //テキスト表示
        textMesh.text = totalDamage.ToString();
        timer = 0f;
        isDisappearing = false;

        // スケール演出
        originalScale = transform.localScale;
        transform.localScale = originalScale * 0.8f;
        scaleTimer = 0f;
        isScaling = true;

        // 色変化（毎回リセットしてから変更）
        if (isCritical) {
            textMesh.color = normalColor; // ← ここで一旦リセット
            textMesh.color = criticalColor;
            isColorFading = true;
            colorFadeTimer = 0f;
        }
        else {
            textMesh.color = normalColor;
        }
    }

    private void Update() {
        // カメラの方向に向ける（Y軸は固定）
        if (mainCamera != null) {
            Vector3 lookDirection = transform.position - mainCamera.transform.position;
            lookDirection.y = 0f; // Y軸の回転を固定（必要に応じて調整）
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }

        // 以下は既存の処理（フェード、スケール、色変更など）
        timer += Time.deltaTime;

        if (!isDisappearing && timer >= disappearDelay) {
            isDisappearing = true;
            timer = 0f;
        }

        if (isDisappearing) {
            transform.position += Vector3.up * riseSpeed * Time.deltaTime;
            Color c = textMesh.color;
            c.a -= fadeSpeed * Time.deltaTime;
            textMesh.color = c;

            if (c.a <= 0f) {
                Destroy(gameObject);
            }
        }

        if (isScaling) {
            scaleTimer += Time.deltaTime;
            float t = Mathf.Clamp01(scaleTimer / scaleDuration);
            transform.localScale = Vector3.Lerp(originalScale * impactScale, originalScale, t);
            if (t >= 1f) {
                isScaling = false;
            }
        }

        if (isColorFading) {
            colorFadeTimer += Time.deltaTime;
            float t = Mathf.Clamp01(colorFadeTimer / colorFadeDuration);
            textMesh.color = Color.Lerp(criticalColor, normalColor, t);
            if (t >= 1f) {
                isColorFading = false;
            }
        }
    }
}
