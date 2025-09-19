using UnityEngine;
using System.Collections.Generic;
using System;

public class InventoryManager : SystemObject<InventoryManager> {
    [Header("Inventory Data")]
    public Inventory inventory; // 元データ
    [NonSerialized] public Inventory bag;

    [Header("UI Prefab")]
    public GameObject inventoryPanelPrefab;
    public GameObject inventorySlotUIPrefab;
    public GameObject craftSlotUIPrefab;
    public GameObject equipSlotUIPrefab;

    [Header("UI Parents")]
    public Transform InventoryTop;
    public Transform InventoryBottom;
    public Transform Craft;
    public Transform Equip;

    private GameObject inventoryPanelInstance;

    // インベントリUI
    private List<InventoryUI> inventoryTopUI = new List<InventoryUI>();
    private List<InventoryUI> inventoryBottomUI = new List<InventoryUI>();
    private List<CraftingSlot> craftUI = new List<CraftingSlot>();
    private List<InventoryUI> equipUI = new List<InventoryUI>();

    private InventorySlot[] craftSlots;
    private InventorySlot[] equipSlots;

    private InventorySlot craftResultSlot = new InventorySlot();


    private int selectedIndex = 0;
    private ItemBase heldItem = null;
    private int heldAmount = 0;
    private int originIndex = -1;
    private UIArea originArea;

    private const int columns = 9;
    private const int inventoryTopCount = 36;
    private const int inventoryBottomCount = 45;

    private enum UIArea { InventoryTop, InventoryBottom, Craft, Equip }
    private UIArea currentArea = UIArea.InventoryTop;

    public override void Initialize() {
        if (inventory == null) {
            Debug.LogError("InventoryManager: inventoryが設定されていません！");
            return;
        }

        // bag にコピー
        bag = inventory;

        // slots が null なら初期化
        if (bag.slots == null || bag.slots.Length != bag.slotCount) {
            bag.slots = new InventorySlot[bag.slotCount];
            for (int i = 0; i < bag.slotCount; i++) {
                bag.slots[i] = new InventorySlot();
            }
        }
        // クラフト・装備スロット初期化
        craftSlots = new InventorySlot[3]; // 2枠＋完成品1
        for (int i = 0; i < craftSlots.Length; i++)
            craftSlots[i] = new InventorySlot();

        equipSlots = new InventorySlot[2]; // 武器1、防具1
        for (int i = 0; i < equipSlots.Length; i++)
            equipSlots[i] = new InventorySlot();
    }

    void Update() {
        if (inventoryPanelInstance == null || !inventoryPanelInstance.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.RightArrow)) MoveRight();
        if (Input.GetKeyDown(KeyCode.LeftArrow)) MoveLeft();
        if (Input.GetKeyDown(KeyCode.UpArrow)) MoveUp();
        if (Input.GetKeyDown(KeyCode.DownArrow)) MoveDown();

        if (Input.GetKeyDown(KeyCode.Z)) HandleSelect();
        if (Input.GetKeyDown(KeyCode.X)) HandleCancel();



