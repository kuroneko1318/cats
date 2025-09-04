using UnityEngine;

public class AttackHitbox : MonoBehaviour {
    private EnemyBase enemy;

    private void Start() {
        enemy = GetComponentInParent<EnemyBase>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            PlayerBase player = other.GetComponent<PlayerBase>();
            if (player != null && enemy != null) {
                player.TakeDamage(enemy.attack, 1.0f, 0.1f, 1.5f);
            }
        }
    }
}
