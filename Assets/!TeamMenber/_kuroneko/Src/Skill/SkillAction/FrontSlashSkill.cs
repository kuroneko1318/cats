using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FrontSlashSkill : SkillBase
{
    private float attackTiming = 2.8f;         // 攻撃発生までの時間（秒）
    private bool isRunning = false;    // 発動中フラグ

    public FrontSlashSkill() {
        SkillName = "前方範囲斬り";
        Cooldown = 5.0f;
    }

    /// <summary>
    /// スキル発動時の処理
    /// </summary>
    public override void Activate(GameObject user) {
        if (!CanUse() || isRunning) return;

        isRunning = true; // 発動開始

        // Animator を取得
        Animator anim = user.GetComponent<Animator>();
        if (anim != null) {
            anim.SetTrigger("Skill"); // 攻撃アニメーション再生
        }

        // 攻撃発生をタイミングに合わせて遅延実行
        user.GetComponent<MonoBehaviour>().StartCoroutine(DelayedAttack(user));
    }

    /// <summary>
    /// 指定時間後に攻撃エフェクトを発生させる
    /// </summary>
    private System.Collections.IEnumerator DelayedAttack(GameObject user) {
        yield return new WaitForSeconds(attackTiming);

        Transform userPos = user.transform;

        Vector3 spawnPos = userPos.position + user.transform.forward * 0.5f;
        Quaternion spawnRot = Quaternion.Euler(0f, -90f, 0f);

        // EffectManager を使ってエフェクトを生成
        EffectManager.Instance.SpawnEffect("FrontSkill", spawnPos, spawnRot, 1f);

        lastUseTime = Time.time;

        // 発動状態を解除
        isRunning = false;

        Debug.Log($"{SkillName} の攻撃発生！");
    }

}
