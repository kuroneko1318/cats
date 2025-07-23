using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : CharacterBase {
    [SerializeField] private GameObject damagePopupPrefab;
    private DamagePopupController currentPopup;
    bool isCritical = false;
    public Animator animator;

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
        throw new System.NotImplementedException();
    }

    public virtual void TakeDamage(int attack,float motionMultiplier, float criticalChance, float criticalMultiplier,
                                    int elementalValue = 0, float staggerValue = 0) {
        isCritical = Random.value < criticalChance; // 20%でクリティカル
        if (isCritical) {
            damage = Mathf.RoundToInt
                ((Mathf.Pow(attack, 2) / attack + defence) * motionMultiplier * Random.Range(0.95f, 1.05f) * criticalMultiplier);
            hp -= damage;
        }
        else {
            damage = Mathf.RoundToInt
                ((Mathf.Pow(attack, 2) / attack + defence) * motionMultiplier * Random.Range(0.95f, 1.05f));
            hp -= damage;
        }
        animator.SetBool("Hit", true); // アニメーション切り替え
        StartCoroutine(ResetHitFlagAfterDelay(0.1f)); // 0.3秒後に戻す

        // 既存のポップアップが存在し、まだフェードアウトしていない場合は加算
        if (currentPopup != null && !currentPopup.IsFadingOut) {
            currentPopup.AddDamage(damage, isCritical);
        }
        else {
            // 新しいポップアップを生成
            Vector3 popupPos = transform.position + Vector3.up * 2f;
            GameObject popupObj = Instantiate(damagePopupPrefab, popupPos, Quaternion.identity);
            currentPopup = popupObj.GetComponent<DamagePopupController>();

            currentPopup.AddDamage(damage, isCritical);

        }

        if (hp < 0) Dead();
    }

    private IEnumerator ResetHitFlagAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        animator.SetBool("Hit", false);
    }
}
