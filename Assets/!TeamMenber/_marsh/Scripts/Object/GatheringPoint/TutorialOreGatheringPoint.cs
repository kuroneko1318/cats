using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialOreGatheringPoint : GatheringPoint {
    int ItemCount = -1;
    public override void Start() {
        base.Start();
        pointType = GatheringPointType.TutorialOre;
    }

    public override void Gather() {
        Debug.Log("zÎ‚©‚ç“SzÎ‚ðÌŽæ‚µ‚Ü‚µ‚½I");


        ItemCount++;
        switch (ItemCount) {
            case 0:
                bag.AddItem(ItemManager.Instance.GetItemByName("Î"), Random.Range(2, 5));
                break;
            case 1:
                bag.AddItem(ItemManager.Instance.GetItemByName("“S"), Random.Range(2, 5));
                break;
            case 2:
                bag.AddItem(ItemManager.Instance.GetItemByName("“S"), Random.Range(2, 4));
                break;
        }

        if (ItemCount >= 2) ItemCount = -1;


    }
}
