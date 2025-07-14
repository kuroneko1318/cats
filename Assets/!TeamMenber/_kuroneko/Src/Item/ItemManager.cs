using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

//    Weapon,     // 武器(1000~1999)
//    Armor,      // 防具(2000~2999)
//    Heal,       // 回復アイテム（3000~3999）
//    Material,   // 素材アイテム（4000~4999）
//    Money       // 換金アイテム・通貨など(5000~5999)

public class ItemManager : SystemObject<ItemManager> {

    public static ItemManager instance = null;

    // ゲーム内に存在する全アイテムの一覧（Listで管理）
    private List<ItemBase> itemList = new List<ItemBase>();
    private List<WeaponBase> weaponList = new List<WeaponBase>();
    private List<ArmorBase> armorList = new List<ArmorBase>();

    //  コンストラクタ：初期化時に全アイテムを登録
    public override void Initialize() {
        instance = this;

        // ここでアイテム5種を登録（ID）
        itemList.Add(new ItemBase("薬草", 4001, eItemType.Material));
        itemList.Add(new ItemBase("棒", 4002, eItemType.Material));
        itemList.Add(new ItemBase("石A", 4003, eItemType.Material));
        itemList.Add(new ItemBase("石B", 4004, eItemType.Material));
        itemList.Add(new ItemBase("鉄", 4005, eItemType.Material));
        itemList.Add(new ItemBase("鉄", 4006, eItemType.Material));
    }

    //  IDからアイテムを取得する
    public ItemBase GetItemByID(int id) {
        return itemList.Find(i => i.itemID == id);
    }

    //  名前からアイテムを取得する
    public ItemBase GetItemByName(string name) {
        return itemList.Find(i => i.itemName == name);
    }

    //  登録されているすべてのアイテムを取得（デバッグ・UI用など）
    public List<ItemBase> GetAllItems() {
        return new List<ItemBase>(itemList); // 外部から編集されないようコピーして返す
    }

    //  武器を追加する際に末尾に新規生成
    public void AddWeapon(string name, int id, int atk, int skill1, int skill2) {
        weaponList.Append(new WeaponBase(name, id, atk, skill1, skill2));
    }

    public void AddArmor(string name, int id, int hp, int skill1, int skill2) {
        armorList.Append(new ArmorBase(name, id, hp, skill1, skill2));
    }

}
