using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

// バフ用クラス
[System.Serializable]
public class TemporaryBuff {
    public int amount;
    public float duration;

    public TemporaryBuff(int amount, float duration) {
        this.amount = amount;
        this.duration = duration;
    }
}

/// <summary>
/// プレイヤーコントローラー（移動・回避・攻撃・スキル統合）
/// 攻撃中・スキル中は移動不可
/// </summary>
public class PlayerBase : MonoBehaviour {
    public PlayerInput input;
    private NewPlayerMove pMove;
    private NewPlayerAttack pAttack;
    private Animator anim;
    private Transform mainCamera;
    private Vector2 moveInput;
    [NonSerialized]public SkillCooldownUI skillUI;
    public GameObject Canvas;
    private bool isNearQuestBoard = false;

    [Header("プレイヤーのステータス")]
    [SerializeField] public int hp;
    [SerializeField] public int maxHp;
    [SerializeField] public int baseDefence;
    [SerializeField] public int defence;
    [SerializeField] public int baseAttack;
    [SerializeField] public int attack;
    [SerializeField] public float stamina;
    public GatheringPoint point;

    // バフ用
    private List<TemporaryBuff> attackBuffs = new List<TemporaryBuff>();
    private List<TemporaryBuff> defenceBuffs = new List<TemporaryBuff>();

    //装備関係
    private WeaponBase currentWeapon;
    private ArmorBase currentArmor;
    [Header("会心系ステータス")]
    [SerializeField] public float criticalChance;     // 会心率（最大値は1.00f）
    [SerializeField] public float criticalMultiplier; // 会心ダメージ倍率
    private bool isCritical = false;                  // クリティカル判定

    [Header("攻撃判定用コライダー")]
    [SerializeField] private GameObject attackCollider1;
    [SerializeField] private GameObject attackCollider2;
    [SerializeField] private GameObject attackCollider3;

    [Header("リスポーン地点（街の初期位置保存用）")]
    [SerializeField] private GameObject startPos;

    [Header("軌跡")]
    [SerializeField] private GameObject trailObject;
    private TrailRenderer swordTrail;

    // 行動制御フラグ
    private bool isDead = false;
    private bool isPick = false;

    // スキル制御
    private SkillManager skillManager;        // スキル管理
    public bool IsSkillActive { get; private set; } = false; // スキル中フラグ

    public InputAction GatherAction;
    public InputAction OpenInventoryAction;
    public InputAction OpenQuestAction;
    public InputAction UseItemAction;
    public InputAction DropItemAction;

    private Vector3 effectSpawnPos;

    // UI用カーソル移動
    private Vector2 uiMoveInput;

    [SerializeField]
    private GameObject attackBuffEffect;
    [SerializeField]
    private GameObject defBuffEffect;

    void Start() {     
        AudioManager.Instance.PlayBGM("Fantasy");
        Canvas.SetActive(false);
        input = GetComponent<PlayerInput>();
        anim = GetComponent<Animator>();
        mainCamera = Camera.main.transform;

        pMove = new NewPlayerMove(transform, anim);
        pAttack = new NewPlayerAttack(anim, attackCollider1, attackCollider2, attackCollider3);

        swordTrail = trailObject.GetComponent<TrailRenderer>();

        // スキルマネージャーを生成してスキルを登録
        skillManager = new SkillManager();
        skillManager.RegisterSkill(new FrontSlashSkill());

        // Gameplay
        input.actions["Move"].performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.actions["Move"].canceled += ctx => moveInput = Vector2.zero;
        input.actions["Attack"].performed += ctx => { if (!IsSkillActive) pAttack.Attack(); };
        input.actions["Avoid"].performed += ctx => { if (!IsSkillActive && !pAttack.IsAttacking()) pMove.Avoid(moveInput, mainCamera); };
        input.actions["Skill"].performed += ctx => { if (!IsSkillActive && !pAttack.IsAttacking() && !isDead && !isPick) skillManager.UseSkill(0, gameObject); };
        GatherAction = input.actions["Gather"];
        OpenInventoryAction = input.actions["OpenInventory"];
        OpenQuestAction = input.actions["OpenQuest"];
        DropItemAction = input.actions["DropItem"];

        // UI
        input.actions["MoveMenu"].performed += ctx => uiMoveInput = ctx.ReadValue<Vector2>();
        input.actions["MoveQuest"].performed += ctx => moveInput = ctx.ReadValue<Vector2>();

        UseItemAction = input.actions["UseItem"];
        // UseItem が押されたら InventoryManager に通知
        UseItemAction.performed += ctx => {
            // インベントリが開いている場合のみ
            if (input.currentActionMap.name == "Inventory") {
                InventoryManager.Instance.UseSelectedItem();
            }
        };
        DropItemAction.performed += ctx => {
            // インベントリが開いている場合のみ処理
            if (input.currentActionMap.name == "Inventory") {
                InventoryManager.Instance.DropHeldOne();
            }
        };

        //スキルクールダウンの取得
        skillUI = GameObject.FindGameObjectWithTag("PlayerUI").GetComponent<SkillCooldownUI>();
    }

