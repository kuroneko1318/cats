using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

//Update にプレイヤーの操作ぶち込む

public class PlayerBase : CharacterBase {

    public PlayerInput input;
    public InputAction moveAction;
    public InputAction jumpAction;
    public InputAction attackAction;
    public InputAction avoidanceAction;
    public InputAction GatherAction;
    public GameObject cam;

    public Rigidbody rb;
    public Collider damageCollider;

    public Vector3 latestPos;
    public Vector3 direction;
    public Vector3 diff;

    public bool isAvoiding = false;
    public static bool attackFlag = false;
    private float avoidanceTimer = 0;
    private float avoidanceDuration = 0.002f;

    private float avoidanceCooldown = 1f;
    private float cooldownTimer = 0f;

    public Vector3 initialPosition;

    //会心率　最大値は1.00f
    public float criticalChance;
    //会心ダメージ倍率
    public float criticalMultiplier;
    private const int _WEAPON_ID = 1001;

    private string inputItemString;
    [SerializeField]
    private TMP_InputField EquipmentinputField;
    public ItemManager itemManager;
    public ItemManager weaponManager;
    WeaponBase weapon;
    Inventory inventory = null;
    [SerializeField] RawImage swordImage;

    [SerializeField]
    public Animator animator;

    bool isCritical = false;

    void Update() {

    }

    public override void Attack() {
        throw new System.NotImplementedException();
    }

    public override void Dead() {
        throw new System.NotImplementedException();
    }

    public override void HealHp() {
        throw new System.NotImplementedException();
    }

    public override void Move() {
        animator.SetBool("Run", true);
    }


    public virtual void TakeDamage(int attack, float motionMultiplier = 1, float criticalChance = 0, float criticalMultiplier = 2,
                                   int elementalValue = 0, float staggerValue = 0) {
        isCritical = Random.value < criticalChance; // 20%でクリティカル
        if (isCritical) {
            damage = Mathf.RoundToInt
                ((Mathf.Pow(attack, 2) / attack + defence) * motionMultiplier * Random.Range(0.90f, 1.1f) * criticalMultiplier);
            hp -= damage;
        }
        else {
            damage = Mathf.RoundToInt
                ((Mathf.Pow(attack, 2) / attack + defence) * motionMultiplier * Random.Range(0.90f, 1.1f));
            hp -= damage;
        }
        animator.SetBool("Hit", true); // アニメーション切り替え
        StartCoroutine(ResetHitFlagAfterDelay(0.1f)); // 0.3秒後に戻す

        if (hp <= 0) Dead();
    }
    private IEnumerator ResetHitFlagAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        animator.SetBool("Hit", false);
    }



    public void Death() {
        if (hp <= 0) {
            animator.SetTrigger("Death");
        }
    }

    public void PlayerMove() {
        if (isAvoiding) return;

        Vector2 inputVector = moveAction.ReadValue<Vector2>();
        if (inputVector != Vector2.zero) {
            animator.SetBool("Run", true);

            Vector3 camForward = cam.transform.forward;
            Vector3 camRight = cam.transform.right;

            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDir = camForward * inputVector.y + camRight * inputVector.x;

            rb.velocity = moveDir * moveSpeed;
            direction = moveDir;

            transform.rotation = Quaternion.LookRotation(moveDir);
        }
        else {
            animator.SetBool("Run", false);
        }
    }


    public void RotMove() {
        Vector2 inputVector = moveAction.ReadValue<Vector2>();
        if (inputVector != Vector2.zero) {
            Vector3 camForward = cam.transform.forward;
            Vector3 camRight = cam.transform.right;

            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDir = camForward * inputVector.y + camRight * inputVector.x;
            transform.rotation = Quaternion.LookRotation(moveDir);
        }
    }

    public void Avoidance() {
        if (cooldownTimer > 0f) {
            cooldownTimer -= Time.deltaTime;
            return;
        }

        if (avoidanceAction.WasPressedThisFrame() && !isAvoiding) {
            animator.SetTrigger("Avoidance");

            isAvoiding = true;
            isInvincible = true;
            avoidanceTimer = avoidanceDuration;
            cooldownTimer = avoidanceCooldown;

            rb.velocity = transform.forward * moveSpeed * 2f;
        }

        if (isAvoiding) {
            avoidanceTimer -= Time.deltaTime;
            if (avoidanceTimer <= 0f) {
                isAvoiding = false;
                isInvincible = false;
            }
        }
    }

    public void Stop() {
        if (!isAvoiding && !moveAction.IsInProgress() && !avoidanceAction.IsInProgress()) {
            rb.velocity = Vector3.zero;
        }
    }


    public void AddAttack(int _weaponATK) {
        attack += _weaponATK;
    }



    private void OnTriggerStay(Collider other) {
        if (other.gameObject.CompareTag("GatheringPoint") && GatherAction.WasPressedThisFrame()) {
            var point = other.gameObject.GetComponent<GatheringPoint>();
            point?.Gather();
        }
    }

    // 死亡アニメーション終了後に呼ばれる
    public void OnDeathAnimationEnd() {
        transform.position = initialPosition;
        hp = maxHp;
        animator.ResetTrigger("Death");
        // 必要に応じて他の初期化処理
    }
}
