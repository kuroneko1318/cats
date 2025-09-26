using System.Collections;
using UnityEngine;

/// <summary>
/// 前方範囲斬りスキル
/// </summary>
public class FrontSlashSkill : SkillBase {
    private float attackTiming = 2.8f; // 攻撃発生までの時間
    private bool isRunning = false;    // スキル実行中フラグ

    public FrontSlashSkill() {
        SkillName = "前方範囲斬り";
        Cooldown = 15.0f; // クールタイム15秒
    }

    public override void Activate(GameObject user) {
        // 使用不可または実行中なら無視
        if (!CanUse() || isRunning) return;

        isRunning = true;

        // クールタイム開始
        lastUseTime = Time.time;

        // プレイヤー制御（スキル中は移動・攻撃不可）
        PlayerBase player = user.GetComponent<PlayerBase>();
        if (player != null)
            player.SetSkillActive(true);

        // アニメーション再生
        AudioManager.Instance.PlaySE("Skill");
        Animator anim = user.GetComponent<Animator>();
        if (anim != null)
            anim.SetTrigger("Skill");

        // チャージエフェクト生成
        Vector3 spawnPos = user.transform.position + user.transform.up * 0.1f;
        EffectManager.Instance.SpawnEffect("ChargeFrontSkill", spawnPos, Quaternion.identity, 2f);

        // 攻撃本体は遅延コルーチンで生成
        user.GetComponent<MonoBehaviour>().StartCoroutine(DelayedAttack(user));
    }

    private IEnumerator DelayedAttack(GameObject user) {
        // 攻撃発生まで待機
        yield return new WaitForSeconds(attackTiming);

        // 攻撃エフェクト生成
        Vector3 spawnPos = user.transform.position +
                           user.transform.forward * 0.5f +
                           user.transform.up * 0.2f;
        Quaternion spawnRot = user.transform.rotation * Quaternion.Euler(0f, -90f, 0f);

        EffectManager.Instance.SpawnEffect("FrontSkill", spawnPos, spawnRot, 1f);

        // スキル終了フラグ
        isRunning = false;

        Debug.Log($"{SkillName} の攻撃発生！");
    }
}