    void Update() {
        //  クールタイムUI用
        skillManager.Update(Time.deltaTime);

        if (input.currentActionMap.name == "Gameplay") {
            if (!IsSkillActive && !pAttack.IsAttacking() && !isDead && !isPick)
                pMove.Move(moveInput, mainCamera);
            pAttack.Update();
            if (OpenInventoryAction.WasPressedThisFrame()) OpenInventory();
            if (OpenQuestAction.WasPressedThisFrame() && isNearQuestBoard) {
                OpenQuest();
            }
        }
        else if (input.currentActionMap.name == "Inventory") {
            HandleUI();
        }
        else if (input.currentActionMap.name == "Quest") {
            HandleQuestUI();
        }

        if (transform.position.y <= -10) {
            DeathAnimationEnd();
        }

        // 攻撃バフ処理
        for (int i = attackBuffs.Count - 1; i >= 0; i--) {
            attackBuffs[i].duration -= Time.deltaTime;
            if (attackBuffs[i].duration <= 0) attackBuffs.RemoveAt(i);
            attackBuffEffect.SetActive(true);
        }

        // 防御バフ処理
        for (int i = defenceBuffs.Count - 1; i >= 0; i--) {
            defenceBuffs[i].duration -= Time.deltaTime;
            if (defenceBuffs[i].duration <= 0) defenceBuffs.RemoveAt(i);
            defBuffEffect.SetActive(true);
        }

        int attackBuff = attackBuffs.Count;
        int defBuff = defenceBuffs.Count;
        if(attackBuff <= 0) attackBuffEffect.SetActive(false);
        if(defBuff <= 0) defBuffEffect.SetActive(false);

        UpdateStatsWithBuffs();
    }

    void OpenQuest() {
        QuestBoardManager.Instance.OpenQuestBoard();
        skillUI.HideUI();
        input.SwitchCurrentActionMap("Quest");       // Questアクションマップに切り替え
    }

    void CloseQuest() {
        QuestBoardManager.Instance.CloseQuestBoard();
        skillUI.ShowUI();
        input.SwitchCurrentActionMap("Gameplay");    // 戻す
    }

    void HandleQuestUI() {

        // 移動
        if (moveInput.y > 0) { QuestBoardManager.Instance.MoveUp(); AudioManager.Instance.PlaySE("Cursol"); }
            if (moveInput.y < 0) { QuestBoardManager.Instance.MoveDown(); AudioManager.Instance.PlaySE("Cursol"); }
                if (moveInput.x > 0) { QuestBoardManager.Instance.MoveRight(); AudioManager.Instance.PlaySE("Cursol"); }
                    if (moveInput.x < 0) { QuestBoardManager.Instance.MoveLeft(); AudioManager.Instance.PlaySE("Cursol"); }

        // 決定
        if (input.actions["Select"].WasPressedThisFrame()) {
            QuestBoardManager.Instance.AcceptQuest();
            AudioManager.Instance.PlaySE("Select");
        }

        // キャンセル
        if (input.actions["Cancel"].WasPressedThisFrame()) {
            CloseQuest(); // QuestUIを閉じてGameplayに戻す
        }
        moveInput = Vector2.zero; // 移動入力もリセット
    }

