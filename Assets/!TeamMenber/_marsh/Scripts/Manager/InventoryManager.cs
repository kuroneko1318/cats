using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.InputSystem;

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

    [Header("Selected Item UI")]
    public Transform selectedItemParent; // InventoryPanel 内の SelectedItem オブジェクト
    private InventoryUI selectedItemUI;   // 生成したUIを保持
    public UnityEngine.UI.Image selectedItemIcon;
    public TMPro.TextMeshProUGUI selectedItemAmountText;

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
    public bool IsHoldingItem => heldItem != null;
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

        // --- bag を新規生成してコピー ---
        bag = Instantiate(inventory);
        bag.slotCount = inventory.slotCount;

        // slots をコピー
        bag.slots = new InventorySlot[bag.slotCount];
        for (int i = 0; i < bag.slotCount; i++) {
            bag.slots[i] = new InventorySlot();
            if (inventory.slots != null && inventory.slots.Length > i && inventory.slots[i] != null)
                bag.slots[i].SetItem(inventory.slots[i].item, inventory.slots[i].amount);
        }

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

    #region UI
    public void OpenInventory() {
        if (inventoryPanelInstance == null) {
            Transform canvas = GameObject.Find("ItemUICanvas").transform;
            inventoryPanelInstance = Instantiate(inventoryPanelPrefab, canvas);

            InventoryTop = inventoryPanelInstance.transform.Find("InventoryTop");
            InventoryBottom = inventoryPanelInstance.transform.Find("InventoryBottom");
            Craft = inventoryPanelInstance.transform.Find("Craft");
            Equip = inventoryPanelInstance.transform.Find("Equip");
            selectedItemParent = inventoryPanelInstance.transform.Find("SelectedItem");

            // 選択中アイテムUIを生成
            if (selectedItemParent != null) {
                GameObject obj = Instantiate(inventorySlotUIPrefab, selectedItemParent);
                obj.transform.localScale *= 1.5f;   // ← 1.5倍に拡大
                selectedItemUI = obj.GetComponent<InventoryUI>();
                UpdateHeldItemUI();
            }

            RefreshUI();
            if (InventoryTop == null || InventoryBottom == null || Craft == null || Equip == null)
                Debug.LogError("InventoryManager: UI Contentの取得に失敗しました。名前を確認してください。");
        }
        else {
            inventoryPanelInstance.SetActive(true);
            RefreshUI();
        }
    }

    public void CloseInventory() {
        if (inventoryPanelInstance != null) {
            inventoryPanelInstance.SetActive(false);
        }
    }

    private void RefreshUI() {

        // --- 現在の状態を保存 ---
        int prevIndex = selectedIndex;
        UIArea prevArea = currentArea;

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
        //if (selectedIndex >= craftUI.Count) selectedIndex = 0;

        // 装備
        equipUI.Clear();
        ClearChildren(Equip);
        for (int i = 0; i < equipSlots.Length; i++) {
            var ui = Instantiate(equipSlotUIPrefab, Equip).GetComponent<InventoryUI>();
            ui.SetSlot(equipSlots[i].item, equipSlots[i].amount);
            equipUI.Add(ui);
        }

        // --- 保存していた選択状態を復元 ---
        currentArea = prevArea;
        selectedIndex = Mathf.Min(prevIndex, GetAreaCount(currentArea) - 1);

        UpdateHighlight();
    }

    private int GetAreaCount(UIArea area) {
        return area switch {
            UIArea.InventoryTop => inventoryTopUI.Count,
            UIArea.InventoryBottom => inventoryBottomUI.Count,
            UIArea.Craft => craftUI.Count,
            UIArea.Equip => equipUI.Count,
            _ => 0
        };
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
    private void UpdateHeldItemUI() {
        if (selectedItemUI == null) return;

        if (heldItem != null && heldAmount > 0) {
            selectedItemUI.SetSlot(heldItem, heldAmount);
        }
        else {
            selectedItemUI.SetSlot(null, 0);
        }
        selectedItemUI.SetHighlight(false);
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
    public void MoveRight() {
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

    public void MoveLeft() {
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

    public void MoveDown() {
        if (currentArea == UIArea.InventoryTop) {
            if (selectedIndex + columns < inventoryTopCount) {
                selectedIndex += columns;
            }
            else {
                currentArea = UIArea.InventoryBottom;
                selectedIndex = selectedIndex % columns;
            }
        }
        else if (currentArea == UIArea.InventoryBottom) {
            if (selectedIndex + columns < inventoryBottomCount) {
                selectedIndex += columns;
            }
            // 下端に到達したら装備スロットに移動
            else {
                currentArea = UIArea.Equip;
                selectedIndex = Mathf.Min(selectedIndex, equipUI.Count - 1);
            }
        }
        else if (currentArea == UIArea.Craft) {
            // クラフト → 装備
            currentArea = UIArea.Equip;
            selectedIndex = Mathf.Min(selectedIndex, equipUI.Count - 1);
        }
        UpdateHighlight();
    }

    public void MoveUp() {
        if (!IsInventoryArea() && currentArea != UIArea.Craft && currentArea != UIArea.Equip) return;

        switch (currentArea) {
            case UIArea.InventoryTop:
                if (selectedIndex >= columns)
                    selectedIndex -= columns;
                break;

            case UIArea.InventoryBottom:
                if (selectedIndex < columns) {
                    currentArea = UIArea.InventoryTop;
                    selectedIndex += inventoryTopCount - columns;
                }
                else {
                    selectedIndex -= columns;
                }
                break;

            case UIArea.Equip:
                // 装備 → クラフト
                currentArea = UIArea.Craft;
                selectedIndex = Mathf.Min(selectedIndex, craftUI.Count - 1);
                break;

            case UIArea.Craft:
                // クラフト → 上部インベントリに移動するなども可能
                currentArea = UIArea.InventoryTop;
                selectedIndex = Mathf.Min(selectedIndex, inventoryTopUI.Count - 1);
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

    public void HandleSelect() {
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

            slot.Clear();

            originArea = currentArea;
            originIndex = selectedIndex;
        }
        else {
            // 移動先がクラフト結果スロットなら何も置かない
            if (currentArea == UIArea.Craft && selectedIndex == 2) {
                return;
            }
            // 同じマスに戻そうとしている場合
            if (originArea == currentArea && originIndex == selectedIndex) {
                // 元スロットにそのまま戻す
                slot.SetItem(heldItem, heldAmount);

                // ここで必ずクリアする
                heldItem = null;
                heldAmount = 0;
                originIndex = -1;
            }
            else {
                // 移動先に同じアイテムがあるなら重ねる
                if (slot.item != null && slot.item == heldItem) {
                    slot.amount += heldAmount;

                    // 持ち物をクリア
                    heldItem = null;
                    heldAmount = 0;
                    originIndex = -1;
                }
                else {
                    // 移動先に何かある場合もない場合も入れ替え
                    slot.SetItem(heldItem, heldAmount);

                    // 元スロットに元々あったアイテムを戻す
                    if (originIndex >= 0) {
                        var originSlot = GetSlot(originArea, originIndex);
                        originSlot.SetItem(slotItem, slotAmount);
                    }

                    // 装備スロットに置いた場合は即反映
                    if (currentArea == UIArea.Equip) {
                        ApplyEquipment(selectedIndex);
                    }

                    // 持ち物をクリア
                    heldItem = null;
                    heldAmount = 0;
                    originIndex = -1;
                }
            }
        }

        RefreshUI();
        UpdateHeldItemUI();
        // --- クラフト結果更新 ---
        CraftManager.Instance.UpdateResult(); // これで craftSlots[2] に結果をセット
        // 装備スロットなら常に反映
        if (currentArea == UIArea.Equip) {
            ApplyEquipment(selectedIndex);
        }
    }

    public void DropHeldOne() {
        if (heldItem == null || heldAmount <= 0) return;

        var slot = GetCurrentSlot();
        if (slot == null) return;

        // 移動先がクラフト結果スロットなら何も置かない
        if (currentArea == UIArea.Craft && selectedIndex == 2) {
            return;
        }

        // 空 or 同じアイテムなら置ける
        if (slot.item == null) {
            slot.SetItem(heldItem, 1);
            heldAmount -= 1;
        }
        else if (slot.item == heldItem) {
            slot.amount += 1;
            heldAmount -= 1;
        }
        else {
            // 違うアイテムがある場合は置けない
            Debug.Log("このスロットには他のアイテムが入っています");
            return;
        }

        // 全部置いたら持ち物をクリア
        if (heldAmount <= 0) {
            heldItem = null;
            originIndex = -1;
        }

        RefreshUI();
        UpdateHeldItemUI();

        // 装備スロットなら即反映
        if (currentArea == UIArea.Equip) {
            ApplyEquipment(selectedIndex);
        }

        // クラフト結果更新
        CraftManager.Instance.UpdateResult();
    }

    public void HandleCancel() {
        if (heldItem != null && originIndex >= 0) {
            var originSlot = GetSlot(originArea, originIndex);

            if (originSlot.item == null) {
                // 空ならそのまま戻す
                originSlot.SetItem(heldItem, heldAmount);
            }
            else if (originSlot.item == heldItem) {
                // 同じアイテムなら合算して戻す
                originSlot.amount += heldAmount;
            }
            else {
                // 本来キャンセル時はここに入らないはず
                Debug.LogWarning("Cancel時にオリジンスロットに別アイテムが残っている！");
            }

            // ここで必ずクリア
            heldItem = null;
            heldAmount = 0;
            originIndex = -1;

            RefreshUI();
            UpdateHeldItemUI();
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

    public void UseSelectedItem() {
        var slot = GetCurrentSlot();
        if (slot == null || slot.item == null) return;

        if (slot?.item is HealItem healItem) {
            healItem.Use(GameObject.FindGameObjectWithTag("Player"));
            slot.amount--;
            if (slot.amount <= 0) slot.item = null;
        }
        if (slot?.item is AttackBoost ATKboost) {
            ATKboost.Use(GameObject.FindGameObjectWithTag("Player"));
            slot.amount--;
            if (slot.amount <= 0) slot.item = null;
        }
        if (slot?.item is DefenceBoost DEFboost) {
            DEFboost.Use(GameObject.FindGameObjectWithTag("Player"));
            slot.amount--;
            if (slot.amount <= 0) slot.item = null;
        }
        RefreshUI();
        UpdateHeldItemUI();
        UpdateHighlight();
    }
    private void ApplyEquipment(int slotIndex) {
        PlayerBase player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBase>();
        if (player == null) return;

        if (slotIndex == 0) { // 武器スロット
            if (equipSlots[0].item is WeaponBase weapon) player.EquipWeapon(weapon);
            else player.EquipWeapon(null); // 空なら装備解除
        }
        else if (slotIndex == 1) { // 防具スロット
            if (equipSlots[1].item is ArmorBase armor) player.EquipArmor(armor);
            else player.EquipArmor(null); // 空なら装備解除
        }
    }
    #endregion

    public string GetHeldItemName() {
        if (heldItem == null) return "";
        return heldItem.itemName; // ItemBase に itemName がある前提
    }
}
