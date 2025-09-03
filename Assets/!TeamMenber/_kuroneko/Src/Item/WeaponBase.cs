using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : ItemBase
{
    public int weaponAttack;

    // コンストラクタ：新しいアイテムを作成
    public WeaponBase(string name, int id,int atk,int skill1 = 0,int skill2 = 0)
        : base(name, id, eItemType.Weapon, Resources.Load<Sprite>("icon/weapon"), skill1, skill2) {

        weaponAttack = atk;
    }
}
