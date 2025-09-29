using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour {
    [Header("ロード画面シーン名")]
    [SerializeField] private string loadingSceneName = "LoadingScene";

    [Header("移動対象オブジェクト")]
    [SerializeField] private GameObject movingObject;

    [Header("フェード用Image（黒背景）")]
    [SerializeField] private Image fadeImage;

    [Header("移動設定")]
    [SerializeField] private Vector3 moveOffset = new Vector3(0, 3, 0); // どれくらい動かすか
    [SerializeField] private float moveDuration = 2f; // 移動時間

    [Header("フェード設定")]
    [SerializeField] private float fadeDuration = 2f; // フェード時間

    private PlayerInput playerInput;
    private InputAction selectAction;

    private bool isLoading = false;
    private Animator animator;

    void Awake() {
        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null) {
            Debug.LogError("PlayerInput が見つかりません。TitleManagerと同じオブジェクトに追加してください。");
            return;
        }
        selectAction = playerInput.actions["Select"];
        animator = movingObject.GetComponent<Animator>();

        if (fadeImage != null) {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c; // 初期は透明
        }
    }

    void OnEnable() {
        if (selectAction != null)
            selectAction.performed += OnSelect;
    }

    void OnDisable() {
        if (selectAction != null)
            selectAction.performed -= OnSelect;
    }

    private void OnSelect(InputAction.CallbackContext context) {
        if (isLoading) return;
        isLoading = true;

        AudioManager.Instance.PlaySE("FirstAttack");
        animator.SetTrigger("Walk");

        Debug.Log("タイトル画面で決定ボタン → 移動しながらフェードアウト開始");

        StartCoroutine(PlayTransition());
    }

    private IEnumerator PlayTransition() {
        Vector3 startPos = movingObject.transform.position;
        Vector3 endPos = startPos + moveOffset;

        float timer = 0f;

        // 移動＋フェードアウトを同時進行
        while (timer < moveDuration) {
            timer += Time.deltaTime;
            float t = timer / moveDuration;

            // 移動
            movingObject.transform.position = Vector3.Lerp(startPos, endPos, t);

            // フェード（0→1）
            if (fadeImage != null) {
                Color c = fadeImage.color;
                c.a = Mathf.Lerp(0f, 1f, t);
                fadeImage.color = c;
            }

            yield return null;
        }

        // 念のため最終位置 & フェードを固定
        movingObject.transform.position = endPos;
        if (fadeImage != null) {
            Color c = fadeImage.color;
            c.a = 1f;
            fadeImage.color = c;
        }

        // 少し待機してからシーン遷移
        yield return new WaitForSeconds(0.5f);

        if (!string.IsNullOrEmpty(loadingSceneName)) {
            SceneManager.LoadScene(loadingSceneName);
        }
    }
}
