using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorBase
{
    public string armorName;    // アイテムの名前（表示名）
    public int armorID;         // 一意のID
    public int armorHp;
    public int[] armorSkill;

    private const int _ARMOR_SKILL_MAX = 2;

    // コンストラクタ：新しいアイテムを作成
    public ArmorBase(string name, int id, int hp, int skill1 = 0, int skill2 = 0) {

        armorName = name;
        armorID = id;
        armorHp = hp;
        armorSkill = new int[_ARMOR_SKILL_MAX] { skill1, skill2 };

    }
}
