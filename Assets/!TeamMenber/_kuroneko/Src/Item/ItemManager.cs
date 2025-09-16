using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


//    Weapon,     // 武器(1000~1999)
//    Armor,      // 防具(2000~2999)
//    Heal,       // 回復アイテム（3000~3999）
//    Material,   // 素材アイテム（4000~4999）
//    Money       // 換金アイテム・通貨など(5000~5999)

public class ItemManager : SystemObject<ItemManager> {

    // ゲーム内に存在する全アイテムの一覧（Listで管理）
    private List<ItemBase> itemList = new List<ItemBase>();
    private List<WeaponBase> weaponList = new List<WeaponBase>();
    private List<ArmorBase> armorList = new List<ArmorBase>();

    //  コンストラクタ：初期化時に全アイテムを登録
    public override void Initialize() {
        Debug.Log("ItemManager　Initialize開始");
        Sprite potionIcon = Resources.Load<Sprite>("icon/potion");
        Sprite herbIcon = Resources.Load<Sprite>("icon/herb");


        // ここでアイテム5種を登録（ID）
        itemList.Add(new ItemBase("回復薬", 3000, eItemType.Heal, potionIcon));
        itemList.Add(new ItemBase("木の実", 3001, eItemType.Heal));
        itemList.Add(new ItemBase("薬草", 4001, eItemType.Material, herbIcon));
        itemList.Add(new ItemBase("紐", 4002, eItemType.Material));
        itemList.Add(new ItemBase("石A", 4020, eItemType.Material));
        itemList.Add(new ItemBase("石B", 4021, eItemType.Material));
        itemList.Add(new ItemBase("鉄", 4030, eItemType.Material));
        itemList.Add(new ItemBase("銅", 4031, eItemType.Material));
        itemList.Add(new ItemBase("棒", 4100, eItemType.Material));
        itemList.Add(new ItemBase("針", 4101, eItemType.Material));
        weaponList.Add(new WeaponBase("武器A", 1001,10));
        weaponList.Add(new WeaponBase("武器B", 1002, 10));
        armorList.Add(new ArmorBase("防具A", 2001, 30));
    }

    //  IDからアイテムを取得する
    public ItemBase GetItemByID(int id) {
        return itemList.Find(i => i.itemID == id);
    }

    public WeaponBase GetWeaponByID(int id) {
        return weaponList.Find(i => i.itemID == id);
    }

    //  名前からアイテムを取得する
    public ItemBase GetItemByName(string name) {
        return itemList.Find(i => i.itemName == name);
    }
    public WeaponBase GetWeaponByName(string name) {
        return weaponList.Find(i => i.itemName == name);
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

    private Sprite icon(string pass) {
        return Resources.Load<Sprite>(pass);
    }

}
