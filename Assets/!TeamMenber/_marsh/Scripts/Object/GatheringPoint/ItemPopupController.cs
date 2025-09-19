
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemPopupController : MonoBehaviour {
    //テキストUI関係
    [SerializeField] private TextMeshPro textMesh;
    [SerializeField] private float disappearDelay = 1f;
    [SerializeField] private float riseSpeed = 1f;
    [SerializeField] private float fadeSpeed = 2f;
    private float timer = 0f;
    private bool isDisappearing = false;
public bool IsFadingOut => isDisappearing;
    private Camera mainCamera;

    private void Start() {
        mainCamera = Camera.main;
    }

    public void GetItemUI(string itemName,int amount) {

        //テキスト表示
        textMesh.text = itemName+" × "+amount;
        timer = 0f;
        isDisappearing = false;
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

       
    }
}

