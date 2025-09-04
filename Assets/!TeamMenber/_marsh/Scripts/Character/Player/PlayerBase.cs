using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

//Update にプレイヤーの操作ぶち込む

public class PlayerBase : CharacterBase {

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

}
