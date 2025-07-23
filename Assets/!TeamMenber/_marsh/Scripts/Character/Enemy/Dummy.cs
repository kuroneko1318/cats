using System.Collections;
using UnityEngine;


public class Dummy : EnemyBase {
    protected virtual void Awake() {
        animator = GetComponent<Animator>();
    }


    private void Update() {
        //デバッグでJキーでダメージ
        if (Input.GetKeyDown(KeyCode.J)) TakeDamage(10, 1.1f, 0.3f, 2);
    }

    private IEnumerator ResetHitFlagAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        animator.SetBool("Hit", false);
    }


    public override void Attack() { }
    public override void Dead() { }
    public override void HealHp() { }
    public override void Move() { }
}
