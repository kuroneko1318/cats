using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerchantNPC : MonoBehaviour {
    [Header("商品設定")]
    public List<ItemBase> allItems; // 商人が持っているアイテム

    [Header("取得設定")]
    public float interactionDistance = 3f; // 近くにいる判定距離

    private Transform player;

    private void Start() {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update() {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= interactionDistance && Input.GetKeyDown(KeyCode.E)) {
            GiveRandomItem();
        }
    }

    private void GiveRandomItem() {
        if (allItems.Count == 0) return;

        int index = Random.Range(0, allItems.Count);
        ItemBase item = allItems[index];

        // InventoryManagerのBagに追加
        if (InventoryManager.Instance.bag != null) {
            InventoryManager.Instance.bag.AddItem(item);
            Debug.Log($"{item.itemName} を入手しました！");
        }
        else {
            Debug.LogWarning("InventoryManagerのBagが設定されていません");
        }
    }
}
