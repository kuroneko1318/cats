using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushGatheringPoint : GatheringPoint {
    private void Awake() {
        pointType = GatheringPointType.Bush;
    }

    protected override void Gather() {
        Debug.Log("茂みから薬草を採取しました！");
        // アイテム追加処理など
    }
}

