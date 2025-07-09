using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBase
{
    // スキル名（UIやデバッグ用）
    public string SkillName { get; protected set; }

    // スキルのクールタイム（再使用までの待機時間）
    public float Cooldown { get; protected set; }

    // 最後に使用した時間（クールタイム管理に使用）
    protected float lastUseTime = -999f;

    //  現在このスキルが使用可能かどうかを判定（クールタイムを考慮）
    public virtual bool CanUse() {
        return Time.time >= lastUseTime + Cooldown;
    }

    //  スキル本体の処理（継承先で具体的な内容を実装する）
    public abstract void Activate(GameObject user);
}
