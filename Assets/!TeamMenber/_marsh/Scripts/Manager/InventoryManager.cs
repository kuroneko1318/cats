using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class InventoryManager : SystemObject<InventoryManager> {

    [Header("InventoryData")]
    public Inventory inventory; // インスペクターでインベントリオブジェクトの設定
    [NonSerialized]public Inventory bag;

    [Header("UIプレハブ")]
    public GameObject inventoryPanelPrefab;
    public GameObject inventorySlotUIPrefab;

    private GameObject inventoryPanelInstance;
    private Transform contentParent; // スクロールできるようにするための親の位置

    //UIをスロットにする
    private int selectedIndex = 0;
    private const int columns = 9;
    private const int rows = 9;
    private List<InventoryUI> slotUIList = new List<InventoryUI>();

    //プレイヤーキャッシュ用
    PlayerBase player;

    public override void Initialize() {
        bag = Instantiate(inventory);
        // シーン内から Player タグの付いたオブジェクトを探す
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) {
            player = playerObj.GetComponent<PlayerBase>();
        }
    }

    void Update() {
        //if (Input.GetKeyDown(KeyCode.Tab)) {
        //    ToggleInventory();
        //}

        //デバッグ
        if (Input.GetKeyDown(KeyCode.O)) {
            var herb = ItemManager.Instance?.GetItemByID(4001);
            var potion = ItemManager.Instance?.GetItemByID(3000);
            if (herb != null && bag != null) {
                bag.AddItem(herb, 10);
                RefreshUI(); // UI更新
            }
            if (potion != null && bag != null) {
                bag.AddItem(potion, 10);
                RefreshUI(); // UI更新
            }
        }

        if (inventoryPanelInstance != null && inventoryPanelInstance.activeSelf) {
            if (Input.GetKeyDown(KeyCode.RightArrow)) MoveSelection(1, 0);
            if (Input.GetKeyDown(KeyCode.LeftArrow)) MoveSelection(-1, 0);
            if (Input.GetKeyDown(KeyCode.UpArrow)) MoveSelection(0, -1);
            if (Input.GetKeyDown(KeyCode.DownArrow)) MoveSelection(0, 1);
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)
            || Input.GetKeyDown(KeyCode.JoystickButton0)) {
            UseSelectedItem();
        }
    }

    /// <summary>
    /// インベントリUIを開く
    /// </summary>
    public void OpenInventory() {
        if (inventoryPanelInstance == null) {
            Transform canvasTransform = GameObject.Find("ItemUICanvas").transform;
            inventoryPanelInstance = Instantiate(inventoryPanelPrefab, canvasTransform);
            inventoryPanelInstance.SetActive(true);
            contentParent = inventoryPanelInstance.transform.Find("Content");
        }

        inventoryPanelInstance.SetActive(true);
        RefreshUI();
    }

    /// <summary>
    /// インベントリUIを閉じる
    /// </summary>
    public void CloseInventory() {
        if (inventoryPanelInstance != null) {
            inventoryPanelInstance.SetActive(false);
        }
    }

    /// <summary>
    /// インベントリUIの開閉を切り替える
    /// </summary>
    public void ToggleInventory() {
        if (inventoryPanelInstance == null || !inventoryPanelInstance.activeSelf) {
            OpenInventory();
        }
        else {
            CloseInventory();
        }
    }

    /// <summary>
    /// インベントリの中身をUIに表示
    /// </summary>
    private void RefreshUI() {
        Debug.Log("RefreshUI 開始");

        if (contentParent == null) {
            Debug.LogError("contentParent が null です");
            return;
        }

        if (inventorySlotUIPrefab == null) {
            Debug.LogError("inventorySlotUIPrefab が null です");
            return;
        }

        if (bag == null || bag.slots == null) {
            Debug.LogError("bag または bag.slots が null です");
            return;
        }

        foreach (Transform child in contentParent) {
            Destroy(child.gameObject);
        }
        slotUIList.Clear();

        for (int i = 0; i < bag.slots.Length; i++) {
            var slot = bag.slots[i];
            GameObject itemObj = Instantiate(inventorySlotUIPrefab, contentParent);

            var slotUI = itemObj.GetComponent<InventoryUI>();
            if (slotUI == null) {
                Debug.LogError($"InventorySlotUI がプレハブに見つかりません（index: {i}）");
                continue;
            }

            slotUI.SetSlot(slot.item, slot.amount);
            slotUIList.Add(slotUI);
        }

        UpdateSelectionHighlight();
    }

    private void MoveSelection(int x, int y) {
        int col = selectedIndex % columns;
        int row = selectedIndex / columns;

        col = Mathf.Clamp(col + x, 0, columns - 1);
        row = Mathf.Clamp(row + y, 0, rows - 1);

        selectedIndex = row * columns + col;
        UpdateSelectionHighlight();
    }

    private void UpdateSelectionHighlight() {
        for (int i = 0; i < slotUIList.Count; i++) {
            slotUIList[i].SetHighlight(i == selectedIndex);
        }
    }
    private void UseSelectedItem() {
        if (bag == null || bag.slots == null) return;

        if (selectedIndex < 0 || selectedIndex >= bag.slots.Length) return;

        var slot = bag.slots[selectedIndex];
        if (slot.item == null || slot.amount <= 0) {
            Debug.Log("このスロットにはアイテムがありません");
            return;
        }

        var item = slot.item;

        if (player == null) {
            Debug.LogError("Player が見つかりません");
            return;
        }

        // 種類ごとに処理
        switch (item.type) {
            case eItemType.Heal:
                if (item.itemID == 3000) { // ポーション例
                    player.HealHp(50);
                }
                break;

            case eItemType.Weapon: // 装備アイテム
                Debug.Log($"{item.itemName} を装備しました");
                // 装備処理を書く
                break;

            case eItemType.Material:
                Debug.Log($"{item.itemName} は使えません");
                return;
        }

        // 使用したので数量を減らす
        slot.amount--;
        if (slot.amount <= 0) {
            slot.item = null;
        }

        // UI更新
        RefreshUI();
    }

}
