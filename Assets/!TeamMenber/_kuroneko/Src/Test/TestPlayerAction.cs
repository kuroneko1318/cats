using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestPlayerAction : MonoBehaviour {
    private SkillManager skillManager;
    private DashSlashSkill dashSlashSkill;

    void Start() {
        skillManager = new SkillManager();

        dashSlashSkill = new DashSlashSkill();
        skillManager.RegisterSkill(dashSlashSkill);
    }

    void Update() {
        // 1キーで突進斬りを発動
        if (Keyboard.current.digit1Key.wasPressedThisFrame) {
            skillManager.UseSkill(0, gameObject);
        }

        // 毎フレーム突進状態の管理を呼ぶ
        dashSlashSkill.UpdateDash(gameObject);
    }
}
