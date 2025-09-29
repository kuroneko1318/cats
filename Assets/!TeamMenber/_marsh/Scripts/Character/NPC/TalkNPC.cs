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
        Text.GetComponent<TextMeshPro>().text = ("–‚Î‚ğg‚¤‚Æ‘•”õ‚ğ‹­‰»‚Å‚«‚é‚ç‚µ‚¢");
        Text.SetActive(true);
    }

    private void OnTriggerExit(Collider other) {
        Text.SetActive(false);
    }

    private void RandomTalkText() {
        int rand = Random.Range(0, 5);

        switch (rand) {
            case 0:
                break; case 1:
                break; case 2:
                break;case 3:
                break;
        }
    }

}
