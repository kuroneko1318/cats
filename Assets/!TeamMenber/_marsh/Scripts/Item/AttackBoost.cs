using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBoost : ItemBase {
    public int boostAmount;
    public int duration;

    public AttackBoost(string name, int id, int amount, int time, Sprite icon = null)
        : base(name, id, eItemType.Boost, icon) {
        this.boostAmount = amount;
        duration = time;
    }

    public override void Use(GameObject user) {
        PlayerBase player = user.GetComponent<PlayerBase>();
        if (player != null) {
            player.AttackBoost(boostAmount, duration);
            Debug.Log($"{itemName} を使って ATK を {boostAmount} ブースト！");
        }
    }
}
