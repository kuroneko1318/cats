using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CactusGather : GatheringPoint {

    private void Awake() {
        pointType = GatheringPointType.saboten;
    }

    private void Start() {
        if (bag == null) {
            bag = GameObject.FindGameObjectWithTag("bag")?.GetComponent<Inventory>();
        }
    }

    public override void Gather() {
        Debug.Log("サボテンを採取しました！");
        bag.AddItem(ItemManager.Instance.GetItemByName("薬草"), Random.Range(1, 5));
        bag.AddItem(ItemManager.Instance.GetItemByName("紐"), Random.Range(1, 3));
        bag.AddItem(ItemManager.Instance.GetItemByName("針"), Random.Range(2, 5));
    }

    //private void Update() {
    //    if(bag == null) {
    //        bag = GameObject.FindGameObjectWithTag("bag")?.GetComponent<Inventory>();
    //    }
    //}
}