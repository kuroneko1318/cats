using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerchantNPC : MonoBehaviour {
    [Header("商人の所持アイテム")]
    public List<ItemBase> allItems = new List<ItemBase>(); // 商人が持っているアイテム

    [Header("取得設定")]
    public float interactionDistance = 3f; // 近くにいる判定距離

    private Transform player;

    private void Start() {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // 例：コードでアイテムを登録
        RegisterItems();
    }

    private void Update() {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= interactionDistance && Input.GetKeyDown(KeyCode.E)) {
            if(QuestManager.Instance.playerMoney >= 100)
                GiveRandomItem();
        }
    }

    private void GiveRandomItem() {
        if (allItems.Count == 0) return;

        int index = Random.Range(0, allItems.Count);
        ItemBase item = allItems[index];

        if (InventoryManager.Instance.bag != null) {
            InventoryManager.Instance.bag.AddItem(item, Random.Range(1, 6));
            Debug.Log($"{item.itemName} を入手しました！");
            QuestManager.Instance.playerMoney -= 100;
        }
        else {
            Debug.LogWarning("InventoryManagerのBagが設定されていません");
        }
    }

    // -------------------------------
    // 商人にアイテムをコードで登録する関数
    // -------------------------------
    private void RegisterItems() {
        allItems.Add(ItemManager.Instance.GetItemByID(3000)); // 回復薬
        allItems.Add(ItemManager.Instance.GetItemByID(9000)); // 攻撃薬
        allItems.Add(ItemManager.Instance.GetItemByID(9001)); // 防御薬
        allItems.Add(ItemManager.Instance.GetItemByID(4105)); // 針
        allItems.Add(ItemManager.Instance.GetItemByID(1001)); 
        allItems.Add(ItemManager.Instance.GetItemByID(1004)); 
        allItems.Add(ItemManager.Instance.GetItemByID(2001)); 
        allItems.Add(ItemManager.Instance.GetItemByID(2002)); 
        allItems.Add(ItemManager.Instance.GetItemByID(4032)); 
        allItems.Add(ItemManager.Instance.GetItemByID(3001)); 
        allItems.Add(ItemManager.Instance.GetItemByID(3003)); 
    }
}
