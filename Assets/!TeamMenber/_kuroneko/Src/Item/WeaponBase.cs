using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase
{
    public string weaponName;    // アイテムの名前（表示名）
    public int weaponID;         // 一意のID
    public int weaponAttack;
    public int[] weaponSkill;

    private const int _WEAPON_SKILL_MAX = 2;

    // コンストラクタ：新しいアイテムを作成
    public WeaponBase(string name, int id,int atk,int skill1 = 0,int skill2 = 0) {

        weaponName = name;
        weaponID = id;
        weaponAttack = atk;
        weaponSkill = new int[_WEAPON_SKILL_MAX] {skill1,skill2};
        
    }
}
