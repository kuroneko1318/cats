using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseDefenceBoost : ItemBase {
    public int boostAmount;

    public BaseDefenceBoost(string name, int id, int Amount, Sprite icon = null)
        : base(name, id, eItemType.Boost, icon) {
        this.boostAmount = Amount;
    }

    public override void Use(GameObject user) {
        PlayerBase player = user.GetComponent<PlayerBase>();
        if (player != null) {
            player.BaseDefenceBoost(boostAmount);
            Debug.Log($"{itemName} を使って DEF を {boostAmount} ブースト！");
        }
    }
}
