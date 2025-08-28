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
    protected bool getM = false;
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

    
    public virtual void TakeDamage(int attack, int elementalValue = 0, float staggerValue = 0) {
        damage = Mathf.RoundToInt
            ((Mathf.Pow(attack, 2) / attack + defence) * Random.Range(0.95f, 1.05f));
        hp -= damage;
        if(hp < 0) Dead();
    }

    
}
