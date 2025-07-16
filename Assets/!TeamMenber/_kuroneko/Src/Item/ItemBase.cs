using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eItemType {
    Weapon,     // 武器(1000~1999)
    Armor,      // 防具(2000~2999)
    Heal,       // 回復アイテム（3000~3999）
    Material,   // 素材アイテム（4000~4999）
    Money       // 換金アイテム・通貨など(5000~5999)
}

public class ItemBase
{
    public string itemName;    // アイテムの名前（表示名）
    public int itemID;         // 一意のID（ここでは1〜5の連番）
    public eItemType type;      // アイテムのカテゴリ（武器、防具など）
    public int[] Skill;

    private const int _SKILL_MAX = 2;

    // コンストラクタ：新しいアイテムを作成
    public ItemBase(string name, int id, eItemType type, int skill1 = 0, int skill2 = 0) {
        this.itemName = name;
        this.itemID = id;
        this.type = type;
        Skill = new int [_SKILL_MAX] { skill1, skill2 };
    }
}
