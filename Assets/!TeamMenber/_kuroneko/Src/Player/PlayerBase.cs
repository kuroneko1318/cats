using UnityEngine;
using UnityEngine.InputSystem;

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

    [Header("プレイヤーのステータス")]
    [SerializeField] public int hp;
    [SerializeField] public int maxHp;
    [SerializeField] public int defence;
    [SerializeField] public int attack;
    [SerializeField] public float stamina;

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

    // UI用カーソル移動
    private Vector2 uiMoveInput;

    void Start() {
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

        // UI
        input.actions["MoveMenu"].performed += ctx => uiMoveInput = ctx.ReadValue<Vector2>();
        input.actions["MoveQuest"].performed += ctx => moveInput = ctx.ReadValue<Vector2>();
    }

    void Update() {
        if (input.currentActionMap.name == "Gameplay") {
            if (!IsSkillActive && !pAttack.IsAttacking() && !isDead && !isPick)
                pMove.Move(moveInput, mainCamera);
            pAttack.Update();
            if (OpenInventoryAction.WasPressedThisFrame()) OpenInventory();
            if (OpenQuestAction.WasPressedThisFrame())
                OpenQuest();
        }
        else if (input.currentActionMap.name == "Inventory") {
            HandleUI();
        }
        else if (input.currentActionMap.name == "Quest") {
            HandleQuestUI();
        }
    }

    void OpenQuest() {
        QuestBoardManager.Instance.OpenQuestBoard(); // ← Quest UI 管理用のシングルトンを用意
        input.SwitchCurrentActionMap("Quest");       // Questアクションマップに切り替え
                                                     // Questアクションマップの入力を取得
    }

    void CloseQuest() {
        QuestBoardManager.Instance.CloseQuestBoard();
        input.SwitchCurrentActionMap("Gameplay");    // 戻す
    }

    void HandleQuestUI() {

        // 移動
        if (moveInput.y > 0) QuestBoardManager.Instance.MoveUp();
        if (moveInput.y < 0) QuestBoardManager.Instance.MoveDown();
        if (moveInput.x > 0) QuestBoardManager.Instance.MoveRight();
        if (moveInput.x < 0) QuestBoardManager.Instance.MoveLeft();

        // 決定
        if (input.actions["Select"].WasPressedThisFrame()) {
            QuestBoardManager.Instance.AcceptQuest();
        }

        // キャンセル
        if (input.actions["Cancel"].WasPressedThisFrame()) {
            CloseQuest(); // QuestUIを閉じてGameplayに戻す
        }
        moveInput = Vector2.zero; // 移動入力もリセット
    }

    void OpenInventory() {
        InventoryManager.Instance.OpenInventory();
        input.SwitchCurrentActionMap("Inventory");
    }

    void CloseInventory() {
        InventoryManager.Instance.CloseInventory();
        input.SwitchCurrentActionMap("Gameplay");
    }

    void HandleUI() {

        // カーソル移動
        if (uiMoveInput.y > 0) InventoryManager.Instance.MoveUp();
        if (uiMoveInput.y < 0) InventoryManager.Instance.MoveDown();
        if (uiMoveInput.x > 0) InventoryManager.Instance.MoveRight();
        if (uiMoveInput.x < 0) InventoryManager.Instance.MoveLeft();

        if (input.actions["Select"].WasPressedThisFrame()) {
            InventoryManager.Instance.HandleSelect();
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
    public virtual void TakeDamage(int attack, float motionMultiplier = 1,
                                   float criticalChance = 0, float criticalMultiplier = 2,
                                   int elementalValue = 0, float staggerValue = 0) {
        if (isDead || IsSkillActive) return;
        isPick = false;


        int damage;
        isCritical = Random.value < criticalChance; // クリティカル判定

        if (isCritical) {
            damage = Mathf.RoundToInt(
                (Mathf.Pow(attack, 2) / attack + defence) *
                motionMultiplier * Random.Range(0.90f, 1.1f) * criticalMultiplier);
            hp -= damage;
        }
        else {
            damage = Mathf.RoundToInt(
                (Mathf.Pow(attack, 2) / attack + defence) *
                motionMultiplier * Random.Range(0.90f, 1.1f));
            hp -= damage;
        }

        pAttack.AttackEnd();

        if (hp <= 0) Dead();
        else anim.SetTrigger("Hit");
    }

    private void Dead() {
        if (hp <= 0) {
            isDead = true;
            anim.SetTrigger("Death");
        }
    }

    public void DeathAnimationEnd() {
        isDead = false;
        transform.position = startPos.transform.position;
        hp = maxHp;
    }

    private void OnTriggerStay(Collider other) {
        if (other.gameObject.CompareTag("GatheringPoint") && GatherAction.WasPressedThisFrame()) {
            var point = other.gameObject.GetComponent<GatheringPoint>();
            if (point != null && point.IsAvailable && !pMove.IsMoving()) {
                anim.SetTrigger("Pick");
                isPick = true;
                
            }
        }
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
}
