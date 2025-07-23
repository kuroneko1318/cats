using System.Collections;
using UnityEngine;


public class Dummy : EnemyBase {
    protected virtual void Awake() {
        animator = GetComponent<Animator>();
    }


    private void Update() {
        //デバッグでJキーでダメージ
        if (Input.GetKeyDown(KeyCode.J)) TakeDamage(100, 1.1f, 0.3f, 2);
    }


    public override void Attack() { }
    public override void Dead() { }
    public override void HealHp() { }
    public override void Move() { }
}
