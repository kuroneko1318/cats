using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAttackBoost : ItemBase {
    public int boostAmount;

    public BaseAttackBoost(string name, int id, int amount, Sprite icon = null)
        : base(name, id, eItemType.Boost, icon) {
        this.boostAmount = amount;
    }

    public override void Use(GameObject user) {
        PlayerBase player = user.GetComponent<PlayerBase>();
        if (player != null) {
            player.BaseAttackBoost(boostAmount);
            Debug.Log($"{itemName} を使って ATK を {boostAmount} ブースト！");
        }
    }
}
