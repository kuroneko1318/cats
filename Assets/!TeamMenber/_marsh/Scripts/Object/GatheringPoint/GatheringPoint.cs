using System;
using System.Collections;
using UnityEngine;

//新しい採取ポイントを追加する場合は、GatheringPoint を継承して Gather() をオーバーライドする

//必要であればタイプを追加する
public enum GatheringPointType {
    Bush,
    Ore,
    Cactus,
    Lava,
}

public abstract class GatheringPoint : MonoBehaviour {
    public GatheringPointType pointType;
    public float respawnTime = 30f;
    protected bool isAvailable = true;
public bool IsAvailable => isAvailable;
    [NonSerialized]public Inventory bag;

    public virtual void Awake() {
        // 基本は InventoryManager の bag を参照
        bag = InventoryManager.Instance?.bag;

        if (bag == null) {
            // 万一 null ならシーン内の Inventory を探す
            bag = GameObject.FindGameObjectWithTag("bag")?.GetComponent<Inventory>();
            if (bag == null)
                Debug.LogError($"[{pointType}] Inventoryが見つかりません！");
        }
    }

    public virtual void Interact() {
        if (!isAvailable) return;
        if (bag == null) {
            bag = GameObject.FindGameObjectWithTag("bag")?.GetComponent<Inventory>();
        }

        Debug.Log($"採取: {pointType}");
        Gather();
        StartCoroutine(RespawnCoroutine());
    }

    public abstract void Gather();

    private IEnumerator RespawnCoroutine() {
        isAvailable = false;
        yield return new WaitForSeconds(respawnTime);
        isAvailable = true;
    }
}

