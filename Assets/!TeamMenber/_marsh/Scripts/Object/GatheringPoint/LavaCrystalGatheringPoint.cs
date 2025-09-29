using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LavaCrystalGatheringPoint : GatheringPoint {
    int Itemrand;
    public override void Start() {
        base.Start();
        pointType = GatheringPointType.LavaCrystal;
    }

    public override void Gather() {
        Debug.Log("鉱石から鉄鉱石を採取しました！");
        Itemrand = Random.Range(0, 5);
        switch (Itemrand) {
            case 0:
                bag.AddItem(ItemManager.Instance.GetItemByName("ブラッドストーン"), Random.Range(1, 2));
                break;
            case 1:
                bag.AddItem(ItemManager.Instance.GetItemByName("鉄"), Random.Range(1, 3));
                break;
            case 2:
                bag.AddItem(ItemManager.Instance.GetItemByName("ライトストーン"), Random.Range(1, 2));
                break;
        }

    }
}