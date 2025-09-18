using System.Collections;
using UnityEngine;

/// <summary>
/// 前方範囲斬りスキル
/// </summary>
public class FrontSlashSkill : SkillBase {
    private float attackTiming = 2.8f; // 攻撃発生までの時間
    private bool isRunning = false;

    public FrontSlashSkill() {
        SkillName = "前方範囲斬り";
        Cooldown = 5.0f;
    }

    public override void Activate(GameObject user) {
        if (!CanUse() || isRunning) return;

        isRunning = true;

        PlayerBase player = user.GetComponent<PlayerBase>();
        if (player != null)
            player.SetSkillActive(true); // スキル中は行動不可

        Animator anim = user.GetComponent<Animator>();
        if (anim != null)
            anim.SetTrigger("Skill");

        Vector3 spawnPos = user.transform.position + user.transform.up * 0.1f;
        EffectManager.Instance.SpawnEffect("ChargeFrontSkill", spawnPos, Quaternion.identity, 2f);

        user.GetComponent<MonoBehaviour>().StartCoroutine(DelayedAttack(user));
    }

    private IEnumerator DelayedAttack(GameObject user) {
        yield return new WaitForSeconds(attackTiming);

        Vector3 spawnPos = user.transform.position +
                           user.transform.forward * 0.5f +
                           user.transform.up * 0.2f;
        Quaternion spawnRot = user.transform.rotation * Quaternion.Euler(0f, -90f, 0f);

        EffectManager.Instance.SpawnEffect("FrontSkill", spawnPos, spawnRot, 1f);

        lastUseTime = Time.time;
        isRunning = false;

        Debug.Log($"{SkillName} の攻撃発生！");
    }
}
