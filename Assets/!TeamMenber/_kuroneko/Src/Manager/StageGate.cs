using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class StageGate : MonoBehaviour {
    public bool goToArea2 = true; // trueならエリア2へ、falseならエリア1へ

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) return;

        StageManager sm = FindObjectOfType<StageManager>();
        if (sm == null) return;

        if (goToArea2) sm.EnterArea2();
        else sm.ReturnToArea1();
    }
}
