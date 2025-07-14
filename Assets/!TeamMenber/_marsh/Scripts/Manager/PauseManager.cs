using UnityEngine;
using UnityEngine.SceneManagement;

//ポーズメニュー表示用
//TimeScaleを使うことで時間を止める

public class PauseManager : SystemObject<PauseManager> {
    public bool isPaused = false;
    private GameObject pauseUI;

    public void Resume() {
        pauseUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Pause() {
        pauseUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void QuitToTitle() {
        Time.timeScale = 1f;
        SceneManager.LoadScene("TitleScene");
    }
}
