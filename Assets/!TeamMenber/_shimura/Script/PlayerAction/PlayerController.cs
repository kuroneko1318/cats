
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : PlayerBase {

    

    void Start() {
        input = GetComponent<PlayerInput>();
        if (input == null) {
            Debug.LogError("PlayerInput が取得できませんでした");
            return;
        }

        moveAction = input.actions["Move"];
        avoidanceAction = input.actions["Avoidance"];
        GatherAction = input.actions["Gather"];

        cam = GameObject.Find("Main Camera");

        initialPosition = transform.position;


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
        if (attackFlag == false) {
            PlayerMove();
            RotMove();
            Avoidance();
            Stop();
        }
        SetDamage(attack);
        attacker = attack;
        PlayAttackSEOnAnimationStart();
        LowAttack();             // 攻撃処理
        HandleAttackCooldown();  // クールタイム処理
        CheckAttackAnimationEnd();
        if (Keyboard.current.digit1Key.wasPressedThisFrame) {
            
            skillManager.UseSkill(0, gameObject);
           
            
        }
    }

    

  

   

    

    
}
