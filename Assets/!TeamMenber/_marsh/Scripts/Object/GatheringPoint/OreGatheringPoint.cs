using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class OreGatheringPoint : GatheringPoint {

    Inventory bag;

    private void Awake() {
        pointType = GatheringPointType.Ore;
    }

    protected override void Gather() {
        Debug.Log("zÎ‚©‚ç“SzÎ‚ğÌæ‚µ‚Ü‚µ‚½I");
        //bag.AddItem()
    }

    private void Update() {
        if(bag == null) {
            bag = GameObject.Find("inventory").GetComponent<Inventory>();
        }
    }
}