    void OpenInventory() {
        InventoryManager.Instance.OpenInventory();
        skillUI.HideUI();
        input.SwitchCurrentActionMap("Inventory");
    }

    void CloseInventory() {
        InventoryManager.Instance.CloseInventory();
        skillUI.ShowUI();
        input.SwitchCurrentActionMap("Gameplay");
    }

    void HandleUI() {

        // カーソル移動
        if (uiMoveInput.y > 0) { InventoryManager.Instance.MoveUp(); AudioManager.Instance.PlaySE("Cursol"); }
        if (uiMoveInput.y < 0) { InventoryManager.Instance.MoveDown(); AudioManager.Instance.PlaySE("Cursol"); }
        if (uiMoveInput.x > 0) { InventoryManager.Instance.MoveRight(); AudioManager.Instance.PlaySE("Cursol"); }
        if (uiMoveInput.x < 0) { InventoryManager.Instance.MoveLeft(); AudioManager.Instance.PlaySE("Cursol"); }

        if (input.actions["Select"].WasPressedThisFrame()) {
            InventoryManager.Instance.HandleSelect();
            AudioManager.Instance.PlaySE("Select");
        }
        if (input.actions["Cancel"].WasPressedThisFrame()) {
            if (InventoryManager.Instance.IsHoldingItem) {
                InventoryManager.Instance.HandleCancel(); // 持っているものを戻す
            }
            else {
                CloseInventory(); // 何も持っていなければインベントリ閉じる
            }
        }

        uiMoveInput = Vector2.zero; // 移動入力もリセット
    }

    // ダメージ処理
    public virtual void TakeDamage(int power, float motionMultiplier = 1f,
                               float criticalChance = 0f, float criticalMultiplier = 2f,
                               int elementalValue = 0, float staggerValue = 0f) {
        if (isDead || IsSkillActive) return;
        isPick = false;

        // 装備やバフ込みの防御力を使う
        int effectiveDefence = defence; // baseDefence + 装備/バフ

        // クリティカル判定
        isCritical = Random.value < criticalChance;

        float randomFactor = UnityEngine.Random.Range(0.90f, 1.10f);
        int damage = Mathf.Max(1, power - effectiveDefence); // 基本ダメージ計算

        if (isCritical) {
            damage = Mathf.RoundToInt(damage * criticalMultiplier);
        }

        // モーション倍率・ランダム補正
        damage = Mathf.RoundToInt(damage * motionMultiplier * randomFactor);

        // HP 減少
        hp -= damage;

        AttackEndEvent();

        if (hp <= 0) Dead();
        else anim.SetTrigger("Hit");
        if(point != null) GatherEnd();
        effectSpawnPos = new Vector3(transform.position.x, transform.position.y + 0.6f, transform.position.z);
        EffectManager.Instance.SpawnEffect("AttackHitEffect", effectSpawnPos, Quaternion.identity, 0.5f);

        Debug.Log($"Damage Taken: {damage} {(isCritical ? "(Critical!)" : "")} HP: {hp}/{maxHp}");
    }

    private void Dead() {
        if (hp <= 0) {
            isDead = true;
            anim.SetTrigger("Death");
            AudioManager.Instance.PlaySE("YOUDIED");
            GameManager.Instance.OnDead();
        }
    }

    public void DeathAnimationEnd() {
        isDead = false;
        HitCancell();
        transform.position = startPos.transform.position;
        hp = maxHp;
    }

    // PlayerBase.cs の一部
    public SkillManager GetSkillManager() {
        return skillManager;
    }

