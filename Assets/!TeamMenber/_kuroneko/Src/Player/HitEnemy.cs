using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEnemy : MonoBehaviour
{
    int damage;

    public void SetDamage(int power) {
        damage = power;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Enemy")) {
            EnemyBase enemy = other.GetComponent<EnemyBase>();
            if (enemy != null) {
                enemy.TakeDamage(damage, 1.0f, 0.1f, 1.5f);
            }
        }
    }
}
