using System.Collections;
using UnityEngine;


public class Dummy : EnemyBase {
    protected virtual void Awake() {
        animator = GetComponent<Animator>();
    }


    protected override void Update() {
        //デバッグでJキーでダメージ
        if (Input.GetKeyDown(KeyCode.J)) TakeDamage(100, 1.1f, 0.3f, 2);
    }

    public override void Dead() {
        hp = maxHp;
    }
}
