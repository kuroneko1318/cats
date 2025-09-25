using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//shimura is draw
public class CactusGatheringPoint: GatheringPoint {
    private int Itemrand;
    public override void Start() {
        base.Start();
        pointType = GatheringPointType.Cactus;
    }

    public override void Gather() {
        Debug.Log("サボテンを採取しました！");
        Itemrand = Random.Range(0, 4);
        switch (Itemrand) {
            case 0:
                bag.AddItem(ItemManager.Instance.GetItemByName("薬草"), Random.Range(1, 5));
                break;
            case 1:
                bag.AddItem(ItemManager.Instance.GetItemByName("紐"), Random.Range(1, 3));
                break;
            case 2:
                bag.AddItem(ItemManager.Instance.GetItemByName("木の実"), Random.Range(1, 2));
                break;
            case 3:
                bag.AddItem(ItemManager.Instance.GetItemByName("木"), Random.Range(2, 5));
                break;
        }
    }
}