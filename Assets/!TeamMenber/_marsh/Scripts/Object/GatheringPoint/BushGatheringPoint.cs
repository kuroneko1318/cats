using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushGatheringPoint : GatheringPoint {

    public override void Start() {
        base.Start();
        pointType = GatheringPointType.Bush;
    }

    public override void Gather() {
        Debug.Log("茂みから薬草を採取しました！");
        // アイテム追加処理など
        bag.AddItem(ItemManager.Instance.GetItemByName("薬草"), Random.Range(1, 5));
        bag.AddItem(ItemManager.Instance.GetItemByName("紐"), Random.Range(1, 3));
        bag.AddItem(ItemManager.Instance.GetItemByName("木の実"), Random.Range(1, 2));
        bag.AddItem(ItemManager.Instance.GetItemByName("木"), Random.Range(2, 5));
    }
}

