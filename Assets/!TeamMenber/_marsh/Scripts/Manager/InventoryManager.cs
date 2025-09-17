using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using static UnityEditor.Progress;

public class InventoryManager : SystemObject<InventoryManager> {
    [Header("InventoryData")]
    public Inventory inventory; // インベントリ
    [NonSerialized] public Inventory bag;

    [Header("UIプレハブ")]
    public GameObject inventoryPanelPrefab;
    public GameObject inventorySlotUIPrefab;

    private GameObject inventoryPanelInstance;
    private Transform contentParent;

    private int selectedIndex = 0;
    private const int columns = 9;
    private const int rows = 9;

    private List<InventoryUI> slotUIList = new List<InventoryUI>();

    // クラフト用スロット
    private InventorySlot[] craftSlots = new InventorySlot[2];
    private InventorySlot craftResult = new InventorySlot();

    // 装備用スロット
    private InventorySlot weaponSlot = new InventorySlot();
    private InventorySlot armorSlot = new InventorySlot();
    private InventorySlot[] accessorySlots = new InventorySlot[4];

    private enum UIArea { InventoryTop, InventoryBottom, Craft, Equip }
    private UIArea currentArea = UIArea.InventoryTop;

    private ItemBase heldItem = null;
    private int originIndex = -1;
    private UIArea originArea;

    public override void Initialize() {
        bag = Instantiate(inventory);

        // クラフトと装備スロット初期化
        for (int i = 0; i < craftSlots.Length; i++) craftSlots[i] = new InventorySlot();
        craftResult = new InventorySlot();

        weaponSlot = new InventorySlot();
        armorSlot = new InventorySlot();
        for (int i = 0; i < accessorySlots.Length; i++) accessorySlots[i] = new InventorySlot();
    }

    void Update() {
        if (inventoryPanelInstance != null && inventoryPanelInstance.activeSelf) {
            if (Input.GetKeyDown(KeyCode.RightArrow)) MoveRight();
            if (Input.GetKeyDown(KeyCode.LeftArrow)) MoveLeft();
            if (Input.GetKeyDown(KeyCode.UpArrow)) MoveSelection(0, -1);
            if (Input.GetKeyDown(KeyCode.DownArrow)) MoveSelection(0, 1);

            if (Input.GetKeyDown(KeyCode.Z)) HandleSelect(); // A
            if (Input.GetKeyDown(KeyCode.X)) HandleCancel(); // B
        }
    }

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

    public void CloseInventory() {
        if (inventoryPanelInstance != null) {
            inventoryPanelInstance.SetActive(false);
        }
    }

    private void RefreshUI() {
        if (contentParent == null) return;

        foreach (Transform child in contentParent) Destroy(child.gameObject);
        slotUIList.Clear();

        // Inventory表示
        for (int i = 0; i < bag.slots.Length; i++) {
            var slot = bag.slots[i];
            GameObject itemObj = Instantiate(inventorySlotUIPrefab, contentParent);
            var slotUI = itemObj.GetComponent<InventoryUI>();
            if (slotUI == null) continue;

            slotUI.SetSlot(slot.item, slot.amount);
            slotUIList.Add(slotUI);
        }

        // Craftスロット表示（簡易的に同じUIで流用）
        foreach (var slot in craftSlots) {
            var itemObj = Instantiate(inventorySlotUIPrefab, contentParent);
            var slotUI = itemObj.GetComponent<InventoryUI>();
            slotUI.SetSlot(slot.item, slot.amount);
            slotUIList.Add(slotUI);
        }

        // Craft結果表示
        {
            var itemObj = Instantiate(inventorySlotUIPrefab, contentParent);
            var slotUI = itemObj.GetComponent<InventoryUI>();
            slotUI.SetSlot(craftResult.item, craftResult.amount);
            slotUIList.Add(slotUI);
        }

        // Equip表示
        var equipSlots = new List<InventorySlot> { weaponSlot, armorSlot };
        equipSlots.AddRange(accessorySlots);
        foreach (var slot in equipSlots) {
            var itemObj = Instantiate(inventorySlotUIPrefab, contentParent);
            var slotUI = itemObj.GetComponent<InventoryUI>();
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
        for (int i = 0; i < slotUIList.Count; i++)
            slotUIList[i].SetHighlight(i == selectedIndex);
    }

    private void MoveRight() {
        // エリア移動判定（Inventory→Craft/Equip）
        if ((currentArea == UIArea.InventoryTop && IsAtRightEdge()) ||
            (currentArea == UIArea.InventoryBottom && IsAtRightEdge())) {
            currentArea = currentArea == UIArea.InventoryTop ? UIArea.Craft : UIArea.Equip;
            selectedIndex = 0;
            RefreshUI();
        }
        else {
            MoveSelection(1, 0);
        }
    }

    private void MoveLeft() {
        if (currentArea == UIArea.Craft || currentArea == UIArea.Equip) {
            currentArea = currentArea == UIArea.Craft ? UIArea.InventoryTop : UIArea.InventoryBottom;
            selectedIndex = GetRightEdgeIndex();
            RefreshUI();
        }
        else {
            MoveSelection(-1, 0);
        }
    }

    private InventorySlot GetCurrentSlot() {
        if (bag == null || bag.slots == null) return null;
        if (selectedIndex < 0 || selectedIndex >= slotUIList.Count) return null;

        // 仮でInventory範囲内かを判定（後でエリア分割で調整）
        return selectedIndex < bag.slots.Length ? bag.slots[selectedIndex] : null;
    }

    private void HandleSelect() {
        var slot = GetCurrentSlot();
        if (slot == null) return;

        if (heldItem == null) {
            heldItem = slot.TakeItem();
            originArea = currentArea;
            originIndex = selectedIndex;
        }
        else {
            ItemBase temp = slot.TakeItem();
            slot.SetItem(heldItem);
            heldItem = temp;
            if (heldItem == null) originIndex = -1;
        }

        RefreshUI();
    }

    private void HandleCancel() {
        if (heldItem != null) {
            var originSlot = GetSlot(originArea, originIndex);
            if (originSlot != null) originSlot.SetItem(heldItem);
            heldItem = null;
            originIndex = -1;
            RefreshUI();
        }
        else {
            CloseInventory();
        }
    }

    private InventorySlot GetSlot(UIArea area, int index) {
        if (bag == null || bag.slots == null) return null;
        if (index < 0 || index >= bag.slots.Length) return null;
        return bag.slots[index];
    }

    private bool IsAtRightEdge() {
        int col = selectedIndex % columns;
        return col == columns - 1;
    }

    private int GetRightEdgeIndex() {
        int row = selectedIndex / columns;
        return row * columns + (columns - 1);
    }
}