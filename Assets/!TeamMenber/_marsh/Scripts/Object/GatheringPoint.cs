using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//新しい採取ポイントを追加する場合は、GatheringPoint を継承して Gather() をオーバーライドする

//必要であればタイプを追加する
public enum GatheringPointType {
    Bush,
    Ore,
}

public abstract class GatheringPoint : MonoBehaviour {
    public GatheringPointType pointType;
    public float respawnTime = 30f;
    protected bool isAvailable = true;

    public virtual void Interact() {
        if (!isAvailable) return;

        Debug.Log($"採取: {pointType}");
        Gather();
        StartCoroutine(RespawnCoroutine());
    }

    protected abstract void Gather();

    private IEnumerator RespawnCoroutine() {
        isAvailable = false;
        yield return new WaitForSeconds(respawnTime);
        isAvailable = true;
    }
}

