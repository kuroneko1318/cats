using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// プレイヤーコントローラー（移動・回避・攻撃統合＋攻撃中は移動不可）
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
    // 会心率（最大値は1.00f）
    [SerializeField] public float criticalChance;
    // 会心ダメージ倍率
    [SerializeField] public float criticalMultiplier;
    private bool isCritical = false; // クリティカル判定

    [Header("攻撃判定用コライダー")]
    [SerializeField] private GameObject attackCollider1;
    [SerializeField] private GameObject attackCollider2;
    [SerializeField] private GameObject attackCollider3;

    [Header("リスポーン地点（街の初期位置保存用）")]
    [SerializeField] private GameObject startPos;


    void Start() {
        input = GetComponent<PlayerInput>();
        anim = GetComponent<Animator>();
        mainCamera = Camera.main.transform;

        pMove = new NewPlayerMove(transform, anim);
        pAttack = new NewPlayerAttack(anim, attackCollider1, attackCollider2, attackCollider3);

        // 入力イベント登録
        input.actions["Move"].performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.actions["Move"].canceled += ctx => moveInput = Vector2.zero;

        // 攻撃入力
        input.actions["Attack"].performed += ctx => {
            pAttack.Attack();
        };

        // 回避入力（攻撃中は無効化）
        input.actions["Avoidance"].performed += ctx => {
            if (!pAttack.IsAttacking()) {
                pMove.Avoid(moveInput, mainCamera);
            }
        };
    }

    void Update() {
        // 攻撃中は移動不可
        if (!pAttack.IsAttacking()) {
            pMove.Move(moveInput, mainCamera);
        }

        pAttack.Update();

        
    }

    // ダメージ処理
    public virtual void TakeDamage(int attack, float motionMultiplier = 1, float criticalChance = 0, float criticalMultiplier = 2,

                                   int elementalValue = 0, float staggerValue = 0) {

        int damage;

        isCritical = Random.value < criticalChance; // クリティカル判定

        if (isCritical) {

            damage = Mathf.RoundToInt(

                (Mathf.Pow(attack, 2) / attack + defence) * motionMultiplier * Random.Range(0.90f, 1.1f) * criticalMultiplier);

            hp -= damage;

        }

        else {

            damage = Mathf.RoundToInt(

                (Mathf.Pow(attack, 2) / attack + defence) * motionMultiplier * Random.Range(0.90f, 1.1f));

            hp -= damage;

        }
        if (hp <= 0) Dead(); // HPが0以下なら死亡処理

        anim.SetTrigger("Hit"); // 被ダメージアニメーション


    }

    private void Dead() {
        if (hp <= 0) {

            anim.SetTrigger("Death");

            DeathAnimationEnd();

        }
    }

    public void DeathAnimationEnd() {
        transform.position = startPos.transform.position;

        hp = maxHp;

        anim.ResetTrigger("Death");
    }

    // Animatorイベント用ラッパー
    public void AttackStartEvent() {
        pAttack.AttackStart();
        pAttack.SetPower(attack);
    }

    public void AttackEndEvent() {
        pAttack.AttackEnd();
    }

    public void ResetAttackFlag() {
        pAttack.ResetAttack();
    }

}
