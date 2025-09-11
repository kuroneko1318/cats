using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerAttack
{
    private Animator anim;
    private int comboStep = 0;        // 今のコンボ段階
    private bool comboQueued = false; // 次攻撃予約フラグ

    public NewPlayerAttack(Animator animator) {
        anim = animator;
    }

    // 攻撃入力を受け付ける
    public void OnAttackInput() {
        if (comboStep > 0) {
            comboQueued = true; // 攻撃中なら次予約
            return;
        }

        StartCombo(1); // 最初の攻撃開始
    }

    // コンボ開始
    private void StartCombo(int step) {
        ResetAllAttackBools();
        comboStep = step;
        anim.SetBool("Attack" + step, true); // Attack1 / Attack2 / Attack3 をON
    }

    // アニメーション終了時に呼ぶ（AnimationEvent推奨）
    public void OnAttackAnimationEnd() {
        anim.SetBool("Attack" + comboStep, false); // 今の段階を終了

        if (comboQueued && comboStep < 3) {
            comboQueued = false;
            StartCombo(comboStep + 1); // 次段へ
        }
        else {
            comboStep = 0; // 終了
            comboQueued = false;
        }
    }

    // 全ての攻撃フラグをOFF
    private void ResetAllAttackBools() {
        anim.SetBool("Attack1", false);
        anim.SetBool("Attack2", false);
        anim.SetBool("Attack3", false);
    }
}
