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
        Debug.Log("zÎ‚©‚ç“SzÎ‚ğÌæ‚µ‚Ü‚µ‚½I");
        Itemrand = Random.Range(0, 3);
        switch (Itemrand) {
            case 0:
                bag.AddItem(ItemManager.Instance.GetItemByName("Î"), Random.Range(1, 5));
                break;
            case 1:
                bag.AddItem(ItemManager.Instance.GetItemByName("“S"), Random.Range(1, 3));
                break;
            case 2:
                bag.AddItem(ItemManager.Instance.GetItemByName("•—jÎ"), Random.Range(1, 2));
                break;
        }
        
    }
}