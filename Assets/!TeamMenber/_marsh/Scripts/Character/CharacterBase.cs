using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//全キャラ共通のステータス

public abstract class CharacterBase : MonoBehaviour {

    public int hp;
    public int maxHp;
    public int attack;
    public float stamina;
    public float moveSpeed;
    public bool isInvincible = false;

    public abstract void TakeDamage(int attack, int elementalValue = 0, float staggerValue = 0);
    public abstract void HealHp();
    public abstract void Dead();
    public abstract void Attack();
    public abstract void Move();


}