using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OreGatheringPoint : GatheringPoint {

    public override void Awake() {
        base.Awake();
        pointType = GatheringPointType.Ore;
    }

    public override void Gather() {
        Debug.Log("zÎ‚©‚ç“SzÎ‚ğÌæ‚µ‚Ü‚µ‚½I");
        bag.AddItem(ItemManager.Instance.GetItemByName("Î"), Random.Range(1,5));
        bag.AddItem(ItemManager.Instance.GetItemByName("“S"), Random.Range(1,3));
        bag.AddItem(ItemManager.Instance.GetItemByName("“º"), Random.Range(1,2));
    }
}

