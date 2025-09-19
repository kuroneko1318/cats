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
        //Sprite armorIcon = Resources.Load<Sprite>("icon/armor");
        Sprite swordIcon = Resources.Load<Sprite>("icon/NormalSword");
        Sprite BarIcon = Resources.Load<Sprite>("icon/Stick");
        Sprite CopperIcon = Resources.Load<Sprite>("icon/Copper");
        Sprite HerbIcon = Resources.Load<Sprite>("icon/Herb");
        Sprite IronIcon = Resources.Load<Sprite>("icon/iron");
        Sprite NeedleIcon = Resources.Load<Sprite>("icon/needle");
        Sprite NutIcon = Resources.Load<Sprite>("icon/nut");
        Sprite ObsidianIcon = Resources.Load<Sprite>("icon/Obsidian");
        Sprite potionIcon = Resources.Load<Sprite>("icon/potion");
        Sprite RopeIcon = Resources.Load<Sprite>("icon/rope");
        Sprite StoneAIcon = Resources.Load<Sprite>("icon/stoneA");
        Sprite StoneBIcon = Resources.Load<Sprite>("icon/stoneB");


        // ここでアイテム5種を登録（ID）
        itemList.Add(new ItemBase("回復薬", 3000, eItemType.Heal, potionIcon));
        itemList.Add(new ItemBase("木の実", 3001, eItemType.Heal, NutIcon));
        itemList.Add(new ItemBase("薬草", 4001, eItemType.Material, HerbIcon));
        itemList.Add(new ItemBase("紐", 4002, eItemType.Material, RopeIcon));
        itemList.Add(new ItemBase("石", 4020, eItemType.Material, StoneAIcon));
        itemList.Add(new ItemBase("ライトストーン", 4021, eItemType.Material));
        itemList.Add(new ItemBase("ブラッドストーン", 4022,eItemType.Material));
        itemList.Add(new ItemBase("ブラックマリン", 4023,eItemType.Material));
        itemList.Add(new ItemBase("シーネリアン", 4024,eItemType.Material));
        itemList.Add(new ItemBase("マカライト", 4025,eItemType.Material));
        itemList.Add(new ItemBase("鉄", 4030, eItemType.Material, IronIcon));
        itemList.Add(new ItemBase("銅", 4031, eItemType.Material, CopperIcon));
        itemList.Add(new ItemBase("黒曜石", 4032, eItemType.Material, ObsidianIcon));
        itemList.Add(new ItemBase("鋭い黒曜石", 4033, eItemType.Material));
        itemList.Add(new ItemBase("研磨石", 4034, eItemType.Material));
        itemList.Add(new ItemBase("魔石", 4035, eItemType.Material));
        itemList.Add(new ItemBase("融合石", 4036, eItemType.Material));
        itemList.Add(new ItemBase("木", 4100, eItemType.Material));
        itemList.Add(new ItemBase("丈夫な木", 4101, eItemType.Material));
        itemList.Add(new ItemBase("棒", 4102, eItemType.Material, BarIcon));
        itemList.Add(new ItemBase("針", 4103, eItemType.Material, NeedleIcon));
        itemList.Add(new ItemBase("魔剣の剣身", 4104, eItemType.Material));
        itemList.Add(new ItemBase("普通の柄", 4105, eItemType.Material));
        itemList.Add(new ItemBase("魔剣の柄", 4106, eItemType.Material));
        weaponList.Add(new WeaponBase("普通の剣", 1001,10, swordIcon));
        weaponList.Add(new WeaponBase("ライトソード", 1002, 10));
        weaponList.Add(new WeaponBase("未完成の魔剣", 1003, 1));
        weaponList.Add(new WeaponBase("魔剣ズルフィカール", 1004, 100));
        weaponList.Add(new WeaponBase("魔剣フルンティング", 1005, 100));
        weaponList.Add(new WeaponBase("魔剣ネイリング", 1006, 100));
        armorList.Add(new ArmorBase("普通のアーマー", 2001, 30));
        armorList.Add(new ArmorBase("マジックアーマー", 2002, 50));
        armorList.Add(new ArmorBase("フュージョンアーマー", 2003, 50));

    }

    //  IDからアイテムを取得する
    public ItemBase GetItemByID(int id) {
        return itemList.Find(i => i.itemID == id);
    }

    public WeaponBase GetWeaponByID(int id) {
        return weaponList.Find(i => i.itemID == id);
    }

    public ArmorBase GetArmorByID(int id) {
        return armorList.Find(i => i.itemID == id);
    }

    //  名前からアイテムを取得する
    public ItemBase GetItemByName(string name) {
        return itemList.Find(i => i.itemName == name);
    }
    public WeaponBase GetWeaponByName(string name) {
        return weaponList.Find(i => i.itemName == name);
    }
    public ArmorBase GetArmorByName(string name) {
        return armorList.Find(i => i.itemName == name);
    }

    //  登録されているすべてのアイテムを取得（デバッグ・UI用など）
    public List<ItemBase> GetAllItems() {
        return new List<ItemBase>(itemList); // 外部から編集されないようコピーして返す
    }

    //  武器を追加する際に末尾に新規生成
    public void AddWeapon(string name, int id, int atk, Sprite image, int skill1, int skill2) {
        weaponList.Append(new WeaponBase(name, id, atk, image, skill1, skill2));
    }

    public void AddArmor(string name, int id, int hp, Sprite image, int skill1, int skill2) {
        armorList.Append(new ArmorBase(name, id, hp, image, skill1, skill2));
    }

    private Sprite icon(string pass) {
        return Resources.Load<Sprite>(pass);
    }

}
