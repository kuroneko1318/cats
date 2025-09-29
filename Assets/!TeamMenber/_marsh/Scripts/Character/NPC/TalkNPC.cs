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
}
