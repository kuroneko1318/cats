using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreGatheringPoint : GatheringPoint {
    private void Awake() {
        pointType = GatheringPointType.Ore;
    }

    protected override void Gather() {
        Debug.Log("zÎ‚©‚ç“SzÎ‚ğÌæ‚µ‚Ü‚µ‚½I");
        // ƒAƒCƒeƒ€’Ç‰Áˆ—‚È‚Ç
    }
}

