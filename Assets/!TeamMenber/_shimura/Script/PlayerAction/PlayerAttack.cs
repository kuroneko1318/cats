
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack :PlayerBase {
    

    

    

    

    void Start() {
        attackCollider.enabled = false;
        // PlayerInput から Attack アクションを取得
        input = GetComponent<PlayerInput>();
        attackAction = input.actions["Attack"];
        if (attackAction == null) {
            Debug.LogError("Attack アクションが見つかりません");
        }
        skillManager = new SkillManager();

        frontSlashSkill = new FrontSlashSkill();
        skillManager.RegisterSkill(frontSlashSkill);
    }

    void Update() {
        PlayAttackSEOnAnimationStart();
        LowAttack();             // 攻撃処理
        HandleAttackCooldown();  // クールタイム処理
        CheckAttackAnimationEndr();
        if (Keyboard.current.digit1Key.wasPressedThisFrame) {
            skillManager.UseSkill(0, gameObject);
        }
    }

  


    
}
