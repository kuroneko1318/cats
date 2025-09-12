using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// アイテムクラフト全体を管理するクラス  
/// ・プレイヤーインベントリ、クラフト用スロット、結果スロットを管理  
/// ・スロット間のアイテム移動、クラフト判定、UI更新を行う
/// </summary>
public class ItemCrafting : MonoBehaviour {
    [Header("UIコンテナ")]
    public RectTransform playerSlotsContainer; // プレイヤーインベントリ用のスロットを格納する親
    public RectTransform craftingSlotsContainer; // クラフト用スロットを格納する親
    public RectTransform resultSlotContainer; // 結果スロットを格納する親
    public Button craftButton; // クラフト実行ボタン
    public SlotTemplate slotTemplate; // スロットのテンプレート
    public SlotTemplate resultSlotTemplate; // 結果用スロットテンプレート

    public SlotContainer[] playerSlots; // プレイヤーインベントリスロット
    private SlotContainer[] craftSlots = new SlotContainer[9]; // クラフト用9スロット
    private SlotContainer resultSlot = new SlotContainer(); // クラフト結果スロット
    public Item[] items; // 全アイテムのリスト

    private SlotContainer selectedItemSlot = null; // 選択中のアイテムスロット
    private int craftTableID = -1; // クラフトスロットのテーブルID
    private int resultTableID = -1; // 結果スロットのテーブルID
    private ColorBlock defaultButtonColors; // ボタンの初期カラーを保持

    /// <summary>
    /// 初期化処理  
    /// ・スロットテンプレートやクラフトボタンを設定  
    /// ・スロットを生成・更新してUIを構築する
    /// </summary>
    void Start() {
        slotTemplate.container.rectTransform.pivot = new Vector2(0, 1);
        slotTemplate.container.rectTransform.anchorMax = slotTemplate.container.rectTransform.anchorMin = new Vector2(0, 1);
        slotTemplate.craftingController = this;
        slotTemplate.gameObject.SetActive(false);

        resultSlotTemplate.container.rectTransform.pivot = new Vector2(0, 1);
        resultSlotTemplate.container.rectTransform.anchorMax = resultSlotTemplate.container.rectTransform.anchorMin = new Vector2(0, 1);
        resultSlotTemplate.craftingController = this;
        resultSlotTemplate.gameObject.SetActive(false);

        craftButton.onClick.AddListener(PerformCrafting);
        defaultButtonColors = craftButton.colors;

        InitializeSlotTable(craftingSlotsContainer, slotTemplate, craftSlots, 5, 0);
        UpdateItems(craftSlots);
        craftTableID = 0;

        InitializeSlotTable(playerSlotsContainer, slotTemplate, playerSlots, 5, 1);
        UpdateItems(playerSlots);

        InitializeSlotTable(resultSlotContainer, resultSlotTemplate, new SlotContainer[] { resultSlot }, 0, 2);
        UpdateItems(new SlotContainer[] { resultSlot });
        resultTableID = 2;

        slotTemplate.container.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        slotTemplate.container.raycastTarget = slotTemplate.item.raycastTarget = slotTemplate.count.raycastTarget = false;
    }

    /// <summary>
    /// 指定したコンテナにスロットを生成してレイアウトを初期化する
    /// </summary>
    /// <param name="container">スロットを配置する親RectTransform</param>
    /// <param name="slotTemplateTmp">元となるスロットテンプレート</param>
    /// <param name="slots">生成先のスロット配列</param>
    /// <param name="margin">スロット間のマージン</param>
    /// <param name="tableIDTmp">テーブル識別ID</param>
    void InitializeSlotTable(RectTransform container, SlotTemplate slotTemplateTmp, SlotContainer[] slots, int margin, int tableIDTmp) {
        int resetIndex = 0;
        int rowTmp = 0;
        for (int i = 0; i < slots.Length; i++) {
            if (slots[i] == null) slots[i] = new SlotContainer();
            GameObject newSlot = Instantiate(slotTemplateTmp.gameObject, container.transform);
            slots[i].slot = newSlot.GetComponent<SlotTemplate>();
            slots[i].slot.gameObject.SetActive(true);
            slots[i].tableID = tableIDTmp;

            float xTmp = (int)((margin + slots[i].slot.container.rectTransform.sizeDelta.x) * (i - resetIndex));
            if (xTmp + slots[i].slot.container.rectTransform.sizeDelta.x + margin > container.rect.width) {
                resetIndex = i;
                rowTmp++;
                xTmp = 0;
            }
            slots[i].slot.container.rectTransform.anchoredPosition = new Vector2(
                margin + xTmp,
                -margin - ((margin + slots[i].slot.container.rectTransform.sizeDelta.y) * rowTmp)
            );
        }
    }

