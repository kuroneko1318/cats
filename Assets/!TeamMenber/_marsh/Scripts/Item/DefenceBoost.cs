using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenceBoost : ItemBase {
    public int boostAmount;
    public int duration;

    public DefenceBoost(string name, int id, int Amount, int time, Sprite icon = null)
        : base(name, id, eItemType.Boost, icon) {
        this.boostAmount = Amount;
        duration = time;
    }

    public override void Use(GameObject user) {
        PlayerBase player = user.GetComponent<PlayerBase>();
        if (player != null) {
            player.DefenceBoost(boostAmount, duration);
            Debug.Log($"{itemName} を使って DEF を {boostAmount} ブースト！");
        }
    }
}
