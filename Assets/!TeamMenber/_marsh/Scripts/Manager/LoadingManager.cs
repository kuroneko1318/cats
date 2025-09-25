using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour {
    [Header("ロード先シーン名")]
    [SerializeField] private string targetSceneName = "GameScene";

    [Header("UI")]
    [SerializeField] private Slider progressBar;      // プログレスバー

    void Start() {
        if (!string.IsNullOrEmpty(targetSceneName)) {
            StartCoroutine(LoadSceneAsyncWithLoading(targetSceneName));
        }
    }

    private IEnumerator LoadSceneAsyncWithLoading(string sceneName) {

        // 非同期ロード開始
        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName);
        asyncOp.allowSceneActivation = false;

        // プログレスバー更新
        while (asyncOp.progress < 0.9f) {
            if (progressBar != null)
                progressBar.value = asyncOp.progress;
            yield return null;
        }

        // 最終調整（0.9～1.0）
        yield return new WaitForSeconds(0.3f);

        // シーン遷移許可
        asyncOp.allowSceneActivation = true;

        // 遷移完了まで待機
        while (!asyncOp.isDone) yield return null;
    }
}
