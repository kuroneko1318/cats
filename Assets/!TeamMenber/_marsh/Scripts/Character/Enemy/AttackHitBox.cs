using UnityEngine;

public class AttackHitbox : MonoBehaviour {
    public int damage;

    public void SetDamage(int power) {
        damage = power;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            PlayerBase player = other.GetComponent<PlayerBase>();
            if (player != null) {
                player.TakeDamage(damage, 1.0f, 0.1f, 1.5f);
            }
        }
    }
}
