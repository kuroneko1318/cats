using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private StageManager stageManager;

    [SerializeField]
    private GameObject TutorialUI;

    // Start is called before the first frame update
    void Start()
    {
        stageManager = FindObjectOfType<StageManager>();
    }

    private void OnTriggerEnter(Collider other) {
        // プレイヤー以外なら無視
        if (!other.CompareTag("Player")) return;

        stageManager?.ReturnToBase();
        TutorialUI.SetActive(false);

    }

}
