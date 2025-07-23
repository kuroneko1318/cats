using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;


public class OreGatheringPoint : GatheringPoint {

    private void Awake() {
        pointType = GatheringPointType.Ore;
    }

    protected override void Gather() {
        Debug.Log("zÎ‚©‚ç“SzÎ‚ğÌæ‚µ‚Ü‚µ‚½I");
        bag.AddItem(ItemManager.Instance.GetItemByName("ÎA"), Random.Range(0,5));
        bag.AddItem(ItemManager.Instance.GetItemByName("“S"), Random.Range(0,3));
        bag.AddItem(ItemManager.Instance.GetItemByName("“º"), Random.Range(0,2));
    }

    private void Update() {
        if(bag == null) {
            bag = GameObject.Find("inventory").GetComponent<Inventory>();
        }
    }
}

