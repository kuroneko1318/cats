using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// プレイヤーコントローラー（移動・回避・攻撃・スキル統合）
/// 攻撃中・スキル中は移動不可
/// </summary>
public class PlayerBase : MonoBehaviour {
    private PlayerInput input;
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

    void Start() {
        input = GetComponent<PlayerInput>();
        anim = GetComponent<Animator>();
        mainCamera = Camera.main.transform;

        GatherAction = input.actions["Gather"];
        OpenInventoryAction = input.actions["Menu"];
        pMove = new NewPlayerMove(transform, anim);
        pAttack = new NewPlayerAttack(anim, attackCollider1, attackCollider2, attackCollider3);

        swordTrail = trailObject.GetComponent<TrailRenderer>();

        // スキルマネージャーを生成してスキルを登録
        skillManager = new SkillManager();
        skillManager.RegisterSkill(new FrontSlashSkill());

        // 移動入力
        input.actions["Move"].performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.actions["Move"].canceled += ctx => moveInput = Vector2.zero;

        // 攻撃入力
        input.actions["Attack"].performed += ctx => {
            if (!IsSkillActive) // スキル中は攻撃不可
                pAttack.Attack();
        };

        // 回避入力
        input.actions["Avoidance"].performed += ctx => {
            if (!pAttack.IsAttacking() && !IsSkillActive) // 攻撃中/スキル中は回避不可
            {
                pMove.Avoid(moveInput, mainCamera);
            }
        };

        // スキル入力
        input.actions["Skill"].performed += ctx => {
            if (!IsSkillActive && !pAttack.IsAttacking() && !isDead && !isPick) {
                skillManager.UseSkill(0, gameObject); // 0番スキルを発動
            }
        };
    }

    void Update() {
        // 攻撃中やスキル中は移動不可
        if (!pAttack.IsAttacking() && !IsSkillActive && !isDead && !isPick) {
            pMove.Move(moveInput, mainCamera);
        }

        pAttack.Update();

        if (OpenInventoryAction.WasPressedThisFrame()) {
            InventoryManager.Instance.OpenInventory();
        }
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
            if (point != null && point.IsAvailable) {
                anim.SetTrigger("Pick");
                isPick = true;
                point.Interact();
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
