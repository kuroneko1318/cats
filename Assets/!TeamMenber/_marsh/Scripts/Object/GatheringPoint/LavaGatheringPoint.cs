using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LavaGatheringPoint : GatheringPoint {
    int Itemrand;
    public override void Start() {
        base.Start();
        pointType = GatheringPointType.Lava;
    }

    public override void Gather() {
        Debug.Log("鉱石から鉄鉱石を採取しました！");
        Itemrand = Random.Range(0, 5);
        switch (Itemrand) {
            case 0:
                bag.AddItem(ItemManager.Instance.GetItemByName("石"), Random.Range(1, 5));
                break;
            case 1:
                bag.AddItem(ItemManager.Instance.GetItemByName("鉄"), Random.Range(1, 3));
                break;
            case 2:
                bag.AddItem(ItemManager.Instance.GetItemByName("黒曜石"), Random.Range(1, 2));
                break;
            case 3:
                bag.AddItem(ItemManager.Instance.GetItemByName("ライトストーン"), Random.Range(1, 2));
                break;
            case 4:
                bag.AddItem(ItemManager.Instance.GetItemByName("ブラットストーン"), Random.Range(1, 2));
                break;
        }
        
    }
}