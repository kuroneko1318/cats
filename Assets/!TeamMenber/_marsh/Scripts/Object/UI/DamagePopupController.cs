using UnityEngine;
using TMPro;

public class DamagePopupController : MonoBehaviour {
    [SerializeField] private TextMeshPro textMesh;
    [SerializeField] private float disappearDelay = 1f;
    [SerializeField] private float riseSpeed = 1f;
    [SerializeField] private float fadeSpeed = 2f;

    private int totalDamage = 0;
    private float timer = 0f;
    private bool isDisappearing = false;
    public bool IsFadingOut => isDisappearing;


    public void AddDamage(int damage) {
        totalDamage += damage;
        textMesh.text = totalDamage.ToString();
        timer = 0f;
        isDisappearing = false;
        Color c = textMesh.color;
        c.a = 1f;
        textMesh.color = c;
    }

    private void Update() {
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
    }
}
