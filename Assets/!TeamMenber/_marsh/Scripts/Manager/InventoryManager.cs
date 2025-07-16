using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class InventoryManager : SystemObject<InventoryManager> {

    [Header("InventoryData")]
    public Inventory inventory; // インスペクターでインベントリオブジェクトの設定
    [NonSerialized]public Inventory bag;

    [Header("UIプレハブ")]
    public GameObject inventoryPanelPrefab;
    public GameObject itemTextPrefab;

    private GameObject inventoryPanelInstance;
    public Transform contentParent; // スクロールできるようにするための親の位置

    public override void Initialize() {
        bag = Instantiate(inventory);
        
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.I)) {
            ToggleInventory();
        }

        if (Input.GetKeyDown(KeyCode.O)) {

            if (ItemManager.Instance.GetItemByID(4001) == null) Debug.LogError("ID 4001 のアイテムが null です");

            var item = ItemManager.Instance?.GetItemByID(4001);
            if (item != null && inventory != null) {
                bag.AddItem(item, 10);
            }

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
            contentParent = inventoryPanelInstance.transform.Find("ScrollView/Viewport/Content");
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
        // 既存の表示をクリア
        foreach (Transform child in contentParent) {
            Destroy(child.gameObject);
        }

        // スロットごとに表示
        foreach (var slot in bag.slots) {
            if (!slot.IsEmpty) {
                GameObject itemObj = Instantiate(itemTextPrefab, contentParent);
                var text = itemObj.GetComponent<TextMeshProUGUI>();
                text.text = $"{slot.item.itemName} x{slot.amount}";
            }
        }
    }
}
