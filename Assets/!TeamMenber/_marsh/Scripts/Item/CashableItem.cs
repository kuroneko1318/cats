using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashableItem : ItemBase {
    public int amount;

    public CashableItem(string name, int id, int amount, Sprite icon = null)
        : base(name, id, eItemType.Money, icon) {
        this.amount = amount;
    }

    public override void Use(GameObject user) {
        PlayerBase player = user.GetComponent<PlayerBase>();
        if (player != null) {
            player.AddCash(amount);
            Debug.Log($"{itemName} を使って DEF を {amount} ブースト！");
        }
    }
}
