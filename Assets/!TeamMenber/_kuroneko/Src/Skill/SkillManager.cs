using System.Collections.Generic;
using UnityEngine;


// SkillManagerクラス 
// 複数のスキル(SkillBase継承クラス)をリストで管理し、順番（インデックス）で呼び出せるマネージャー 
// 【使い方】 
// 1. スキルを作成し、SkillBaseを継承する 
// 2. SkillManagerのインスタンスを作成する 
// 3. RegisterSkill() メソッドでスキルを登録する 
// 4. UseSkill(index, user) メソッドで登録したスキルを呼び出す indexは登録順の0から始まる番号 
// 5. GetSkillCount() で登録されているスキル数を取得できるので、UI等で活用可能 
// 6. GetSkillNames() で登録スキルの名前リストを取得可能 
// 【例】 // Start等で使用↓ 
// SkillManager skillManager = new SkillManager(); 
// skillManager.RegisterSkill(new スキルのコンストラクタ);
// Updateで使用↓
// skillManager.UseSkill(Listで登録されたインデックス, スキル使用者);
// 【注意】 
// スキルはリストの順番で管理されるため、インデックスを間違えない
// 同じスキルインスタンスは重複登録されない

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
}
