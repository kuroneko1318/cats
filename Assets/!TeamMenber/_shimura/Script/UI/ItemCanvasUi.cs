using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class ItemCanvasUi : MonoBehaviour
{
   [SerializeField] private TextMeshPro textMesh;
    [SerializeField] private float disappearDelay = 1f;
    [SerializeField] private float riseSpeed = 1f;
    [SerializeField] private float fadeSpeed = 2f;
    private float timer = 0f;
    private bool isDisappearing = false;
    public bool IsFadingOut => isDisappearing;
    public void GetItemUI(string itemName, int amount) {

        //テキスト表示
        textMesh.text = itemName + " × " + amount;
    }

    private void Update() {
        

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


    }
}
