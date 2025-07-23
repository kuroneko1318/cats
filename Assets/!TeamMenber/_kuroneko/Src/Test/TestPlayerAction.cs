using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestPlayerAction : MonoBehaviour {
    private SkillManager skillManager;
    private FrontSlashSkill frontSlashSkill;

    void Start() {
        skillManager = new SkillManager();

        frontSlashSkill = new FrontSlashSkill();
        skillManager.RegisterSkill(frontSlashSkill);
    }

    void Update() {
        // 1ÉLÅ[Ç≈ìÀêiéaÇËÇî≠ìÆ
        if (Keyboard.current.digit1Key.wasPressedThisFrame) {
            skillManager.UseSkill(0, gameObject);
        }

    }
}
