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
    private const int columns = 10;
    private const int rows = 10;
    private List<InventoryUI> slotUIList = new List<InventoryUI>();


    public override void Initialize() {
        bag = Instantiate(inventory);
        
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            ToggleInventory();
        }

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

}
