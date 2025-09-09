using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damageene : MonoBehaviour
{
    int damage;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        damage = PlayerController.attacker;
    }

    public void SetDamage(int power) {
        damage = power;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Enemy")) {
            EnemyBase player = other.GetComponent<EnemyBase>();
            if (player != null) {
                player.TakeDamage(damage, 1.0f, 0.1f, 1.5f);
            }
        }
    }
}
