using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//基本的にマネージャーはここで管理する
//他のマネージャーで作った内容をここで使う

public class GameManager : MonoBehaviour {

    private GameObject pauseUI;

    public static GameManager Instance { get; private set; }

    public PauseManager pauseManager;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            PauseGame();
        }
    }

    public void PauseGame() {
        if (pauseManager.isPaused)
            pauseManager.Resume(pauseUI);
        else
            pauseManager.Pause(pauseUI);
    }

}
