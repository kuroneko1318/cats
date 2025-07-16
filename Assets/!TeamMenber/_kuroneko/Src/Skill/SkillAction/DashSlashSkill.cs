using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DashSlashSkill : SkillBase
{
    // 突進力（大きいほど遠く速く突進）
    private float dashForce = 100f;

    // 突進時間（秒）
    private float dashDuration = 0.1f;

    // 突進中かどうか
    private bool isDashing = false;

    // 突進開始時刻
    private float dashStartTime;

    public DashSlashSkill() {
        SkillName = "突進斬り";
        Cooldown = 1.0f; // 短めのクールタイムで連打感を出す
    }

    /// <summary>
    /// スキル発動処理
    /// </summary>
    /// <param name="user">使用者（プレイヤー）</param>
    public override void Activate(GameObject user) {
        // クールタイムチェック
        if (!CanUse() || isDashing) return;

        lastUseTime = Time.time;
        isDashing = true;
        dashStartTime = Time.time;

        Rigidbody rb = user.GetComponent<Rigidbody>();
        if (rb != null) {
            // 前方に瞬間的に力を加えて突進開始
            Vector3 forward = user.transform.forward;
            rb.AddForce(forward * dashForce, ForceMode.VelocityChange);
        }

        Debug.Log($"{SkillName} 発動！ 突進開始");
    }

    /// <summary>
    /// 毎フレーム呼び出して突進時間が経過したら突進状態を解除する
    /// </summary>
    /// <param name="user"></param>
    public void UpdateDash(GameObject user) {
        if (!isDashing) return;

        float elapsed = Time.time - dashStartTime;

        if (elapsed >= dashDuration) {
            isDashing = false;
            Debug.Log($"{SkillName} 突進終了");

            // ここで速度を抑えたり、突進終了の演出を入れることも可能
            Rigidbody rb = user.GetComponent<Rigidbody>();
            if (rb != null) {
                rb.velocity = Vector3.zero; // 突進後に速度リセット
            }
        }
    }
}