    //採取関係
    private void OnTriggerStay(Collider other) {
        if (other.gameObject.CompareTag("GatheringPoint") || other.gameObject.CompareTag("Shop")) {
            Canvas.SetActive(true);
            if (GatherAction.WasPressedThisFrame()&&isPick==false) {
                point = other.gameObject.GetComponent<GatheringPoint>();
                if (point != null && point.IsAvailable && !pMove.IsMoving()) {
                    anim.SetTrigger("Pick");
                    isPick = true;

                }
            }
        }
    }
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("QuestBoard")) {
            isNearQuestBoard = true;
            Canvas.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("QuestBoard")) {
            isNearQuestBoard = false;
            Canvas.SetActive(false);
        }
        if (other.gameObject.CompareTag("GatheringPoint") || other.gameObject.CompareTag("Shop")) {
            Canvas.SetActive(false);
            point = null;
        }
    }

    //アイテム関係の処理
    public void Heal(int amount) {
        AudioManager.Instance.PlaySE("Skill");
        hp += amount;
        if (hp >= maxHp) {
            hp = maxHp;
        }
    }

    // 装備＋バフ込みで攻撃力と防御力を更新
    private void UpdateStatsWithBuffs() {
        attack = baseAttack;
        if (currentWeapon != null) attack += currentWeapon.weaponAttack;
        foreach (var buff in attackBuffs) attack += buff.amount;

        defence = baseDefence;
        if (currentArmor != null) defence += currentArmor.armorDefence;
        foreach (var buff in defenceBuffs) defence += buff.amount;
    }

    // アイテム使用時に呼ぶ
    public void AttackBoost(int amount, float duration) {
        attackBuffs.Add(new TemporaryBuff(amount, duration));
        UpdateStatsWithBuffs();
    }

    public void DefenceBoost(int amount, float duration) {
        defenceBuffs.Add(new TemporaryBuff(amount, duration));
        UpdateStatsWithBuffs();
    }
    public void BaseAttackBoost(int amount) {
        baseAttack += amount;
    }
    public void BaseDefenceBoost(int amount) {
        baseDefence += amount;
    }
    public void MaxHpBoost(int amount) {
        maxHp += amount;
    }

    public void UpdateEquipmentStats() {
        attack = baseAttack;
        defence = baseDefence;

        if (currentWeapon != null) {
            attack += currentWeapon.weaponAttack;
            attack += currentWeapon.weaponAttack;
        }
        else {
            attack = baseAttack;
        }
        if (currentArmor != null) {
            defence += currentArmor.armorDefence;
        }
        else {
            defence = baseDefence;
        }
    }
    public void EquipWeapon(WeaponBase weapon) {
        currentWeapon = weapon;
        UpdateEquipmentStats();
    }

    public void EquipArmor(ArmorBase armor) {
        currentArmor = armor;
        UpdateEquipmentStats();
    }

    //アニメーション関係の処理----------------------------------------------------------------------------
    public void GatherStart() {
        point.Interact();
    }

    public void GatherEnd() {
        point.StartCol();
    }
    // Animatorイベント用ラッパー
    public void AttackStartEvent() {
        swordTrail.emitting = true;
        pAttack.AttackStart();
        pAttack.SetPower(attack);
    }

    public void AttackEndEvent() {
        swordTrail.emitting = false;
        pAttack.AttackEnd();
    }

    public void ResetAttackFlag() {
        swordTrail.emitting = false;
        pAttack.ResetAttack();
    }

    public void PickEnd() {
        isPick = false;
    }

    // スキルフラグ制御（スキルクラスから呼び出される）
    public void SetSkillActive(bool active) {
        IsSkillActive = active;
    }

    public void EndSkillEvent() {
        SetSkillActive(false);
    }

    public void HitCancell() {
        // 攻撃、回避、スキルなどのフラグをfalseにリセットする例
        anim.ResetTrigger("Attack");
        anim.ResetTrigger("Avoid");
        anim.ResetTrigger("Skill");
        anim.ResetTrigger("Pick");
        anim.SetBool("Run",false);

        ResetAttackFlag();
        IsSkillActive = false;
        isPick = false;
        isDead = false;
    }
}
