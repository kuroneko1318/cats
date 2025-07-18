using UnityEngine;
using TMPro;
using System.Collections;

public class DamagePopupDigit : MonoBehaviour {
    private TextMeshPro textMesh;
    private float fallSpeed = 2f;
    private float fadeSpeed = 2f;
    private float riseSpeed = 1f;
    private float delay;
    private bool startFade = false;
    private Color originalColor;

    public void Setup(char digit, float delayTime) {
        textMesh = GetComponent<TextMeshPro>();
        textMesh.text = digit.ToString();
        originalColor = textMesh.color;
        delay = delayTime;
        StartCoroutine(FallThenFade());
    }

    private IEnumerator FallThenFade() {
        yield return new WaitForSeconds(delay);

        float fallDuration = 0.3f;
        float elapsed = 0f;

        while (elapsed < fallDuration) {
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }

        startFade = true;
    }

    private void Update() {
        if (startFade) {
            transform.position += Vector3.up * riseSpeed * Time.deltaTime;
            Color c = textMesh.color;
            c.a -= fadeSpeed * Time.deltaTime;
            textMesh.color = c;

            if (c.a <= 0f) {
                Destroy(gameObject);
            }
        }
    }
}
