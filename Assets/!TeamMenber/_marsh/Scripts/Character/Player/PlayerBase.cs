using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

//Update にプレイヤーの操作ぶち込む

public class PlayerBase : CharacterBase {

    //会心率　最大値は1.00f
    public float criticalChance;
    //会心ダメージ倍率
    public float criticalMultiplier;

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

    private void OnTriggerStay(Collider other) {
        if (other.gameObject.CompareTag("GatheringPoint") && Input.GetKeyDown(KeyCode.F)) {
            var point = other.gameObject.GetComponent<GatheringPoint>();
            point?.Interact();
        }
    }
}
