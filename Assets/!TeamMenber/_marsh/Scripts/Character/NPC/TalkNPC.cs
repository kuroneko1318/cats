using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TalkNPC : MonoBehaviour {

    public GameObject Text;

    private Animator animator;

    public static TalkNPC Instance;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        Text.SetActive(false);
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other) {
        RandomTalkText();
        Text.SetActive(true);
    }

    private void OnTriggerExit(Collider other) {
        Text.SetActive(false);
    }

    public void ChangeAnimation() {
        animator.SetBool("Clap",true);
        Text.GetComponent<TextMeshPro>().text = ("おめでとう！\n君は英雄だ！");
    }

    private void RandomTalkText() {
        int rand = Random.Range(0, 7);

        switch (rand) {
            case 0:
                Text.GetComponent<TextMeshPro>().text = ("魔石を使うと\n装備を強化できるらしい");
                break; 
            case 1:
                Text.GetComponent<TextMeshPro>().text = ("紐を使えば薬も服も作れるぞ");
                break; 
            case 2:
                Text.GetComponent<TextMeshPro>().text = ("木の実食べると\nなんか力が湧いてくるな");
                break; 
            case 3:
                Text.GetComponent<TextMeshPro>().text = ("黒曜石を混ぜて\n飲むなんて正気か？");
                break;
            case 4:
                Text.GetComponent<TextMeshPro>().text = ("変わった石を組み合わせると\n強そうな石が出来そうだな");
                break;
            case 5:
                Text.GetComponent<TextMeshPro>().text = ("溶岩はイカれた強さしている\n奴らが沢山湧いてやがる");
                break;
            case 6:
                Text.GetComponent<TextMeshPro>().text = ("剣を作るには\n木を棒に加工しないといけないらしいぞ");
                break;
        }
    }

}
