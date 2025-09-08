using UnityEngine;

public class AttackHitbox : MonoBehaviour {
    private int attackPower;

    public void SetDamage(int power) {
        attackPower = power;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            PlayerBase player = other.GetComponent<PlayerBase>();
            if (player != null) {
                player.TakeDamage(attackPower, 1.0f, 0.1f, 1.5f);
            }
        }
    }
}
