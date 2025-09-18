using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OreGatheringPoint : GatheringPoint {

    private void Awake() {
        pointType = GatheringPointType.Ore;
    }

    private void Start() {
        if (bag == null) {
            bag = GameObject.FindGameObjectWithTag("bag")?.GetComponent<Inventory>();
        }
    }

    public override void Gather() {
        Debug.Log("zÎ‚©‚ç“SzÎ‚ğÌæ‚µ‚Ü‚µ‚½I");
        bag.AddItem(ItemManager.Instance.GetItemByName("Î"), Random.Range(1,5));
        bag.AddItem(ItemManager.Instance.GetItemByName("“S"), Random.Range(1,3));
        bag.AddItem(ItemManager.Instance.GetItemByName("“º"), Random.Range(0,2));
    }

    private void Update() {
        if(bag == null) {
            bag = GameObject.FindGameObjectWithTag("bag")?.GetComponent<Inventory>();
        }
    }
}