        if (Input.GetKeyDown(KeyCode.O)) {
            bag.AddItem(ItemManager.Instance.GetItemByName("棒"), 5);
            bag.AddItem(ItemManager.Instance.GetItemByName("鉄"), 5);
            bag.AddItem(ItemManager.Instance.GetItemByName("薬草"), 5);
        }
    }

    #region UI
    public void OpenInventory() {
        if (inventoryPanelInstance == null) {
            Transform canvas = GameObject.Find("ItemUICanvas").transform;
            inventoryPanelInstance = Instantiate(inventoryPanelPrefab, canvas);

            InventoryTop = inventoryPanelInstance.transform.Find("InventoryTop");
            InventoryBottom = inventoryPanelInstance.transform.Find("InventoryBottom");
            Craft = inventoryPanelInstance.transform.Find("Craft");
            Equip = inventoryPanelInstance.transform.Find("Equip");

            if (InventoryTop == null || InventoryBottom == null || Craft == null || Equip == null)
                Debug.LogError("InventoryManager: UI Contentの取得に失敗しました。名前を確認してください。");
        }

        inventoryPanelInstance.SetActive(true);
        RefreshUI();
    }

    public void CloseInventory() {
        if (inventoryPanelInstance != null)
            inventoryPanelInstance.SetActive(false);
    }

    private void RefreshUI() {
        // インベントリ上部
        inventoryTopUI.Clear();
        ClearChildren(InventoryTop);
        for (int i = 0; i < inventoryTopCount; i++) {
            var slot = bag.slots[i];
            var ui = Instantiate(inventorySlotUIPrefab, InventoryTop).GetComponent<InventoryUI>();
            ui.SetSlot(slot.item, slot.amount);
            inventoryTopUI.Add(ui);
        }

        // インベントリ下部
        inventoryBottomUI.Clear();
        ClearChildren(InventoryBottom);
        for (int i = 0; i < inventoryBottomCount; i++) {
            var slot = bag.slots[i + inventoryTopCount];
            var ui = Instantiate(inventorySlotUIPrefab, InventoryBottom).GetComponent<InventoryUI>();
            ui.SetSlot(slot.item, slot.amount);
            inventoryBottomUI.Add(ui);
        }

        // クラフト
        craftUI.Clear();
        ClearChildren(Craft);

        // craftSlots を InventoryManager 内で保持している場合
        for (int i = 0; i < craftSlots.Length; i++) {
            GameObject obj = Instantiate(craftSlotUIPrefab, Craft);
            CraftingSlot slotUI = obj.GetComponent<CraftingSlot>();

            // craftSlots[i] は InventorySlot とかデータ用の構造体
            // CraftingSlot UI にデータを紐付け
            slotUI.SetItem(craftSlots[i].item, craftSlots[i].amount);

            // craftUI リストに追加
            craftUI.Add(slotUI);
        }
        // CraftManager にスロットとUIを渡す
        if (craftUI.Count >= 3 && craftSlots.Length >= 3) {
            CraftManager.Instance.SetCraftSlot(0, craftSlots[0], craftUI[0]);
            CraftManager.Instance.SetCraftSlot(1, craftSlots[1], craftUI[1]);
            CraftManager.Instance.SetCraftSlot(2, craftSlots[2], craftUI[2]);
        }

        CraftManager.Instance.UpdateResult();

        // 選択インデックスが範囲外の場合は 0 に
        if (selectedIndex >= craftUI.Count) selectedIndex = 0;

        // 装備
        equipUI.Clear();
        ClearChildren(Equip);
        for (int i = 0; i < equipSlots.Length; i++) {
            var ui = Instantiate(equipSlotUIPrefab, Equip).GetComponent<InventoryUI>();
            ui.SetSlot(equipSlots[i].item, equipSlots[i].amount);
        }

        UpdateHighlight();
    }

    private void ClearChildren(Transform parent) {
        foreach (Transform t in parent) Destroy(t.gameObject);
    }

    private void UpdateHighlight() {
        // まず全てのUIのハイライトをリセット
        foreach (var ui in inventoryTopUI) ui.SetHighlight(false);
        foreach (var ui in inventoryBottomUI) ui.SetHighlight(false);
        foreach (var ui in craftUI) ui.SetHighlight(false);
        foreach (var ui in equipUI) ui.SetHighlight(false);

        // 現在選択中のUIだけハイライト
        switch (currentArea) {
            case UIArea.InventoryTop:
                if (selectedIndex >= 0 && selectedIndex < inventoryTopUI.Count)
                    inventoryTopUI[selectedIndex].SetHighlight(true);
                break;
            case UIArea.InventoryBottom:
                if (selectedIndex >= 0 && selectedIndex < inventoryBottomUI.Count)
                    inventoryBottomUI[selectedIndex].SetHighlight(true);
                break;
            case UIArea.Craft:
                if (selectedIndex >= 0 && selectedIndex < craftUI.Count)
                    craftUI[selectedIndex].SetHighlight(true);
                break;
            case UIArea.Equip:
                if (selectedIndex >= 0 && selectedIndex < equipUI.Count)
                    equipUI[selectedIndex].SetHighlight(true);
                break;
        }
    }



    private InventoryUI GetCurrentUI() {
        switch (currentArea) {
            case UIArea.InventoryTop: return inventoryTopUI[selectedIndex];
            case UIArea.InventoryBottom: return inventoryBottomUI[selectedIndex];
            case UIArea.Craft: return craftUI[selectedIndex];
            case UIArea.Equip: return equipUI[selectedIndex];
        }
        return null;
    }
    #endregion

    #region 移動
    private void MoveRight() {
        switch (currentArea) {
            case UIArea.InventoryTop:
                if (selectedIndex % columns == columns - 1) {
                    currentArea = UIArea.Craft;
                    selectedIndex = 0;
                }
                else selectedIndex++;
                break;
            case UIArea.InventoryBottom:
                if (selectedIndex % columns == columns - 1) {
                    currentArea = UIArea.Equip;
                    selectedIndex = 0;
                }
                else selectedIndex++;
                break;
            case UIArea.Craft:
                selectedIndex = Mathf.Min(selectedIndex + 1, craftUI.Count - 1);
                break;
            case UIArea.Equip:
                selectedIndex = Mathf.Min(selectedIndex + 1, equipUI.Count - 1);
                break;
        }
        UpdateHighlight();
    }

    private void MoveLeft() {
        switch (currentArea) {
            case UIArea.InventoryTop:
                if (selectedIndex > 0) selectedIndex--;
                break;
            case UIArea.InventoryBottom:
                if (selectedIndex > 0) selectedIndex--;
                break;
            case UIArea.Craft:
                if (selectedIndex > 0) selectedIndex--;
                else { currentArea = UIArea.InventoryTop; selectedIndex = columns - 1; }
                break;
            case UIArea.Equip:
                if (selectedIndex > 0) selectedIndex--;
                else { currentArea = UIArea.InventoryBottom; selectedIndex = columns - 1; }
                break;
        }
        UpdateHighlight();
    }

    private void MoveDown() {
        if (currentArea == UIArea.InventoryTop) {
            if (selectedIndex + columns < inventoryTopCount) selectedIndex += columns;
            else { currentArea = UIArea.InventoryBottom; selectedIndex = selectedIndex % columns; }
        }
        else if (currentArea == UIArea.InventoryBottom) {
            if (selectedIndex + columns < inventoryBottomCount) selectedIndex += columns;
        }
        UpdateHighlight();
    }

    private void MoveUp() {
        if (!IsInventoryArea()) return;

        switch (currentArea) {
            case UIArea.InventoryTop:
                // 一番上の行は上に移動できない
                if (selectedIndex >= columns)
                    selectedIndex -= columns;
                break;

            case UIArea.InventoryBottom:
                // 下部インベントリの最上段にいる場合は上部インベントリに移動
                if (selectedIndex < columns) {
                    currentArea = UIArea.InventoryTop;
                    selectedIndex += inventoryTopCount - columns; // 下部最上段から上部最下段に移動
                }
                else {
                    selectedIndex -= columns;
                }
                break;
        }

        UpdateHighlight();
    }


    private bool IsAtRightEdge() {
        return (selectedIndex % columns) == (columns - 1);
    }

    private bool IsInventoryArea() {
        return currentArea == UIArea.InventoryTop || currentArea == UIArea.InventoryBottom;
    }
    #endregion

    #region アイテム操作
    private InventorySlot GetCurrentSlot() {
        switch (currentArea) {
            case UIArea.InventoryTop: return bag.slots[selectedIndex];
            case UIArea.InventoryBottom: return bag.slots[selectedIndex + inventoryTopCount];
            case UIArea.Craft: return craftSlots[selectedIndex];
            case UIArea.Equip: return equipSlots[selectedIndex];
        }
        return null;
    }

    private void HandleSelect() {
        var slot = GetCurrentSlot();
        if (slot == null) return;

        // スロットのアイテムを一時保存
        ItemBase slotItem = slot.item;
        int slotAmount = slot.amount;

        // --- クラフト領域 ---
        if (currentArea == UIArea.Craft) {
            // 選択が結果スロット（完成品）なら取得処理
            if (currentArea == UIArea.Craft && selectedIndex == 2) {
                int craftedAmount;
                string craftedName = CraftManager.Instance.TakeResult(out craftedAmount);
                if (string.IsNullOrEmpty(craftedName)) return;

                // アイテム取得
                ItemBase item = ItemManager.Instance.GetItemByName(craftedName);
                if (item != null) {
                    bag.AddItem(item, craftedAmount);
                    RefreshUI();
                    return;
                }

                WeaponBase weapon = ItemManager.Instance.GetWeaponByName(craftedName);
                if (weapon != null) {
                    bag.AddItem(weapon, craftedAmount);
                    RefreshUI();
                    return;
                }

                ArmorBase armor = ItemManager.Instance.GetArmorByName(craftedName);
                if (armor != null) {
                    bag.AddItem(armor, craftedAmount);
                    RefreshUI();
                    return;
                }

                Debug.LogError($"[Craft] ItemManager に '{craftedName}' が存在しません");
            }
        }

        // まだ何も持っていない場合
        if (heldItem == null) {
            if (slot.item == null || slot.amount <= 0) return; // 空なら何もしない

            heldItem = slot.item;
            heldAmount = slot.amount;

            // クラフトスロットなら数量を消さない
            if (currentArea != UIArea.Craft) {
                slot.Clear();
            }

            originArea = currentArea;
            originIndex = selectedIndex;
        }
        else {
            // 移動先に何かある場合もない場合も入れ替え
            slot.SetItem(heldItem, heldAmount);

            // 元スロットに元々あったアイテムを戻す
            if (originIndex >= 0) {
                var originSlot = GetSlot(originArea, originIndex);
                originSlot.SetItem(slotItem, slotAmount);
            }

            // 持ち物を更新（今回の操作で持つものは無し）
            heldItem = null;
            heldAmount = 0;
            originIndex = -1;
        }

        RefreshUI();

        // --- クラフト結果更新 ---
        CraftManager.Instance.UpdateResult(); // これで craftSlots[2] に結果をセット
    }

    private void HandleCancel() {
        if (heldItem != null && originIndex >= 0) {
            var originSlot = GetSlot(originArea, originIndex);
            originSlot.SetItem(heldItem, heldAmount);
            heldItem = null;
            originIndex = -1;
            RefreshUI();
        }
        else {
            CloseInventory();
        }
    }

    private InventorySlot GetSlot(UIArea area, int index) {
        switch (area) {
            case UIArea.InventoryTop: return bag.slots[index];
            case UIArea.InventoryBottom: return bag.slots[index + inventoryTopCount];
            case UIArea.Craft: return craftSlots[index];
            case UIArea.Equip: return equipSlots[index];
        }
        return null;
    }
    #endregion
}