    /// <summary>
    /// スロットの表示をアイテムデータに基づいて更新する
    /// </summary>
    /// <param name="slots">更新対象のスロット配列</param>
    void UpdateItems(SlotContainer[] slots) {
        for (int i = 0; i < slots.Length; i++) {
            Item slotItem = FindItem(slots[i].itemSprite);
            if (slotItem != null) {
                if (!slotItem.stackable) slots[i].itemCount = 1;

                if (slots[i].itemCount > 1) {
                    slots[i].slot.count.enabled = true;
                    slots[i].slot.count.text = slots[i].itemCount.ToString();
                } else {
                    slots[i].slot.count.enabled = false;
                }
                slots[i].slot.item.enabled = true;
                slots[i].slot.item.sprite = slotItem.itemSprite;
            } else {
                slots[i].slot.count.enabled = false;
                slots[i].slot.item.enabled = false;
            }
        }
    }

    /// <summary>
    /// スプライトから対応するアイテムを検索する
    /// </summary>
    Item FindItem(Sprite sprite) {
        if (!sprite) return null;
        for (int i = 0; i < items.Length; i++) {
            if (items[i].itemSprite == sprite) return items[i];
        }
        return null;
    }

    /// <summary>
    /// レシピ文字列から対応するアイテムを検索する
    /// </summary>
    Item FindItem(string recipe) {
        if (recipe == "") return null;
        for (int i = 0; i < items.Length; i++) {
            if (items[i].craftRecipe == recipe) return items[i];
        }
        return null;
    }

    /// <summary>
    /// スロットクリック後の再チェックを行い、アイテムの移動・スタック・入れ替えを処理する
    /// </summary>
    public void ClickEventRecheck() {
        if (selectedItemSlot == null) {
            selectedItemSlot = GetClickedSlot();
            if (selectedItemSlot != null && selectedItemSlot.itemSprite != null) {
                selectedItemSlot.slot.count.color = selectedItemSlot.slot.item.color = new Color(1, 1, 1, 0.5f);
            } else {
                selectedItemSlot = null;
            }
        } else {
            SlotContainer newClickedSlot = GetClickedSlot();
            if (newClickedSlot != null) {
                bool swapPositions = false;
                bool releaseClick = true;

                if (newClickedSlot != selectedItemSlot) {
                    if (newClickedSlot.tableID == selectedItemSlot.tableID) {
                        if (newClickedSlot.itemSprite == selectedItemSlot.itemSprite) {
                            Item slotItem = FindItem(selectedItemSlot.itemSprite);
                            if (slotItem.stackable) {
                                selectedItemSlot.itemSprite = null;
                                newClickedSlot.itemCount += selectedItemSlot.itemCount;
                                selectedItemSlot.itemCount = 0;
                            } else swapPositions = true;
                        } else swapPositions = true;
                    } else {
                        if (resultTableID != newClickedSlot.tableID) {
                            if (craftTableID != newClickedSlot.tableID) {
                                if (newClickedSlot.itemSprite == selectedItemSlot.itemSprite) {
                                    Item slotItem = FindItem(selectedItemSlot.itemSprite);
                                    if (slotItem.stackable) {
                                        selectedItemSlot.itemSprite = null;
                                        newClickedSlot.itemCount += selectedItemSlot.itemCount;
                                        selectedItemSlot.itemCount = 0;
                                    } else swapPositions = true;
                                } else swapPositions = true;
                            } else {
                                if (newClickedSlot.itemSprite == null || newClickedSlot.itemSprite == selectedItemSlot.itemSprite) {
                                    newClickedSlot.itemSprite = selectedItemSlot.itemSprite;
                                    newClickedSlot.itemCount++;
                                    selectedItemSlot.itemCount--;
                                    if (selectedItemSlot.itemCount <= 0) selectedItemSlot.itemSprite = null;
                                    else releaseClick = false;
                                } else swapPositions = true;
                            }
                        }
                    }
                }

                if (swapPositions) {
                    Sprite previousItemSprite = selectedItemSlot.itemSprite;
                    int previousItemCount = selectedItemSlot.itemCount;
                    selectedItemSlot.itemSprite = newClickedSlot.itemSprite;
                    selectedItemSlot.itemCount = newClickedSlot.itemCount;
                    newClickedSlot.itemSprite = previousItemSprite;
                    newClickedSlot.itemCount = previousItemCount;
                }

                if (releaseClick) {
                    selectedItemSlot.slot.count.color = selectedItemSlot.slot.item.color = Color.white;
                    selectedItemSlot = null;
                }

                UpdateItems(playerSlots);
                UpdateItems(craftSlots);
                UpdateItems(new SlotContainer[] { resultSlot });
            }
        }
    }

