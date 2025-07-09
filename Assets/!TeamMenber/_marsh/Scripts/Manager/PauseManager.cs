using UnityEngine;
using UnityEngine.SceneManagement;

//ポーズメニュー表示用
//TimeScaleを使うことで時間を止める

public class PauseManager : MonoBehaviour {
    public bool isPaused = false;


    public void Resume(GameObject obj) {
        obj.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Pause(GameObject obj) {
        obj.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void QuitToTitle() {
        Time.timeScale = 1f;
        SceneManager.LoadScene("TitleScene");
    }
}
