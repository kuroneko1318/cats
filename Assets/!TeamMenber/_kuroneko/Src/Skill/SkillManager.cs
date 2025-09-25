using System.Collections.Generic;
using UnityEngine;

public class SkillManager {
    // 登録されたスキルの一覧
    private List<SkillBase> skillList = new List<SkillBase>();

    // スキルごとのクールタイム残り時間を管理するリスト
    private List<float> cooldownTimers = new List<float>();

    // スキルを登録する（重複は無視）
    public void RegisterSkill(SkillBase skill) {
        if (!skillList.Contains(skill)) {
            skillList.Add(skill);
            cooldownTimers.Add(0f); // 初期値は0（すぐ使える）
        }
    }

    // 指定インデックスのスキルを発動する
    public void UseSkill(int index, GameObject user) {
        if (index >= 0 && index < skillList.Count) {
            // まだクールタイム中なら無視
            if (cooldownTimers[index] > 0f) return;

            skillList[index].Activate(user);
            cooldownTimers[index] = skillList[index].Cooldown; // クールタイムをセット
        }
    }

    // 毎フレーム呼ぶことでクールタイムを減らす
    public void Update(float deltaTime) {
        for (int i = 0; i < cooldownTimers.Count; i++) {
            if (cooldownTimers[i] > 0f) {
                cooldownTimers[i] -= deltaTime;
                if (cooldownTimers[i] < 0f) cooldownTimers[i] = 0f;
            }
        }
    }

    // 登録されているスキルの数を取得する
    public int GetSkillCount() {
        return skillList.Count;
    }

    // 登録済みスキルの名前一覧を取得（UIなどに利用可）
    public List<string> GetSkillNames() {
        List<string> names = new List<string>();
        foreach (var s in skillList)
            names.Add(s.SkillName);
        return names;
    }

    // 指定スキルの残りクールタイムを取得
    public float GetRemainingCooldown(int index) {
        if (index >= 0 && index < cooldownTimers.Count)
            return cooldownTimers[index];
        return 0f;
    }

    // 指定スキルの最大クールタイムを取得
    public float GetCooldownTime(int index) {
        if (index >= 0 && index < skillList.Count)
            return skillList[index].Cooldown;
        return 0f;
    }
}
