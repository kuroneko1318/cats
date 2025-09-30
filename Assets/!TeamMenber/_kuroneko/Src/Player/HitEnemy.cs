using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEnemy : MonoBehaviour
{
    int damage;

    private GameObject player;

    private void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void SetDamage(int power) {
        damage = power;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Enemy")) {
            EnemyBase enemy = other.GetComponent<EnemyBase>();
            if (enemy != null) {
                enemy.TakeDamage(damage, 1.0f, player.GetComponent<PlayerBase>().criticalChance, player.GetComponent<PlayerBase>().criticalMultiplier);
            }
        }
    }
}
