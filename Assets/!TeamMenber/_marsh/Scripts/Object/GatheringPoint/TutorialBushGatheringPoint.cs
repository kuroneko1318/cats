using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBushGatheringPoint : GatheringPoint
{
    int ItemCount = -1;
    public override void Start() {
        base.Start();
        pointType = GatheringPointType.TutorialOre;
    }

    public override void Gather() {
        Debug.Log("");


        ItemCount++;
        switch (ItemCount) {
            case 0:
                bag.AddItem(ItemManager.Instance.GetItemByName("–ò‘"), Random.Range(2, 5));
                break;
            case 1:
                bag.AddItem(ItemManager.Instance.GetItemByName("•R"), Random.Range(3, 5));
                break;
            case 2:
                bag.AddItem(ItemManager.Instance.GetItemByName("–Ø"), Random.Range(3, 6));
                break;
        }

        if (ItemCount >= 2) ItemCount = -1;


    }
}