    /// <summary>
    /// クリックされたスロットを検出して返す
    /// </summary>
    SlotContainer GetClickedSlot() {
        for (int i = 0; i < playerSlots.Length; i++) {
            if (playerSlots[i].slot.hasClicked) {
                playerSlots[i].slot.hasClicked = false;
                return playerSlots[i];
            }
        }
        for (int i = 0; i < craftSlots.Length; i++) {
            if (craftSlots[i].slot.hasClicked) {
                craftSlots[i].slot.hasClicked = false;
                return craftSlots[i];
            }
        }
        if (resultSlot.slot.hasClicked) {
            resultSlot.slot.hasClicked = false;
            return resultSlot;
        }
        return null;
    }

    /// <summary>
    /// クラフトボタンが押されたときにクラフト処理を行う  
    /// ・クラフトレシピを結合して一致するアイテムを検索  
    /// ・成功なら結果スロットに反映、失敗ならボタンを赤色にする
    /// </summary>
    void PerformCrafting() {
        string[] combinedItemRecipe = new string[craftSlots.Length];
        craftButton.colors = defaultButtonColors;

        for (int i = 0; i < craftSlots.Length; i++) {
            Item slotItem = FindItem(craftSlots[i].itemSprite);
            combinedItemRecipe[i] = (slotItem != null)
                ? slotItem.itemSprite.name + (craftSlots[i].itemCount > 1 ? "(" + craftSlots[i].itemCount + ")" : "")
                : "";
        }

        string combinedRecipe = string.Join(",", combinedItemRecipe);
        print(combinedRecipe);

        Item craftedItem = FindItem(combinedRecipe);
        if (craftedItem != null) {
            for (int i = 0; i < craftSlots.Length; i++) {
                craftSlots[i].itemSprite = null;
                craftSlots[i].itemCount = 0;
            }
            resultSlot.itemSprite = craftedItem.itemSprite;
            resultSlot.itemCount = 1;

            UpdateItems(craftSlots);
            UpdateItems(new SlotContainer[] { resultSlot });
        } else {
            ColorBlock colors = craftButton.colors;
            colors.selectedColor = colors.pressedColor = new Color(0.8f, 0.55f, 0.55f, 1);
            craftButton.colors = colors;
        }
    }

    /// <summary>
    /// 毎フレーム呼ばれる更新処理  
    /// ・選択中のアイテムをマウスに追従させる  
    /// ・選択が解除されたらテンプレートを非表示にする
    /// </summary>
    void Update() {
        if (selectedItemSlot != null) {
            if (!slotTemplate.gameObject.activeSelf) {
                slotTemplate.gameObject.SetActive(true);
                slotTemplate.container.enabled = false;
                slotTemplate.count.color = selectedItemSlot.slot.count.color;
                slotTemplate.item.sprite = selectedItemSlot.slot.item.sprite;
                slotTemplate.item.color = selectedItemSlot.slot.item.color;
            }
            slotTemplate.container.rectTransform.position = Input.mousePosition;
            slotTemplate.count.text = selectedItemSlot.slot.count.text;
            slotTemplate.count.enabled = selectedItemSlot.slot.count.enabled;
        } else {
            if (slotTemplate.gameObject.activeSelf) {
                slotTemplate.gameObject.SetActive(false);
            }
        }
    }
}