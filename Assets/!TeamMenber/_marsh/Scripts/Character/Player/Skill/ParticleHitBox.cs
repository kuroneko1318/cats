using UnityEngine;

public class ParticleHitbox : MonoBehaviour {

    public float skillDamageMultiplier;

    protected PlayerBase player = null;

    private void OnTriggerEnter(Collider other) {
        if(player == null) player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBase>();

        if (other.CompareTag("Enemy")) {
            var enemy = other.GetComponent<EnemyBase>(); // 敵のベースクラス
            if (enemy != null) {
                enemy.TakeDamage(player.attack, skillDamageMultiplier, player.criticalChance, player.criticalMultiplier);
            }
        }
    }
}
