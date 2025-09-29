using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TalkNPC : MonoBehaviour {

    public GameObject Text;

    private void Start() {
        Text.SetActive(false);
    }

    private void OnTriggerEnter(Collider other) {
        RandomTalkText();
        Text.SetActive(true);
    }

    private void OnTriggerExit(Collider other) {
        Text.SetActive(false);
    }

    private void RandomTalkText() {
        int rand = Random.Range(0, 6);

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
        }
    }

}
