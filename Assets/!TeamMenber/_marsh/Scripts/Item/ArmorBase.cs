using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorBase : ItemBase
{
    public int armorHp;

    // コンストラクタ：新しいアイテムを作成
    public ArmorBase(string name, int id, int hp, Sprite image = null, int skill1 = 0, int skill2 = 0) 
        : base(name, id, eItemType.Armor, image, skill1, skill2){

        armorHp = hp;
    }
}
