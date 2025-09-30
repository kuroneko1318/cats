using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : ItemBase
{
    public int weaponAttack;
    public int weaponCriticalChance;
    public int weaponCriticalDamage;

    // コンストラクタ：新しいアイテムを作成
    public WeaponBase(string name, int id,int atk,int criChance, int criDamage, Sprite image = null, int skill1 = 0,int skill2 = 0)
        : base(name, id, eItemType.Weapon, image, skill1, skill2) {

        weaponAttack = atk;
        weaponCriticalChance = criChance;
        weaponCriticalDamage = criDamage;
    }
}
