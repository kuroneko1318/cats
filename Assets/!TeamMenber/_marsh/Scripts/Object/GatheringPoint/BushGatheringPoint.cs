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
        bag.AddItem(ItemManager.Instance.GetItemByName("薬草"), Random.Range(0, 5));
        bag.AddItem(ItemManager.Instance.GetItemByName("紐"), Random.Range(0, 3));
        bag.AddItem(ItemManager.Instance.GetItemByName("木の実"), Random.Range(0, 2));
    }

    private void Update() {
        if (bag == null) {
            bag = GameObject.Find("inventory").GetComponent<Inventory>();
        }
    }
}

